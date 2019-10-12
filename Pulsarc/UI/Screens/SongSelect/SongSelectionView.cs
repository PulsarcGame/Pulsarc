using System;
using System.Collections.Generic;
using System.IO;
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
        float currentFocus = 0;
        float lastFocus = 0;

        public SongSelectionView(Screen screen, List<Beatmap> beatmaps, string search = "") : base(screen)
        {
            scores = new List<ScoreCard>();

            searchBox = new SearchBox(search, new Vector2(1920, 0), Anchor.TopRight);

            background = new Background("select_background");

            button_back = new ReturnButton("select_button_back", new Vector2(0, 1080));

            int i = 0;
            foreach(Beatmap beatmap in beatmaps)
            {
                GetSongSelection().cards.Add(new BeatmapCard(beatmap, new Vector2(1920 - cardWidth,(cardHeight + cardMargin) * i), new Vector2(cardWidth, cardHeight)));
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
        }

        public void focusCard(BeatmapCard card)
        {
            float step = (cardHeight + cardMargin) / 100;
            GetSongSelection().selectedFocus = -3.5f;

            foreach (BeatmapCard s in GetSongSelection().cards)
            {
                GetSongSelection().selectedFocus += step;

                if(card == s)
                {
                    card.setClicked(true);
                    scores.Clear();
                    Vector2 currentPos = new Vector2(0, 20);
                    int rank = 1;
                    card.beatmap = BeatmapHelper.Load(card.beatmap.path, card.beatmap.fileName);
                    foreach(ScoreData score in card.beatmap.getLocalScores())
                    {
                        scores.Add(new ScoreCard(score, currentPos, rank));
                        currentPos.Y += 150;
                        rank++;
                    }

                    string backgroundPath = card.beatmap.path + "/" + card.beatmap.Background;
                    Texture2D backgroundTexture = AssetsManager.Load(backgroundPath);

                    if (backgroundTexture != null)
                    {
                        background.changeBackground(backgroundTexture);
                    } else
                    {
                        if(background.Texture != Skin.assets["select_background"])
                        {
                            background.changeBackground(Skin.assets["select_background"]);
                        }
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
            float selectedFocus = GetSongSelection().selectedFocus;

            // Move cards if focus has changed.
            if(currentFocus != selectedFocus)
            {
                currentFocus = PulsarcMath.Lerp(currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);

                float diff = lastFocus - currentFocus;
                lastFocus = currentFocus;

                foreach(BeatmapCard card in GetSongSelection().cards)
                {
                    card.move(new Vector2(0, (cardHeight + cardMargin) * diff));
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
