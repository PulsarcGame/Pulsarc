using System;
using System.Collections.Generic;
using System.Text;
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
using Wobble.Logging;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    class SongSelectionView : ScreenView
    {
        SongSelection GetSongSelection() { return (SongSelection)Screen; }

        // SearchBox
        public SearchBox searchBox;

        // Background image of Song Select
        Background background;

        // Current Scores to display
        List<ScoreCard> scores;

        // Back button to the Main Menu
        ReturnButton button_back;

        // Card stats
        // TODO: Make this more skinnable/adjustable by the user.
        float cardWidth, cardHeight, cardMargin, totalHeight, currentFocus, lastFocus;

        public SongSelectionView(Screen screen, List<Beatmap> beatmaps, string search = "") : base(screen)
        {
            scores = new List<ScoreCard>();

            Texture2D beatmapCardTex = Skin.assets["beatmap_card"];
            cardWidth = beatmapCardTex.Width - 10;
            cardHeight = beatmapCardTex.Height - 10;
            cardMargin = getSkinnablePropertyInt("BeatmapCardMargin");
            totalHeight = cardHeight + cardMargin;

            int i = 0;
            foreach (Beatmap beatmap in beatmaps)
            {
                Vector2 position = new Vector2(Pulsarc.CurrentWidth, totalHeight * Pulsarc.HeightScale * i);
                Vector2 size = new Vector2(cardWidth, cardHeight);

                GetSongSelection().cards.Add(new BeatmapCard(beatmap, position, size));
                i++;
            }

            // Select a random map by default in the song selection.
            if (GetSongSelection().cards.Count > 0)
            {
                Random rd = new Random();

                GetSongSelection().focusedCard = GetSongSelection().cards[rd.Next(0, GetSongSelection().cards.Count)];
                GetSongSelection().focusedCard.onClick();
                focusCard(GetSongSelection().focusedCard);
            }

            searchBox = new SearchBox(search, AnchorUtil.FindScreenPosition(Anchor.TopRight), Anchor.TopRight);

            background = new Background("select_background");

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
                    card.setClicked(true);

                    card.beatmap = BeatmapHelper.Load(card.beatmap.path, card.beatmap.fileName);

                    scores.Clear();
                    Vector2 currentPos = new Vector2(0, 20);
                    int rank = 1;

                    foreach (ScoreData score in card.beatmap.getLocalScores())
                    {
                        scores.Add(new ScoreCard(score, currentPos, rank));
                        currentPos.Y += 150;
                        rank++;
                    }
                    break;
                }
            }
        }

        public override void Destroy()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw();
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
                    card.move(new Vector2(0, totalHeight * diff));
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
