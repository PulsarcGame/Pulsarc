using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Audio;
using Pulsarc.Utils.Input;
using Pulsarc.Utils.SQLite;
using System;
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

        private float selectedFocus = 0;
        public float SelectedFocus
        {
            get => selectedFocus;
            // Don't set this higher or lower than the min/max focus
            set => selectedFocus =
                value > maxFocus ? maxFocus : value < MIN_FOCUS ? MIN_FOCUS : value;
        }

        private const float MIN_FOCUS = -3;
        private float maxFocus => Cards.Count - 4;

        public BeatmapCard FocusedCard { get; set; }

        // The time when the last key was pressed
        private double lastSearchBoxKeyPressTime;
        private const int SEARCHBOX_REFRESH_TIME = 1000;

        // When this is set to true, the timer resets.
        private bool restartSearchBoxKeyPressTimer;
        private bool RestartSearchBoxKeyPressTimer
        {
            get => restartSearchBoxKeyPressTimer;
            set
            {
                if (value)
                {
                    lastSearchBoxKeyPressTime = PulsarcTime.CurrentElapsedTime;
                }

                restartSearchBoxKeyPressTimer = value;
            }
        }

        // Returns true once a second has passed since the last RestartKeyPressTimer = true call
        private bool OneSecondSinceLastSearchBoxKeyPress
            => PulsarcTime.CurrentElapsedTime >= (lastSearchBoxKeyPressTime + SEARCHBOX_REFRESH_TIME);

        // The last time "Delete" was pressed
        private double lastDeleteKeyPress;
        private const int DELETE_MAP_HOLD_TIME = 3000;

        private bool IsTimeUpSinceLastDeleteKeyPress
            => PulsarcTime.CurrentElapsedTime >= (lastDeleteKeyPress + DELETE_MAP_HOLD_TIME);

        // Whether or not the user has already deleted the map with the current delete hold
        private bool alreadyDeletedMap = false;

        public SongSelection() { }

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
            keyword = keyword.ToLower();

            List<Beatmap> beatmaps = new List<Beatmap>();
            Cards = new List<BeatmapCard>();

            List<BeatmapData> data = DataManager.BeatmapDB.GetBeatmaps();

            for (int i = 0; i < data.Count; i++)
            {
                if (keyword == "" || data[i].Match(keyword))
                {
                    beatmaps.Add(BeatmapHelper.LoadLight(data[i]));
                }
            }

            // TODO: Allow user to choose sorting method.
            beatmaps = SortBeatmaps(beatmaps, "difficulty");

            // Reset global index
            Pulsarc.FocusedCardIndex = -1;

            View = CreateView(this, beatmaps, keyword);
        }

        protected virtual ScreenView CreateView(Screen screen, List<Beatmap> beatmaps, string keyword)
            => new SongSelectionView(this, beatmaps, keyword);

        /// <summary>
        /// Rescan the Beatmap data
        /// </summary>
        public void RescanBeatmaps()
        {
            List<Beatmap> beatmaps = new List<Beatmap>();

            DataManager.BeatmapDB.ClearBeatmaps();

            string[] directories = Array.Empty<string>();

            try
            {
                directories = Directory.GetDirectories("Songs/");
            }
            catch { }

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
                    return ascending
                        ? beatmaps.OrderBy(i => i.Difficulty).ToList()
                        : beatmaps.OrderByDescending(i => i.Difficulty).ToList();
                case "artist":
                    return ascending
                        ? beatmaps.OrderBy(i => i.Artist).ToList()
                        : beatmaps.OrderByDescending(i => i.Artist).ToList();
                case "title":
                    return ascending
                        ? beatmaps.OrderBy(i => i.Title).ToList()
                        : beatmaps.OrderByDescending(i => i.Title).ToList();
                case "mapper":
                    return ascending
                        ? beatmaps.OrderBy(i => i.Mapper).ToList()
                        : beatmaps.OrderByDescending(i => i.Mapper).ToList();
                case "version":
                    return ascending
                        ? beatmaps.OrderBy(i => i.Version).ToList()
                        : beatmaps.OrderByDescending(i => i.Version).ToList();
                default:
                    return beatmaps;
            }
        }

        public void DeleteMap(in BeatmapCard card)
        {
            AudioManager.Stop();

            string folder = new DirectoryInfo(card.Beatmap.Path).FullName;
            try
            {
                Directory.Delete(folder, true);
            }
            catch (Exception e)
            {
                PulsarcLogger.Warning($"Couldn't delete all files in {folder}!" +
                    $"\n\nError:\n\n{e}");
            }

            RescanBeatmaps();

            Pulsarc.FocusedCardIndex = FocusedCard.Index;
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

            if (Pulsarc.FocusedCardIndex >= 0 && Pulsarc.FocusedCardIndex != FocusedCard.Index)
            {
                FocusedCard.SetClicked(false);
                FocusedCard = Cards[Pulsarc.FocusedCardIndex];
                FocusedCard.OnClick();
                GetSongSelectionView().FocusCard(FocusedCard);
            }
            else
            {
                // Since this UpdateDiscord() is called every time
                // We re-enter the song select screen, reload leaderboards just in case.
                GetSongSelectionView().RefocusCurrentCard();
            }
        }

        public override void UpdateDiscord() => PulsarcDiscord.SetStatus("", "Browsing Maps");

        private void HandleKeyboardPresses()
        {
            while (InputManager.PressActions.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.PressActions.Dequeue();

                switch (press.Value)
                {
                    // If F5 has been pressed, refresh beatmaps
                    case Keys.F5:
                        RescanBeatmaps();
                        break;
                    // If Enter is pressed, start playing the beatmap of the focused card
                    case Keys.Enter:
                        FocusedCard.OnClick();
                        break;
                    // If Escape has been pressed with no text in the searchbox, go back one screen.
                    case Keys.Escape:
                        if (GetSongSelectionView().SearchBox.GetText().Length <= 0)
                        {
                            LeaveScreen();
                            break;
                        }
                        // If there was text in the searchbox, fall through to Keys.Delete, which
                        // clears the box
                        goto case Keys.Delete;
                    // If Delete is pressed, clear the search bar
                    case Keys.Delete:
                        lastDeleteKeyPress = PulsarcTime.CurrentElapsedTime;

                        // If there's nothing in the box, don't refresh.
                        if (GetSongSelectionView().SearchBox.GetText().Length <= 0) { break; }

                        GetSongSelectionView().SearchBox.Clear();
                        RefreshBeatmaps();

                        // Stop the timer to prevent a second refresh
                        RestartSearchBoxKeyPressTimer = false;
                        break;
                    // If the backspace is pressed, delete the last character
                    case Keys.Back:
                        // If there's nothing in the box, don't do anything
                        if (GetSongSelectionView().SearchBox.GetText().Length <= 0)
                        {
                            break;
                        }

                        // Reset the timer
                        RestartSearchBoxKeyPressTimer = true;

                        GetSongSelectionView().SearchBox.DeleteLastCharacter();
                        break;
                    // If none of the above, type into the search bar
                    // TODO? Ignore keypresses unless clicked on
                    default:
                        // If the press is not an acceptable typing character, break
                        if (!XnaKeyHelper.IsTypingCharacter(press.Value)) { break; }

                        // Reset the timer
                        RestartSearchBoxKeyPressTimer = true;

                        string key = XnaKeyHelper.GetStringFromKey(press.Value);

                        // If caps/shift isn't on, lowercase the text
                        if (!InputManager.Caps)
                        {
                            key = key.ToLower();
                        }

                        GetSongSelectionView().SearchBox.AddText(key);
                        break;
                }
            }

            // If one second has passed since the last search box key press, refresh the maps.
            if (RestartSearchBoxKeyPressTimer && OneSecondSinceLastSearchBoxKeyPress)
            {
                // Don't call this block every frame
                RestartSearchBoxKeyPressTimer = false;

                RefreshBeatmaps(GetSongSelectionView().SearchBox.GetText());
            }
            // If the user has been holding on to Delete for more than a second,
            // delete the currently focused map
            else if (InputManager.PressedKeys.Contains(Keys.Delete))
            {
                if (IsTimeUpSinceLastDeleteKeyPress && !alreadyDeletedMap)
                {
                    DeleteMap(FocusedCard);
                    alreadyDeletedMap = true;
                }
            }
            else if (alreadyDeletedMap)
            {
                alreadyDeletedMap = false;
            }
        }

        public void LeaveScreen()
        {
            Pulsarc.FocusedCardIndex = FocusedCard.Index;
            ScreenManager.RemoveScreen(true);
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
