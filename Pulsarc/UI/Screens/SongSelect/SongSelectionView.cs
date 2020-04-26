using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using Pulsarc.Utils.SQLite;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    public class SongSelectionView : ScreenView
    {
        SongSelection GetSongSelection() => (SongSelection)Screen;

        SongSelection songSelectScreen;

        // SearchBox
        public SearchBox SearchBox { get; private set; }

        // Background image of Song Select
        public static Background DefaultBackground => new Background("select_background");

        // Current Scores to display
        private List<ScoreCard> scores;

        private List<BeatmapCard> cards;

        // Back button to the Main Menu
        private ReturnButton button_back;

        // Focus (used for scrolling through the menu)
        private float currentFocus, lastFocus;
        
        // Background changing stuff.
        public bool ChangingBackground { get; private set; } = false;
        public Background CurrentBackground { get; private set; }
        private Background OldBackground;
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
            songSelectScreen = GetSongSelection();

            cards = songSelectScreen.Cards;
            scores = new List<ScoreCard>();

            // Prepare backgrounds
            CurrentBackground = DefaultBackground;
            OldBackground = DefaultBackground;

            // Set up beatmap cards
            for (int i = 0; i < beatmaps.Count; i++)
            {
                cards.Add(new BeatmapCard(beatmaps[i], i));
            }

            Anchor searchBoxAnchor = GetSkinnablePropertyAnchor("SearchBarAnchor");
            Vector2 searchBarStartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "SearchBarStartPos");

            SearchBox = new SearchBox(search, searchBarStartPosition, searchBoxAnchor);

            int searchBarX = GetSkinnablePropertyInt("SearchBarX");
            int searchBarY = GetSkinnablePropertyInt("SearchBarY");
            SearchBox.Move(searchBarX, searchBarY);

            button_back = new ReturnButton("select_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
        }

        #region GetSkinnable Methods
        /// <summary>
        /// Find a float from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float GetSkinnablePropertyFloat(string key) => Skin.GetConfigFloat("song_select", "Properties", key);

        /// <summary>
        /// Find a int from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int GetSkinnablePropertyInt(string key) => Skin.GetConfigInt("song_select", "Properties", key);

        /// <summary>
        /// Find an Anchor from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor GetSkinnablePropertyAnchor(string key) => Skin.GetConfigAnchor("song_select", "Properties", key);

        /// <summary>
        /// Find a string from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string GetSkinnablePropertyString(string key) => Skin.GetConfigString("song_select", "Properties", key);
        #endregion

        /// <summary>
        /// Select a card and change the focus of
        /// the Song Select screen accordingly.
        /// </summary>
        /// <param name="card">The card to focus on.</param>
        public void FocusCard(in BeatmapCard card)
        {
            if (cards[card.Index] != card) { return; }

            // Reset focus for consistency
            songSelectScreen.SelectedFocus = card.Index - 3;

            // Set to "selected" state
            card.SetClicked(true);

            // Load Beatmap
            card.Beatmap = BeatmapHelper.Load(card.Beatmap.Path, card.Beatmap.FileName);

            string backgroundPath = card.Beatmap.Path + "/" + card.Beatmap.Background;
            Texture2D backgroundTexture = AssetsManager.Load(backgroundPath);

            StartChangingBackground(backgroundTexture);

            UpdateScoreCard(card);
        }

        public void RefocusCurrentCard() => UpdateScoreCard(GetSongSelection().FocusedCard);

        /// <summary>
        /// Finish the selecting process for the card we're
        /// focusing on.
        /// </summary>
        /// <param name="card">The card we're focusing on.</param>
        private void UpdateScoreCard(in BeatmapCard card)
        {
            if (card == null) { return; }

            scores.Clear();

            // Make a ScoreCard for each score.
            List<ScoreData> scoresInMap = card.Beatmap.GetLocalScores();

            for (int i = 0; i < scoresInMap.Count; i++)
            {
                scores.Add(new ScoreCard(scoresInMap[i], i + 1));
            }
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
                OldBackground.ChangeBackground(CurrentBackground.Texture);
                CurrentBackground.ChangeBackground(newBackground);
                CurrentBackground.Opacity = 0;
            }
            else if (CurrentBackground.Texture != DefaultBackground.Texture)
            {
                ChangingBackground = true;
                OldBackground = CurrentBackground;
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

            // Keep things in one for-loop for optimization
            for (int i = 0; i < Math.Max(cards.Count, scores.Count); i++)
            {
                if (i < cards.Count)
                {
                    cards[i].AdjustClickDistance();
                    cards[i].Draw();
                }

                if (i < scores.Count)
                {
                    scores[i].Draw();
                }
            }

            button_back.Draw();
            SearchBox.Draw();
        }

        /// <summary>
        /// Draw any backgrounds on screen and calculate opacity changes.
        /// </summary>
        private void DrawBackgrounds()
        {
            if (ChangingBackground && CurrentBackground.Opacity < 1)
            {
                OldBackground.Draw();
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
            // Move cards if focus has changed.
            // Rounding is used so this isn't called over and over
            // When currentFocus refuses to go above x.99999999...
            if (Math.Round(currentFocus, 2) != Math.Round(songSelectScreen.SelectedFocus, 2))
            {
                currentFocus = PulsarcMath.Lerp(
                    currentFocus, songSelectScreen.SelectedFocus,
                    (float)PulsarcTime.DeltaTime / 100f);
                
                float diff = lastFocus - currentFocus;
                lastFocus = currentFocus;

                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].Move(new Vector2(0, BeatmapCard.TotalHeight * diff));
                }
            }

            // Go back if the back button was clicked.
            if (InputManager.IsLeftClick()
                && button_back.Hovered(InputManager.LastMouseClick.Key.Position))
            {
                button_back.OnClick();
            }
        }
    }
}
