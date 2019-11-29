using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Input;
using Pulsarc.Utils.SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    public class SongSelection : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }
        SongSelectionView GetSongSelectionView() { return (SongSelectionView)View; }

        // All cards that can be played.
        public List<BeatmapCard> Cards { get; private set; }

        // Used for determining the position of all the cards and moving them with mouse scrolling
        private int lastScrollValue = 0;
        private bool leftClicking = false;
        private MouseState leftClickedState;
        public float SelectedFocus = 0;
        public BeatmapCard FocusedCard { get; set; }

        public SongSelection()
        { }

        public override void Init()
        {
            base.Init();
            RefreshBeatmaps();
        }

        /// <summary>
        /// Load all beatmaps.
        /// TODO: this should be loaded from a cache and updated incrementally or when doing a refresh
        /// </summary>
        public void RefreshBeatmaps(string keyword = "")
        {
            List<Beatmap> beatmaps = new List<Beatmap>();
            Cards = new List<BeatmapCard>();

            List<BeatmapData> data = DataManager.BeatmapDB.GetBeatmaps();

            for (int i = 0; i < data.Count; i++)
                if (keyword == "" || data[i].Match(keyword))
                    beatmaps.Add(BeatmapHelper.LoadLight(data[i]));

            // TODO: Allow user to choose sorting method.
            beatmaps = SortBeatmaps(beatmaps, "difficulty");

            View = new SongSelectionView(this, beatmaps, keyword);
        }

        /// <summary>
        /// Rescan the Beatmap data
        /// </summary>
        public void RescanBeatmaps()
        {
            List<Beatmap> beatmaps = new List<Beatmap>();

            DataManager.BeatmapDB.ClearBeatmaps();

            string[] directories = Directory.GetDirectories("Songs/");

            for (int i = 0; i < directories.Length; i++)
            {
                string[] files = Directory.GetFiles(directories[i], "*.psc").Select(Path.GetFileName).ToArray();

                for (int j = 0; j < files.Length; j++)
                {
                    BeatmapData debugData = new BeatmapData(BeatmapHelper.Load(directories[i], files[j]));
                    DataManager.BeatmapDB.AddBeatmap(debugData);
                }
            }

            RefreshBeatmaps(GetSongSelectionView().SearchBox.GetText());
        }

        /// <summary>
        /// Sort the beatmaps by the provided metadata string.
        /// </summary>
        /// <param name="beatmaps">The list of beatmaps to sort.</param>
        /// <param name="sort">The way to sort. "Difficulty," "Artist", "Title", "Mapper", or "Version"</param>
        /// <param name="ascending">Whether the list should be sorted Ascending (A->Z,1->9), or Descending (Z->A,9->1)</param>
        /// <returns></returns>
        public List<Beatmap> SortBeatmaps(List<Beatmap> beatmaps, string sort, bool ascending = true)
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
            HandleKeyboardPresses();
            HandleMouseInput();
            
            View?.Update(gameTime);
        }

        private void HandleKeyboardPresses()
        {
            while (InputManager.KeyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.KeyboardPresses.Dequeue();

                // If Escape has been pressed, go back one screen.
                if (press.Value == Keys.Escape)
                    ScreenManager.RemoveScreen(true);

                // If F5 has been pressed, refresh beatmaps
                else if (press.Value == Keys.F5)
                    RescanBeatmaps();

                // If Enter is pressed, start playing the beatmap of the focused card
                else if (press.Value == Keys.Enter)
                    FocusedCard.OnClick();

                // If Delete or backspace is pressed, clear the search bar
                else if (press.Value == Keys.Delete || press.Value == Keys.Back)
                {
                    GetSongSelectionView().SearchBox.Clear();
                    RefreshBeatmaps();
                }
                // If none of the above, type into the search bar
                // TODO: Ignore keypressses like CTRL, SHIFT, ALT, etc
                // TODO? Ignore keypresses unless clicked on
                else
                {
                    if (XnaKeyHelper.isTypingCharacter(press.Value))
                    {
                        GetSongSelectionView().SearchBox.AddText(InputManager.Caps ? XnaKeyHelper.GetStringFromKey(press.Value) : XnaKeyHelper.GetStringFromKey(press.Value).ToLower());
                        RefreshBeatmaps(GetSongSelectionView().SearchBox.GetText().ToLower());
                    }
                }
            }
        }

        private void HandleMouseInput()
        {
            MouseState ms = Mouse.GetState();

            ChangeFocus(ms);
            HandleClicks(ms);
        }

        private void ChangeFocus(MouseState ms)
        {
            // If the scroll wheel's state has changed, change the focus
            if (ms.ScrollWheelValue < lastScrollValue)
                SelectedFocus += 0.3f;

            else if (ms.ScrollWheelValue > lastScrollValue)
                SelectedFocus -= 0.3f;

            lastScrollValue = ms.ScrollWheelValue;
        }

        private void HandleClicks(MouseState ms)
        {
            // If the Focused Card is clicked (and released), play its map.
            if (!leftClicking && ms.LeftButton == ButtonState.Pressed)
            {
                leftClicking = true;
                leftClickedState = ms;
            }
            // If a non-focused card is clicked, focus that clicked card
            else if (leftClicking && ms.LeftButton == ButtonState.Released)
            {
                leftClicking = false;
                MouseState leftRelease = ms;

                for (int i = 0; i < Cards.Count; i++)
                {
                    if (Cards[i].Clicked(leftClickedState, leftRelease))
                    {
                        if (FocusedCard != Cards[i])
                        {
                            FocusedCard.SetClicked(false);
                            FocusedCard = Cards[i];
                        }

                        Cards[i].OnClick();
                        GetSongSelectionView().FocusCard(Cards[i]);
                    }
                }
            }
        }
    }
}
