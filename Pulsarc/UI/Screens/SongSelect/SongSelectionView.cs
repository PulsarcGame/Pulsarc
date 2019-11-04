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

        // SearchBox
        public SearchBox SearchBox { get; private set; }

        // Background image of Song Select
        public static Background DefaultBackground => new Background("select_background");

        // Current Scores to display
        private List<ScoreCard> scores;

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
            scores = new List<ScoreCard>();

            // Prepare backgrounds
            CurrentBackground = DefaultBackground;
            OldBackground = DefaultBackground;

            // Beatmap Card stats
            setBeatmapCardStats();

            // Score Card statss
            setScoreCardStats();

            // Set up beatmap cards
            createBeatmapCards(beatmaps);

            // Select a random map by default in the song selection.
            if (GetSongSelection().Cards.Count > 0)
            {
                Random rd = new Random();

                GetSongSelection().FocusedCard = GetSongSelection().Cards[rd.Next(0, GetSongSelection().Cards.Count)];
                GetSongSelection().FocusedCard.OnClick();
                FocusCard(GetSongSelection().FocusedCard);
            }

            Anchor searchBoxAnchor = getSkinnablePropertyAnchor("SearchBarAnchor");
            Vector2 searchBarStartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "SearchBarStartPos");

            SearchBox = new SearchBox(search, searchBarStartPosition, searchBoxAnchor);

            int searchBarX = getSkinnablePropertyInt("SearchBarX");
            int searchBarY = getSkinnablePropertyInt("SearchBarY");
            SearchBox.Move(searchBarX, searchBarY);

            button_back = new ReturnButton("select_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
        }

        private void setBeatmapCardStats()
        {
            beatmapCardWidth = BeatmapCard.DefaultTexture.Width;
            beatapCardHeight = BeatmapCard.DefaultTexture.Height;

            beatmapCardMargin = getSkinnablePropertyInt("BeatmapCardMargin");
            beatmapCardTotalHeight = beatapCardHeight + beatmapCardMargin;
        }

        private void setScoreCardStats()
        {
            scoreCardWidth = ScoreCard.DefaultTexture.Width;
            scoreCardHeight = ScoreCard.DefaultTexture.Height;

            scoreCardMargin = getSkinnablePropertyFloat("ScoreCardMargin");
            scoreCardTotalHeight = scoreCardHeight + scoreCardMargin;
        }

        private void createBeatmapCards(List<Beatmap> beatmaps)
        {
            int i = 0;
            Vector2 beatmapCardStartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "BeatmapCardStartPos");

            int offsetX = getSkinnablePropertyInt("BeatmapCardX");
            int offsetY = getSkinnablePropertyInt("BeatmapCardY");

            foreach (Beatmap beatmap in beatmaps)
            {
                Vector2 position = new Vector2
                (
                    beatmapCardStartPosition.X,
                    beatmapCardStartPosition.Y + (beatmapCardTotalHeight * Pulsarc.HeightScale * i++)
                );

                Anchor anchor = getSkinnablePropertyAnchor("BeatmapCardAnchor");

                BeatmapCard card = new BeatmapCard(beatmap, position, anchor);
                card.Move(offsetX, offsetY);

                GetSongSelection().Cards.Add(card);
            }
        }

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

        public void FocusCard(BeatmapCard card)
        {
            GetSongSelection().SelectedFocus = -4f;

            foreach (BeatmapCard s in GetSongSelection().Cards)
            {
                GetSongSelection().SelectedFocus++;

                if (card == s)
                {
                    // Set to "selected" state
                    card.SetClicked(true);

                    // Load Beatmap
                    card.Beatmap = BeatmapHelper.Load(card.Beatmap.Path, card.Beatmap.FileName);

                    string backgroundPath = card.Beatmap.Path + "/" + card.Beatmap.Background;
                    Texture2D backgroundTexture = AssetsManager.Load(backgroundPath);

                    StartChangingBackground(backgroundTexture);

                    // ScoreCard Stuff
                    ScoreCardStuff(card);
                    break;
                }
            }
        }

        private void ScoreCardStuff(BeatmapCard card)
        {
            scores.Clear();

            // ScoreCard Position Setup
            // Get start position
            Vector2 startPos = Skin.GetConfigStartPosition("song_select", "Properties", "ScoreCardStartPos");

            float offsetX = getSkinnablePropertyFloat("ScoreCardX");
            float offsetY = getSkinnablePropertyFloat("ScoreCardY");

            int rank = 0;

            // Make a ScoreCard for each score.
            foreach (ScoreData score in card.Beatmap.GetLocalScores())
            {
                Vector2 position = new Vector2(startPos.X, startPos.Y + (scoreCardTotalHeight * Pulsarc.HeightScale * rank++));

                Anchor anchor = getSkinnablePropertyAnchor("ScoreCardAnchor");

                ScoreCard scoreCard = new ScoreCard(score, position, rank, anchor);
                scoreCard.Move(offsetX, offsetY);

                scores.Add(new ScoreCard(score, position, rank));
            }
        }
                
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

        public override void Draw(GameTime gameTime)
        {
            DrawBackgrounds();

            foreach (BeatmapCard card in GetSongSelection().Cards)
            {
                card.AdjustClickDistance();

                if (card.OnScreen())
                    card.Draw();
            }

            foreach(ScoreCard card in scores)
                card.Draw();

            button_back.Draw();
            SearchBox.Draw();
        }

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

        public override void Update(GameTime gameTime)
        {
            float selectedFocus = GetSongSelection().SelectedFocus;

            // Move cards if focus has changed.
            if (currentFocus != selectedFocus)
            {
                currentFocus = PulsarcMath.Lerp(currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);

                float diff = lastFocus - currentFocus;
                lastFocus = currentFocus;

                foreach (BeatmapCard card in GetSongSelection().Cards)
                    card.Move(new Vector2(0, beatmapCardTotalHeight * diff));
            }

            // Go back if the back button was clicked.
            if (MouseManager.IsUniqueClick(MouseButton.Left) && button_back.Clicked(MouseManager.CurrentState.Position))
                button_back.OnClick();
        }
    }
}
