using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    class SongSelection : PulsarcScreen
    {

        public override ScreenView View { get; protected set; }
        SongSelectionView GetSongSelectionView() { return (SongSelectionView)View; }

        // All cards that can be played.
        public List<BeatmapCard> cards;

        // Used for determining the position of all the cards and moving them with mouse scrolling
        int lastScrollValue = 0;
        bool leftClicking = false;
        Vector2 leftClickingPos;
        public float selectedFocus = 0;
        public BeatmapCard focusedCard;

        public SongSelection()
        {
        }

        public override void Init()
        {
            RefreshBeatmaps();
        }

        /// <summary>
        /// Load all beatmaps. TODO: this should be loaded from a cache and updated incrementally or when doing a refresh
        /// </summary>
        public void RefreshBeatmaps(string keyword = "")
        {
            List<Beatmap> beatmaps = new List<Beatmap>();
            cards = new List<BeatmapCard>();

            foreach(BeatmapData data in Pulsarc.beatmapDB.getBeatmaps())
            {
                if (keyword == "" || data.match(keyword))
                {
                    beatmaps.Add(BeatmapHelper.LoadLight(data));
                }
            }

            beatmaps = sortBeatmaps(beatmaps, "difficulty"); // TODO: Allow user to choose sorting method.
            View = new SongSelectionView(this, beatmaps, keyword);
        }

        public void rescanBeatmaps()
        {
            List<Beatmap> beatmaps = new List<Beatmap>();

            Pulsarc.beatmapDB.clearBeatmaps();
            foreach (string dir in Directory.GetDirectories("Songs/"))
            {
                foreach (string file in Directory.GetFiles(dir, "*.psc")
                                     .Select(Path.GetFileName)
                                     .ToArray())
                {
                    Pulsarc.beatmapDB.addBeatmap(new BeatmapData(BeatmapHelper.Load(dir, file)));
                }
            }

            RefreshBeatmaps(GetSongSelectionView().searchBox.getText());
        }

        /// <summary>
        /// Sort the beatmaps by the provided metadata string.
        /// </summary>
        /// <param name="beatmaps">The list of beatmaps to sort.</param>
        /// <param name="sort">The way to sort. "Difficulty," "Artist", "Title", "Mapper", or "Version"</param>
        /// <param name="ascending">Whether the list should be sorted Ascending (A->Z,1->9), or Descending (Z->A,9->1)</param>
        /// <returns></returns>
        public List<Beatmap> sortBeatmaps(List<Beatmap> beatmaps, string sort, bool ascending = true)
        {
            switch(sort)
            {
                case "difficulty":
                    return ascending ? beatmaps.OrderBy(i => i.Difficulty).ToList() : beatmaps.OrderByDescending(i => i.Difficulty).ToList();
                case "artist":
                    return ascending ? beatmaps.OrderBy(i => i.Artist).ToList() : beatmaps.OrderByDescending(i => i.Artist).ToList();
                case "title":
                    return ascending ? beatmaps.OrderBy(i => i.Title).ToList() : beatmaps.OrderByDescending(i => i.Title).ToList();
                case "mapper":
                    return ascending ? beatmaps.OrderBy(i => i.Mapper).ToList() : beatmaps.OrderByDescending(i => i.Mapper).ToList();
                case "version":
                    return ascending ? beatmaps.OrderBy(i => i.Version).ToList() : beatmaps.OrderByDescending(i => i.Version).ToList();
                default:
                    return beatmaps;
            }
        }

        public override void Update(GameTime gameTime)
        {
            handleKeyboardPresses();
            handleMouseInput();
            
            View?.Update(gameTime);
        }

        private void handleKeyboardPresses()
        {
            while (InputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.keyboardPresses.Dequeue();

                // If Escape has been pressed, go back one screen.
                if (press.Value == Keys.Escape)
                {
                    ScreenManager.RemoveScreen(true);
                }
                // If F5 has been pressed, refresh beatmaps
                else if (press.Value == Keys.F5)
                {
                    rescanBeatmaps();
                }
                // If Enter is pressed, start playing the beatmap of the focused card
                else if (press.Value == Keys.Enter)
                {
                    focusedCard.onClick();
                }
                // If Delete or backspace is pressed, clear the search bar
                else if (press.Value == Keys.Delete || press.Value == Keys.Back)
                {
                    GetSongSelectionView().searchBox.clear();
                    RefreshBeatmaps();
                }
                // If none of the above, type into the search bar
                else
                {
                    if (XnaKeyHelper.isTypingCharacter(press.Value))
                    {
                        GetSongSelectionView().searchBox.addText(InputManager.caps ? XnaKeyHelper.GetStringFromKey(press.Value) : XnaKeyHelper.GetStringFromKey(press.Value).ToLower());
                    }
                    RefreshBeatmaps(GetSongSelectionView().searchBox.getText().ToLower());
                }
            }
        }

        private void handleMouseInput()
        {
            MouseState ms = Mouse.GetState();

            changeFocus(ms);
            handleClicks(ms);
        }

        private void changeFocus(MouseState ms)
        {
            // If the scroll wheel's state has changed, change the focus
            if (ms.ScrollWheelValue < lastScrollValue)
            {
                selectedFocus -= 0.3f;
            }
            else if (ms.ScrollWheelValue > lastScrollValue)
            {
                selectedFocus += 0.3f;
            }

            lastScrollValue = ms.ScrollWheelValue;
        }

        private void handleClicks(MouseState ms)
        {
            // If the Focused Card is clicked (and released), play its map.
            if (!leftClicking && ms.LeftButton == ButtonState.Pressed)
            {
                leftClicking = true;
                leftClickingPos = new Vector2(ms.Position.X, ms.Position.Y);
            }
            // If a non-focused card is clicked, focus that clicked card
            else if (leftClicking && ms.LeftButton == ButtonState.Released)
            {
                leftClicking = false;
                Vector2 leftReleasePos = new Vector2(ms.Position.X, ms.Position.Y);
                foreach (BeatmapCard card in cards)
                {
                    if (card.clicked(leftClickingPos) && card.clicked(leftReleasePos))
                    {
                        if (focusedCard != card)
                        {
                            focusedCard.setClicked(false);
                            focusedCard = card;
                        }
                        card.onClick();
                        GetSongSelectionView().focusCard(card);
                    }
                }
            }
        }
    }
}
