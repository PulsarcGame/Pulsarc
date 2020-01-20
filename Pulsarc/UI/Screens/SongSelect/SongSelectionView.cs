using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using Pulsarc.Utils.SQLite;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    public class SongSelectionView : ScreenView
    {
        private SongSelection GetSongSelection() { return (SongSelection)Screen; }

        readonly SongSelection _songSelectScreen;

        // SearchBox
        public SearchBox SearchBox { get; private set; }

        // Background image of Song Select
        private static Background DefaultBackground => new Background("select_background");

        // Current Scores to display
        private readonly List<ScoreCard> _scores;

        private readonly List<BeatmapCard> _cards;

        // Back button to the Main Menu
        private readonly ReturnButton _buttonBack;

        // Focus (used for scrolling through the menu)
        private float _currentFocus, _lastFocus;
        
        // Background changing stuff.
        private bool ChangingBackground { get; set; }
        private Background CurrentBackground { get; set; }
        private Background _oldBackground;
        private const int BackgroundFadeTime = 200;

        /// <summary>
        /// The view for the SongSelect Screen.
        /// TODO: Cleanup this initializer
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="beatmaps">The list of beatmaps to show on this screen.</param>
        /// <param name="search">The starting string value of the SearchBar. Default is ""</param>
        public SongSelectionView(Screen screen, List<Beatmap> beatmaps, string search = "") : base(screen)
        {
            _songSelectScreen = GetSongSelection();

            _cards = _songSelectScreen.Cards;
            _scores = new List<ScoreCard>();

            // Prepare backgrounds
            CurrentBackground = DefaultBackground;
            _oldBackground = DefaultBackground;

            // Set up beatmap cards
            for (int i = 0; i < beatmaps.Count; i++)
                _cards.Add(new BeatmapCard(beatmaps[i], i));

            // Select a random map by default in the song selection.
            if (_cards.Count > 0)
            {
                Random rd = new Random();

                _songSelectScreen.FocusedCard = _cards[rd.Next(0, _cards.Count)];
                _songSelectScreen.FocusedCard.OnClick();
                FocusCard(_songSelectScreen.FocusedCard);
            }

            Anchor searchBoxAnchor = getSkinnablePropertyAnchor("SearchBarAnchor");
            Vector2 searchBarStartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "SearchBarStartPos");

            SearchBox = new SearchBox(search, searchBarStartPosition, searchBoxAnchor);

            int searchBarX = getSkinnablePropertyInt("SearchBarX");
            int searchBarY = getSkinnablePropertyInt("SearchBarY");
            SearchBox.Move(searchBarX, searchBarY);

            _buttonBack = new ReturnButton("select_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
        }

        #region GetSkinnable Methods
        /// <summary>
        /// Find a float from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float GetSkinnablePropertyFloat(string key)
        {
            return Skin.GetConfigFloat("song_select", "Properties", key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePropertyInt(string key)
        {
            return Skin.GetConfigInt("song_select", "Properties", key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePropertyAnchor(string key)
        {
            return Skin.GetConfigAnchor("song_select", "Properties", key);
        }

        /// <summary>
        /// Find a string from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string GetSkinnablePropertyString(string key)
        {
            return Skin.GetConfigString("song_select", "Properties", key);
        }
        #endregion

        /// <summary>
        /// Select a card and change the focus of
        /// the Song Select screen accordingly.
        /// </summary>
        /// <param name="card">The card to focus on.</param>
        /// <param name="restart"></param>
        public void FocusCard(in BeatmapCard card, bool restart = false)
        {
            if (_cards[card.Index] != card)
                return;

            _songSelectScreen.SelectedFocus = card.Index - 3;

            if (restart)
            {
                FocusCard(_cards[0]);
                _cards[0].SetClicked(false);
            }

            // Set to "selected" state
            card.SetClicked(true);

            // Load Beatmap
            card.Beatmap = BeatmapHelper.Load(card.Beatmap.Path, card.Beatmap.FileName);

            string backgroundPath = card.Beatmap.Path + "/" + card.Beatmap.Background;
            Texture2D backgroundTexture = AssetsManager.Load(backgroundPath);

            StartChangingBackground(backgroundTexture);

            UpdateScoreCard(card);
        }

        public void RefocusCurrentCard()
        {
            UpdateScoreCard(GetSongSelection().FocusedCard);
        }

        /// <summary>
        /// Finish the selecting process for the card we're
        /// focusing on.
        /// </summary>
        /// <param name="card">The card we're focusing on.</param>
        private void UpdateScoreCard(in BeatmapCard card)
        {
            if (card == null)
                return;

            _scores.Clear();

            // Make a ScoreCard for each score.
            List<ScoreData> scoresInMap = card.Beatmap.GetLocalScores();

            int rank = 1;
            for (int i = 0; i < scoresInMap.Count; i++)
                _scores.Add(new ScoreCard(scoresInMap[i], rank++));
        }
              
        /// <summary>
        /// Start changing backgrounds from the old to the
        /// new provided one.
        /// </summary>
        /// <param name="newBackground">The background to change to.</param>
        private void StartChangingBackground(Texture2D newBackground)
        {
            if (newBackground != null)
            {
                ChangingBackground = true;
                _oldBackground.ChangeBackground(CurrentBackground.Texture);
                CurrentBackground.ChangeBackground(newBackground);
                CurrentBackground.Opacity = 0;
            }
            else
            {
                if (CurrentBackground.Texture == DefaultBackground.Texture) return;
                ChangingBackground = true;
                _oldBackground = CurrentBackground;
                CurrentBackground = DefaultBackground;
                CurrentBackground.Opacity = 0;
            }
        }

        public override void Destroy()
        { }

        /// <summary>
        /// Draw everything.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            DrawBackgrounds();

            foreach (var t in _cards)
            {
                t.AdjustClickDistance();
                t.Draw();
            }

            foreach (var t in _scores)
                t.Draw();

            _buttonBack.Draw();
            SearchBox.Draw();
        }

        /// <summary>
        /// Draw any backgrounds on screen and calculate opacity changes.
        /// </summary>
        private void DrawBackgrounds()
        {
            if (ChangingBackground && CurrentBackground.Opacity < 1)
            {
                _oldBackground.Draw();
                CurrentBackground.Opacity += (float)PulsarcTime.DeltaTime / BackgroundFadeTime;

                if (CurrentBackground.Opacity > 1)
                {
                    CurrentBackground.Opacity = 1;
                    ChangingBackground = false;
                }
            }

            CurrentBackground.Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            ref float selectedFocus = ref _songSelectScreen.SelectedFocus;

            // Move cards if focus has changed.
            // Rounding is used so this isn't called over and over
            // When currentFocus refuses to go above x.99999999...
            if (Math.Round(_currentFocus, 2) != Math.Round(selectedFocus, 2))
            {
                _currentFocus = PulsarcMath.Lerp(_currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);
                
                float diff = _lastFocus - _currentFocus;
                _lastFocus = _currentFocus;
                
                foreach (var t in _cards)
                    t.Move(new Vector2(0, BeatmapCard.TotalHeight * diff));
            }

            // Go back if the back button was clicked.
            if (MouseManager.IsUniqueClick(MouseButton.Left) && _buttonBack.Hovered(MouseManager.CurrentState.Position))
                _buttonBack.OnClick();
        }
    }
}
