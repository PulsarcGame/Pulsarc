using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.SongSelect.UI;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    class SongSelectionView : ScreenView
    {
        SongSelection GetSongSelection() { return (SongSelection)Screen; }

        Background background;

        ReturnButton button_back;

        int cardWidth = 800;
        int cardHeight = 170;
        int cardMargin = 10;
        int lastFocus = 0;

        public SongSelectionView(Screen screen, List<Beatmap> beatmaps) : base(screen)
        {

            int i = 0;
            foreach(Beatmap beatmap in beatmaps)
            {
                GetSongSelection().cards.Add(new BeatmapCard(beatmap, new Vector2(1920 - cardWidth,(cardHeight + cardMargin) * i), new Vector2(cardWidth, cardHeight)));
                i++;
            }

            background = new Background("select_background");

            button_back = new ReturnButton("select_button_back", new Vector2(0, 1080));
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
            button_back.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            if(lastFocus != GetSongSelection().currentFocus)
            {
                int diff = GetSongSelection().currentFocus - lastFocus;

                foreach(BeatmapCard card in GetSongSelection().cards)
                {
                    card.move(new Vector2(0, (cardHeight + cardMargin) *diff / 2.5f));
                }

                lastFocus = GetSongSelection().currentFocus;
            }

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
