using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapCard : Drawable
    {
        public Beatmap beatmap;
        private bool isClicked = false;
        private static float clickedDistance = 45f;
        private float currentClickDistance = 0f;
        private float lastClickDistance = 0f;

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
            adjustClickDistance();

            base.Draw();
            diffBar.Draw();

            title.Draw();
            artist.Draw();
            version.Draw();
            mapper.Draw();
            difficulty.Draw();
        }

        private void adjustClickDistance()
        {
            // If clicked, smoothly move to the clicked distance
            if (isClicked && currentClickDistance <= clickedDistance)
            {
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, clickedDistance, (float)PulsarcTime.smoothDeltaTime / 100f);
            }
            // Else if not clicked and currentClickDistacne is greater than 0, smoothly move to 0
            else if (!isClicked && currentClickDistance >= 0)
            {
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, 0, (float)PulsarcTime.smoothDeltaTime / 100f);
            }

            float diff = lastClickDistance - currentClickDistance;
            lastClickDistance = currentClickDistance;

            move(new Vector2(diff, 0));
        }

        /// <summary>
        /// When clicked, start playing the beatmap.
        /// </summary>
        public void onClick()
        {
            if (isClicked)
            {
                GameplayEngine gameplay = new GameplayEngine();
                ScreenManager.AddScreen(gameplay);
                gameplay.Init(beatmap);
            } else
            {
                AudioManager.song_path = beatmap.getFullAudioPath();
                AudioManager.StartLazyPlayer();
                Console.WriteLine("Now Playing: " + AudioManager.song_path);
            }
        }

        public void setClicked(bool set)
        {
            isClicked = set;
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
