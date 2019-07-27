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
        BeatmapDifficulty difficulty;

        public BeatmapCard(Beatmap beatmap, Vector2 position, Vector2 size) : base(Skin.assets["beatmap_card"], position, size)
        {
            this.beatmap = beatmap;

            title = new BeatmapTitle(new Vector2(position.X + 40, position.Y + 20));
            artist = new BeatmapArtist(new Vector2(position.X + 45, position.Y + 50));
            version = new BeatmapVersion(new Vector2(position.X + 45, position.Y + 80));
            mapper = new BeatmapMapper(new Vector2(position.X + 600, position.Y + 130));
            difficulty = new BeatmapDifficulty(new Vector2(position.X + 45, position.Y + 120));

            title.Update(beatmap.Title);
            artist.Update(beatmap.Artist);
            version.Update(beatmap.Version);
            mapper.Update(beatmap.Mapper);
            difficulty.Update(beatmap.Difficulty);
        }

        public override void Draw()
        {
            base.Draw();

            title.Draw();
            artist.Draw();
            version.Draw();
            mapper.Draw();
            difficulty.Draw();
        }

        public void onClick()
        {
            GameplayEngine gameplay = new GameplayEngine();
            ScreenManager.AddScreen(gameplay);
            gameplay.Init(beatmap);
        }

        public override void move(Vector2 delta)
        {
            title.move(delta);
            artist.move(delta);
            version.move(delta);
            mapper.move(delta);
            difficulty.move(delta);
            base.move(delta);
        }
    }
}
