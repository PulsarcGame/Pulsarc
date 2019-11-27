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
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    class SongSelectionView : ScreenView
    {
        SongSelection GetSongSelection() { return (SongSelection)Screen; }

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

        // Stats for cards and focus
        // Focus
        private float currentFocus, lastFocus;
        // Beatmap Card stats
        private float beatmapCardWidth, beatapCardHeight, beatmapCardMargin, beatmapCardTotalHeight;
        // Score Card stats
        private float scoreCardWidth, scoreCardHeight, scoreCardMargin, scoreCardTotalHeight;
        
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

            // Beatmap Card stats
            SetBeatmapCardStats();

            // Score Card stats
            SetScoreCardStats();

            // Set up beatmap cards
            CreateBeatmapCards(beatmaps);

            // Select a random map by default in the song selection.
            if (cards.Count > 0)
            {
                Random rd = new Random();

                songSelectScreen.FocusedCard = cards[rd.Next(0, cards.Count)];
                songSelectScreen.FocusedCard.OnClick();
                FocusCard(songSelectScreen.FocusedCard);
            }

            Anchor searchBoxAnchor = getSkinnablePropertyAnchor("SearchBarAnchor");
            Vector2 searchBarStartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "SearchBarStartPos");

            SearchBox = new SearchBox(search, searchBarStartPosition, searchBoxAnchor);

            int searchBarX = getSkinnablePropertyInt("SearchBarX");
            int searchBarY = getSkinnablePropertyInt("SearchBarY");
            SearchBox.Move(searchBarX, searchBarY);

            button_back = new ReturnButton("select_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
        }

        #region Init Methods
        /// <summary>
        /// Initialize required Beatmap Card variables.
        /// </summary>
        private void SetBeatmapCardStats()
        {
            beatmapCardWidth = BeatmapCard.DefaultTexture.Width;
            beatapCardHeight = BeatmapCard.DefaultTexture.Height;

            beatmapCardMargin = getSkinnablePropertyInt("BeatmapCardMargin");
            beatmapCardTotalHeight = beatapCardHeight + beatmapCardMargin;
        }

        /// <summary>
        /// Initialize required Score Card variables
        /// </summary>
        private void SetScoreCardStats()
        {
            scoreCardWidth = ScoreCard.DefaultTexture.Width;
            scoreCardHeight = ScoreCard.DefaultTexture.Height;

            scoreCardMargin = getSkinnablePropertyFloat("ScoreCardMargin");
            scoreCardTotalHeight = scoreCardHeight + scoreCardMargin;
        }

        /// <summary>
        /// Create each beatmapcard out of the List of Beatmaps
        /// provided
        /// </summary>
        /// <param name="beatmaps"></param>
        private void CreateBeatmapCards(List<Beatmap> beatmaps)
        {
            Vector2 beatmapCardStartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "BeatmapCardStartPos");

            int offsetX = getSkinnablePropertyInt("BeatmapCardX");
            int offsetY = getSkinnablePropertyInt("BeatmapCardY");

            for (int i = 0; i < beatmaps.Count; i++)
            {
                Vector2 position = new Vector2
                (
                    beatmapCardStartPosition.X,
                    beatmapCardStartPosition.Y + (beatmapCardTotalHeight * Pulsarc.HeightScale * i)
                );

                Anchor anchor = getSkinnablePropertyAnchor("BeatmapCardAnchor");

                BeatmapCard card = new BeatmapCard(beatmaps[i], position, anchor);
                card.Move(offsetX, offsetY);

                cards.Add(card);
            }
        }
        #endregion

        #region GetSkinnable Methods
        /// <summary>
        /// Find a float from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePropertyFloat(string key)
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
        private string getSkinnablePropertyString(string key)
        {
            return Skin.GetConfigString("song_select", "Properties", key);
        }
        #endregion

        /// <summary>
        /// Select a card and change the focus of
        /// the Song Select screen accordingly.
        /// </summary>
        /// <param name="card">The card to focus on.</param>
        public void FocusCard(BeatmapCard card)
        {
            songSelectScreen.SelectedFocus = -4f;

            for (int i = 0; i < cards.Count; i++)
            {
                songSelectScreen.SelectedFocus++;

                if (card == cards[i])
                {
                    // Set to "selected" state
                    card.SetClicked(true);

                    // Load Beatmap
                    card.Beatmap = BeatmapHelper.Load(card.Beatmap.Path, card.Beatmap.FileName);

                    string backgroundPath = card.Beatmap.Path + "/" + card.Beatmap.Background;
                    Texture2D backgroundTexture = AssetsManager.Load(backgroundPath);

                    StartChangingBackground(backgroundTexture);

                    UpdateScoreCard(card);
                    break;
                }
            }
        }

        /// <summary>
        /// Finish the selecting process for the card we're
        /// focusing on.
        /// </summary>
        /// <param name="card">The card we're focusing on.</param>
        private void UpdateScoreCard(BeatmapCard card)
        {
            scores.Clear();

            // ScoreCard Position Setup
            // Get start position
            Vector2 startPos = Skin.GetConfigStartPosition("song_select", "Properties", "ScoreCardStartPos");

            float offsetX = getSkinnablePropertyFloat("ScoreCardX");
            float offsetY = getSkinnablePropertyFloat("ScoreCardY");

            int rank = 0;

            // Make a ScoreCard for each score.
            List<ScoreData> scoress = card.Beatmap.GetLocalScores();
            for (int i = 0; i < scoress.Count; i++)
            {
                Vector2 position = new Vector2(startPos.X, startPos.Y + (scoreCardTotalHeight * Pulsarc.HeightScale * rank++));

                Anchor anchor = getSkinnablePropertyAnchor("ScoreCardAnchor");

                ScoreCard scoreCard = new ScoreCard(scoress[i], position, rank, anchor);
                scoreCard.Move(offsetX, offsetY);

                scores.Add(scoreCard);
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
            else
            {
                if (CurrentBackground.Texture != DefaultBackground.Texture)
                {
                    ChangingBackground = true;
                    OldBackground = CurrentBackground;
                    CurrentBackground = DefaultBackground;
                    CurrentBackground.Opacity = 0;
                }
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

            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].AdjustClickDistance();

                if (cards[i].OnScreen())
                    cards[i].Draw();
            }

            for (int i = 0; i < scores.Count; i++)
                scores[i].Draw();

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
            float selectedFocus = songSelectScreen.SelectedFocus;

            // Move cards if focus has changed.
            if (currentFocus != selectedFocus)
            {
                currentFocus = PulsarcMath.Lerp(currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);

                float diff = lastFocus - currentFocus;
                lastFocus = currentFocus;

                for (int i = 0; i < cards.Count; i++)
                    cards[i].Move(new Vector2(0, beatmapCardTotalHeight * diff));
            }

            // Go back if the back button was clicked.
            if (MouseManager.IsUniqueClick(MouseButton.Left) && button_back.Hovered(MouseManager.CurrentState.Position))
                button_back.OnClick();
        }
    }
}
