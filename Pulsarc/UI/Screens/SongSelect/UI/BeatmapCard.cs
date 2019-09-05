﻿using Microsoft.Xna.Framework;
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
        public Beatmap beatmap;
        private bool isclicked = false;

        // The difficulty of the map represented as a bar
        BeatmapCardDifficulty diffBar;

        // Metadata
        BeatmapTitle title;
        BeatmapArtist artist;
        BeatmapVersion version;
        BeatmapMapper mapper;
        BeatmapDifficulty difficulty;

        /// <summary>
        /// A card displayed on the Song Select Screen. When clicked it loads the beatmap associated with this card.
        /// </summary>
        /// <param name="beatmap">The beatmap associated with this card.</param>
        /// <param name="position">The position of the card.</param>
        /// <param name="size">The size of the card.</param>
        public BeatmapCard(Beatmap beatmap, Vector2 position, Vector2 size) : base(Skin.assets["beatmap_card"], position, size)
        {
            this.beatmap = beatmap;

            float percent = (float) (beatmap.Difficulty / 10f);
            diffBar = new BeatmapCardDifficulty(new Vector2(position.X + 265, position.Y + 130), percent <= 10 ? percent >= 0 ? percent : 0 : 10);

            title = new BeatmapTitle(new Vector2(position.X + 30, position.Y + 10), Color.White, 22);
            artist = new BeatmapArtist(new Vector2(position.X + 30, position.Y + 50), Color.White, 22);
            version = new BeatmapVersion(new Vector2(position.X + 30, position.Y + 90), new Color(74, 245, 254), 22);
            mapper = new BeatmapMapper(new Vector2(position.X + 765, position.Y + 50), new Color(74, 245, 254), 22, Anchor.TopRight);
            difficulty = new BeatmapDifficulty(new Vector2(position.X + 30, position.Y + 128), new Color(74, 245, 254), 22);

            title.Update(beatmap.Title);
            artist.Update(beatmap.Artist);
            version.Update(beatmap.Version);
            mapper.Update(beatmap.Mapper);
            difficulty.Update(beatmap.Difficulty);
        }

        public override void Draw()
        {
            base.Draw();
            diffBar.Draw();

            title.Draw();
            artist.Draw();
            version.Draw();
            mapper.Draw();
            difficulty.Draw();
        }

        /// <summary>
        /// When clicked, start playing the beatmap.
        /// </summary>
        public void onClick()
        {
            if (isclicked)
            {
                GameplayEngine gameplay = new GameplayEngine();
                ScreenManager.AddScreen(gameplay);
                gameplay.Init(beatmap);
            }
        }

        public void setClicked(bool set)
        {
            if(isclicked && !set)
            {
                move(new Vector2(45, 0));
                isclicked = set;
            }
            else if (!isclicked && set)
            {
                move(new Vector2(-45, 0));
                isclicked = set;
            }
        }

        /// <summary>
        /// Move this card and all related drawables by the provided delta.
        /// </summary>
        /// <param name="delta">How much to move from the current position.</param>
        public override void move(Vector2 delta)
        {
            title.move(delta);
            artist.move(delta);
            version.move(delta);
            mapper.move(delta);
            difficulty.move(delta);
            diffBar.move(delta);
            base.move(delta);
        }
    }
}
