using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    class SongSelectionView : ScreenView
    {
        SongSelection GetSongSelection() { return (SongSelection)Screen; }

        // SearchBox
        public SearchBox searchBox;

        // Background image of Song Select
        public static Background DefaultBackground => new Background("select_background");

        // Current Scores to display
        List<ScoreCard> scores;

        // Back button to the Main Menu
        ReturnButton button_back;

        // Stats for cards and focus
        // Focus
        float currentFocus, lastFocus;
        // Beatmap Card stats
        float beatmapCardWidth, beatapCardHeight, beatmapCardMargin, beatmapCardTotalHeight;
        // Score Card stats
        float scoreCardWidth, scoreCardHeight, scoreCardMargin, scoreCardTotalHeight;
        
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

            CurrentBackground = DefaultBackground;
            OldBackground = DefaultBackground;

            // Beatmap Card stats
            beatmapCardWidth = BeatmapCard.StaticTexture.Width;
            beatapCardHeight = BeatmapCard.StaticTexture.Height;

            beatmapCardMargin = getSkinnablePropertyInt("BeatmapCardMargin");
            beatmapCardTotalHeight = beatapCardHeight + beatmapCardMargin;

            // Score Card stats
            scoreCardWidth = ScoreCard.StaticTexture.Width;
            scoreCardHeight = ScoreCard.StaticTexture.Height;

            scoreCardMargin = getSkinnablePropertyFloat("ScoreCardMargin");
            scoreCardTotalHeight = scoreCardHeight + scoreCardMargin;

            // Set up beatmap cards
            int i = 0;
            Vector2 beatmapCardStartPosition = Skin.getConfigStartPosition("song_select", "Properties", "BeatmapCardStartPos");

            int offsetX = getSkinnablePropertyInt("BeatmapCardX");
            int offsetY = getSkinnablePropertyInt("BeatmapCardY");

            foreach (Beatmap beatmap in beatmaps)
            {
                Vector2 position = new Vector2(
                    beatmapCardStartPosition.X,
                    beatmapCardStartPosition.Y + (beatmapCardTotalHeight * Pulsarc.HeightScale * i++)
                );

                Anchor anchor = getSkinnablePropertyAnchor("BeatmapCardAnchor");

                BeatmapCard card = new BeatmapCard(beatmap, position, anchor);
                card.move(offsetX, offsetY);

                GetSongSelection().cards.Add(card);
            }

            // Select a random map by default in the song selection.
            if (GetSongSelection().cards.Count > 0)
            {
                Random rd = new Random();

                GetSongSelection().focusedCard = GetSongSelection().cards[rd.Next(0, GetSongSelection().cards.Count)];
                GetSongSelection().focusedCard.onClick();
                focusCard(GetSongSelection().focusedCard);
            }

            Anchor searchBoxAnchor = getSkinnablePropertyAnchor("SearchBarAnchor");
            Vector2 searchBarStartPosition = Skin.getConfigStartPosition("song_select", "Properties", "SearchBarStartPos");

            searchBox = new SearchBox(search, searchBarStartPosition, searchBoxAnchor);

            int searchBarX = getSkinnablePropertyInt("SearchBarX");
            int searchBarY = getSkinnablePropertyInt("SearchBarY");
            searchBox.move(searchBarX, searchBarY);

            button_back = new ReturnButton("select_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
        }

        /// <summary>
        /// Find a float from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePropertyFloat(string key)
        {
            return Skin.getConfigFloat("song_select", "Properties", key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePropertyInt(string key)
        {
            return Skin.getConfigInt("song_select", "Properties", key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePropertyAnchor(string key)
        {
            return Skin.getConfigAnchor("song_select", "Properties", key);
        }

        /// <summary>
        /// Find a string from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string getSkinnablePropertyString(string key)
        {
            return Skin.getConfigString("song_select", "Properties", key);
        }

        public void focusCard(BeatmapCard card)
        {
            GetSongSelection().selectedFocus = -4f;

            foreach (BeatmapCard s in GetSongSelection().cards)
            {
                GetSongSelection().selectedFocus++;

                if (card == s)
                {
                    // Set to "selected" state
                    card.setClicked(true);

                    // Load Beatmap
                    card.beatmap = BeatmapHelper.Load(card.beatmap.path, card.beatmap.fileName);

                    string backgroundPath = card.beatmap.path + "/" + card.beatmap.Background;
                    Texture2D backgroundTexture = AssetsManager.Load(backgroundPath);

                    startChangingBackground(backgroundTexture);
                    break;
                }
            }
        }

        private void scoreCardStuff(BeatmapCard card)
        {
            scores.Clear();

            // ScoreCard Position Setup
            // Get start position
            Vector2 startPos = Skin.getConfigStartPosition("song_select", "Properties", "ScoreCardStartPos");

            float offsetX = getSkinnablePropertyFloat("ScoreCardX");
            float offsetY = getSkinnablePropertyFloat("ScoreCardY");

            int rank = 0;

            // Make a ScoreCard for each score.
            foreach (ScoreData score in card.beatmap.getLocalScores())
            {
                Vector2 position = new Vector2(startPos.X, startPos.Y + (scoreCardTotalHeight * Pulsarc.HeightScale * rank++));

                Anchor anchor = getSkinnablePropertyAnchor("ScoreCardAnchor");

                ScoreCard scoreCard = new ScoreCard(score, position, rank, anchor);
                scoreCard.move(offsetX, offsetY);

                scores.Add(new ScoreCard(score, position, rank));
            }
        }
                
        private void startChangingBackground(Texture2D newBackground)
        {
            if (newBackground != null)
            {
                ChangingBackground = true;
                OldBackground.changeBackground(CurrentBackground.Texture);
                CurrentBackground.changeBackground(newBackground);
                CurrentBackground.opacity = 0;
            }
            else
            {
                if (CurrentBackground.Texture != DefaultBackground.Texture)
                {
                    ChangingBackground = true;
                    OldBackground = CurrentBackground;
                    CurrentBackground = DefaultBackground;
                    CurrentBackground.opacity = 0;
                }
            }
        }

        public override void Destroy()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            drawBackgrounds();

            foreach (BeatmapCard card in GetSongSelection().cards)
            {
                card.adjustClickDistance();

                if (card.onScreen())
                {
                    card.Draw();
                }
            }
            foreach(ScoreCard card in scores)
            {
                card.Draw();
            }
            button_back.Draw();
            searchBox.Draw();
        }

        private void drawBackgrounds()
        {
            if (ChangingBackground && CurrentBackground.opacity < 1)
            {
                OldBackground.Draw();
                CurrentBackground.opacity += (float)PulsarcTime.DeltaTime / BackgroundFadeTime;

                if (CurrentBackground.opacity > 1)
                {
                    CurrentBackground.opacity = 1;
                    ChangingBackground = false;
                }
            }

            CurrentBackground.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            float selectedFocus = GetSongSelection().selectedFocus;

            // Move cards if focus has changed.
            if(currentFocus != selectedFocus)
            {
                currentFocus = PulsarcMath.Lerp(currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);

                float diff = lastFocus - currentFocus;
                lastFocus = currentFocus;

                foreach(BeatmapCard card in GetSongSelection().cards)
                {
                    card.move(new Vector2(0, beatmapCardTotalHeight * diff));
                }
            }

            // Go back if the back button was clicked.
            if (MouseManager.IsUniqueClick(MouseButton.Left))
            {
                if (button_back.clicked(MouseManager.CurrentState.Position))
                {
                    button_back.onClick();
                }
            }
        }
    }
}
