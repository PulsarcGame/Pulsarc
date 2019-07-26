using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapCard : Drawable
    {
        Beatmap beatmap;

        BeatmapTitle title;
        BeatmapArtist artist;
        BeatmapVersion version;
        BeatmapMapper mapper;

        public BeatmapCard(Beatmap beatmap, Vector2 position, Vector2 size) : base(Skin.assets["beatmap_card"], position, size)
        {
            this.beatmap = beatmap;

            title = new BeatmapTitle(new Vector2(position.X + 280, position.Y + 30));
            artist = new BeatmapArtist(new Vector2(position.X + 330, position.Y + 85));
            version = new BeatmapVersion(new Vector2(position.X + 270, position.Y + 120));
            mapper = new BeatmapMapper(new Vector2(position.X + 270, position.Y + 200));

            title.Update(beatmap.Title);
            artist.Update(beatmap.Artist);
            version.Update(beatmap.Version);
            mapper.Update(beatmap.Mapper);
        }

        public override void Draw()
        {
            base.Draw();

            title.Draw();
            artist.Draw();
            version.Draw();
            mapper.Draw();
        }

        public void onClick()
        {
            GameplayEngine gameplay = new GameplayEngine();
            ScreenManager.AddScreen(gameplay);
            gameplay.Init(beatmap);
        }
    }
}
