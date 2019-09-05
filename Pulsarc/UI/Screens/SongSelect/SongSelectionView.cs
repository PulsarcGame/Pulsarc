using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.SongSelect.UI;
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
        Background background;

        // Current Scores to display
        List<ScoreCard> scores;

        // Back button to the Main Menu
        ReturnButton button_back;

        // Card stats
        // TODO: Make this more skinnable/adjustable by the user.
        int cardWidth = 800;
        int cardHeight = 170;
        int cardMargin = 10;
        float lastFocus = 0;

        public SongSelectionView(Screen screen, List<Beatmap> beatmaps, string search = "") : base(screen)
        {
            scores = new List<ScoreCard>();
            int i = 0;
            foreach(Beatmap beatmap in beatmaps)
            {
                GetSongSelection().cards.Add(new BeatmapCard(beatmap, new Vector2(1920 - cardWidth,(cardHeight + cardMargin) * i), new Vector2(cardWidth, cardHeight)));
                i++;
            }
            // Select a map by default in the song selection.
            // TODO: Select a map that was randomly playing in the background on the previous menu
            if (GetSongSelection().cards.Count > 0)
            {
                GetSongSelection().focusedCard = GetSongSelection().cards[0];
                focusCard(GetSongSelection().focusedCard);
            }

            searchBox = new SearchBox(search, new Vector2(1920, 0), Anchor.TopRight);

            background = new Background("select_background");

            button_back = new ReturnButton("select_button_back", new Vector2(0, 1080));
        }

        public void focusCard(BeatmapCard card)
        {
            GetSongSelection().currentFocus = 3;
            foreach(BeatmapCard s in GetSongSelection().cards)
            {
                GetSongSelection().currentFocus--;
                if(card == s)
                {
                    card.setClicked(true);
                    scores.Clear();
                    Vector2 currentPos = new Vector2(0, 20);
                    int rank = 1;
                    if (!card.beatmap.fullyLoaded) card.beatmap = BeatmapHelper.Load(card.beatmap.path, card.beatmap.fileName);
                    foreach(ScoreData score in card.beatmap.getLocalScores())
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
            // Move cards if focus has changed.
            if(lastFocus != GetSongSelection().currentFocus)
            {
                float diff = GetSongSelection().currentFocus - lastFocus;

                foreach(BeatmapCard card in GetSongSelection().cards)
                {
                    card.move(new Vector2(0, (cardHeight + cardMargin) *diff));
                }

                lastFocus = GetSongSelection().currentFocus;
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
