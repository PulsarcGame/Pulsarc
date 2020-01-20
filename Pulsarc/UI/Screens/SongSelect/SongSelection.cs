using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Input;
using Pulsarc.Utils.SQLite;
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
        private int _lastScrollValue;
        private bool _leftClicking;
        private MouseState _leftClickedState;
        public float SelectedFocus;
        public BeatmapCard FocusedCard { get; set; }

        public override void Init()
        {
            base.Init();

            RefreshBeatmaps();
        }

        /// <summary>
        /// Load all beatmaps.
        /// TODO: this should be loaded from a cache and updated incrementally or when doing a refresh
        /// </summary>
        private void RefreshBeatmaps(string keyword = "")
        {
            Cards = new List<BeatmapCard>();

            List<BeatmapData> data = DataManager.BeatmapDb.GetBeatmaps();

            List<Beatmap> beatmaps = (from t in data where keyword == "" || t.Match(keyword) select BeatmapHelper.LoadLight(t)).ToList();

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

            DataManager.BeatmapDb.ClearBeatmaps();

            string[] directories = Directory.GetDirectories("Songs/");

            foreach (var t in directories)
            {
                string[] files = Directory.GetFiles(t, "*.psc").Select(Path.GetFileName).ToArray();

                foreach (var s in files)
                {
                    BeatmapData debugData = new BeatmapData(BeatmapHelper.Load(t, s));
                    DataManager.BeatmapDb.AddBeatmap(debugData);
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
        private List<Beatmap> SortBeatmaps(List<Beatmap> beatmaps, string sort, bool ascending = true)
        {
            return sort switch
            {
                "difficulty" => (ascending
                    ? beatmaps.OrderBy(i => i.Difficulty).ToList()
                    : beatmaps.OrderByDescending(i => i.Difficulty).ToList()),
                "artist" => (ascending
                    ? beatmaps.OrderBy(i => i.Artist).ToList()
                    : beatmaps.OrderByDescending(i => i.Artist).ToList()),
                "title" => (ascending
                    ? beatmaps.OrderBy(i => i.Title).ToList()
                    : beatmaps.OrderByDescending(i => i.Title).ToList()),
                "mapper" => (ascending
                    ? beatmaps.OrderBy(i => i.Mapper).ToList()
                    : beatmaps.OrderByDescending(i => i.Mapper).ToList()),
                "version" => (ascending
                    ? beatmaps.OrderBy(i => i.Version).ToList()
                    : beatmaps.OrderByDescending(i => i.Version).ToList()),
                _ => beatmaps
            };
        }

        public override void Update(GameTime gameTime)
        {
            HandleKeyboardPresses();
            HandleMouseInput();
            
            View?.Update(gameTime);
        }

        /// <summary>
        /// Refocus the current card in SongSelectionView to refresh leaderboards.
        /// </summary>
        public override void EnteredScreen()
        {
            UpdateDiscord();

            // Since this UpdateDiscord() is called every time
            // We re-enter the song select screen, reload leaderboards just in case.
            GetSongSelectionView().RefocusCurrentCard();
        }

        protected override void UpdateDiscord()
        {
            PulsarcDiscord.SetStatus("", "Browsing Maps");
        }

        private void HandleKeyboardPresses()
        {
            while (InputManager.KeyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.KeyboardPresses.Dequeue();

                switch (press.Value)
                {
                    // If Escape has been pressed, go back one screen.
                    // If F5 has been pressed, refresh beatmaps
                    case Keys.Escape:
                        ScreenManager.RemoveScreen(true);
                        break;
                    // If Enter is pressed, start playing the beatmap of the focused card
                    case Keys.F5:
                        RescanBeatmaps();
                        break;
                    // If Delete or backspace is pressed, clear the search bar
                    case Keys.Enter:
                        FocusedCard.OnClick();
                        break;
                    case Keys.Delete:
                    // If none of the above, type into the search bar
                    // TODO: Ignore keypressses like CTRL, SHIFT, ALT, etc
                    // TODO? Ignore keypresses unless clicked on
                    case Keys.Back:
                        GetSongSelectionView().SearchBox.Clear();
                        RefreshBeatmaps();
                        break;
                    default:
                    {
                        if (XnaKeyHelper.IsTypingCharacter(press.Value))
                        {
                            GetSongSelectionView().SearchBox.AddText(InputManager.Caps ? XnaKeyHelper.GetStringFromKey(press.Value) : XnaKeyHelper.GetStringFromKey(press.Value).ToLower());
                            RefreshBeatmaps(GetSongSelectionView().SearchBox.GetText().ToLower());
                        }

                        break;
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
            if (ms.ScrollWheelValue < _lastScrollValue)
                SelectedFocus += 0.3f;

            else if (ms.ScrollWheelValue > _lastScrollValue)
                SelectedFocus -= 0.3f;

            _lastScrollValue = ms.ScrollWheelValue;
        }

        private void HandleClicks(MouseState ms)
        {
            // If the Focused Card is clicked (and released), play its map.
            if (!_leftClicking && ms.LeftButton == ButtonState.Pressed)
            {
                _leftClicking = true;
                _leftClickedState = ms;
            }
            // If a non-focused card is clicked, focus that clicked card
            else if (_leftClicking && ms.LeftButton == ButtonState.Released)
            {
                _leftClicking = false;
                MouseState leftRelease = ms;

                foreach (var t in Cards.Where(t => t.Clicked(_leftClickedState, leftRelease)))
                {
                    if (FocusedCard != t)
                    {
                        FocusedCard.SetClicked(false);
                        FocusedCard = t;
                    }

                    t.OnClick();
                    GetSongSelectionView().FocusCard(t);
                }
            }
        }
    }
}
