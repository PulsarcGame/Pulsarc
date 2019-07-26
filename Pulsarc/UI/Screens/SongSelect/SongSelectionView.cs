using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    class SongSelectionView : ScreenView
    {
        SongSelection GetSongSelection() { return (SongSelection)Screen; }


        int cardWidth = 800;
        int cardHeight = 250;
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
        }

        public override void Destroy()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(BeatmapCard card in GetSongSelection().cards)
            {
                card.Draw();
            }
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
        }
    }
}
