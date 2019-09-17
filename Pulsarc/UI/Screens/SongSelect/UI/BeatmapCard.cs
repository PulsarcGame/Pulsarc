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
    public class BeatmapCard : Drawable
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
        /// <param name="truePosition">The position of the card.</param>
        /// <param name="size">The size of the card.</param>
        public BeatmapCard(Beatmap beatmap, Vector2 position, Vector2 size) : base(Skin.assets["beatmap_card"], position, size, anchor: Anchor.CenterRight)
        {
            this.beatmap = beatmap;

            float percent = (float) (beatmap.Difficulty / 10f);
            diffBar = new BeatmapCardDifficulty(new Vector2(AnchorUtil.FindScreenPosition(Anchor.CenterRight).X, truePosition.Y), percent <= 10 ? percent >= 0 ? percent : 0 : 10);
            diffBar.scaledMove(-10, 165); // TODO: Make these values customizeable for custom skins.

            title = new BeatmapTitle(truePosition, Color.White, 22);
            title.scaledMove(20, 10);

            artist = new BeatmapArtist(truePosition, Color.White, 22);
            artist.scaledMove(20, 50);

            version = new BeatmapVersion(truePosition, new Color(74, 245, 254), 22);
            version.scaledMove(20, 90);
            
            mapper = new BeatmapMapper(truePosition, new Color(74, 245, 254), 22, Anchor.TopRight);
            mapper.scaledMove(700, 50); // TODO: Fix this mother fucker. Doesn't like to scale with everything else.

            difficulty = new BeatmapDifficulty(truePosition, new Color(74, 245, 254), 22);
            difficulty.scaledMove(20, 128);

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
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, clickedDistance, (float)PulsarcTime.DeltaTime / 100f);
            }
            // Else if not clicked and currentClickDistacne is greater than 0, smoothly move to 0
            else if (!isClicked && currentClickDistance >= 0)
            {
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, 0, (float)PulsarcTime.DeltaTime / 100f);
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
                string path = beatmap.getFullAudioPath();
                if (AudioManager.song_path != path)
                {
                    AudioManager.song_path = beatmap.getFullAudioPath();
                    AudioManager.audioRate = Config.getFloat("Gameplay", "SongRate");
                    AudioManager.StartLazyPlayer();

                    if(beatmap.PreviewTime != 0)
                    {
                        AudioManager.deltaTime(beatmap.PreviewTime);
                    }

                    Console.WriteLine("Now Playing: " + AudioManager.song_path);
                }
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
        public override void move(Vector2 delta, bool scaledPositioning = true)
        {
            title.move(delta, scaledPositioning);
            artist.move(delta, scaledPositioning);
            version.move(delta, scaledPositioning);
            mapper.move(delta, scaledPositioning);
            difficulty.move(delta, scaledPositioning);
            diffBar.move(delta, scaledPositioning);
            base.move(delta, scaledPositioning);
        }
    }
}
