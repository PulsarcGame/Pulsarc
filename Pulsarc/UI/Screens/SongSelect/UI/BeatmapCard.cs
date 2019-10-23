using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using System;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class BeatmapCard : Card
    {
        public static Texture2D StaticTexture = Skin.assets["beatmap_card"];

        public Beatmap beatmap;

        // How far this card will extend when selected
        private float clickedDistance;
        // What direction this card will extend in
        private string clickedDirection;

        private float currentClickDistance = 0f;
        private float lastClickDistance = 0f;

        // The difficulty of the map represented as a bar
        private BeatmapCardDifficulty diffBar;
        
        /// <summary>
        /// A card displayed on the Song Select Screen. When clicked it loads the beatmap associated with this card.
        /// TODO: Cleanup
        /// </summary>
        /// <param name="beatmap">The beatmap associated with this card.</param>
        /// <param name="truePosition">The position of the card.</param>
        public BeatmapCard(Beatmap beatmap, Vector2 position, Anchor anchor = Anchor.CenterRight) : base(StaticTexture, position, anchor)
        {
            // set clicked distance and direction
            clickedDistance = Skin.getConfigFloat(config, "Properties", "BeatmapCardSelectOffset");
            clickedDirection = Skin.getConfigString(config, "Properties", "BeatmapCardSelectDirection");

            // set beatmap
            this.beatmap = beatmap;

            // set diffbar
            float percent = (float) (beatmap.Difficulty / 10f);

            Vector2 startPos = Skin.getConfigStartPosition(config, section, "DiffBarStartPos", this);

            Anchor diffAnchor = getSkinnableAnchor("DiffBarAnchor");

            diffBar = new BeatmapCardDifficulty
            (
                startPos,
                // diffbar displayed percentage is 0 if less than 0, and 10 if greater than 10
                percent <= 10 ? percent >= 0 ? percent : 0 : 10,
                anchor
            );

            int diffBarXOffset = getSkinnableInt("DiffBarX");
            int diffBarYOffset = getSkinnableInt("DiffBarY");
            diffBar.scaledMove(diffBarXOffset, diffBarYOffset);

            // set metadata
            addTextDisplayElement("Title");
            textElements[0].Update(beatmap.Title);

            addTextDisplayElement("Artist");
            textElements[1].Update(beatmap.Artist);

            addTextDisplayElement("Version");
            textElements[2].Update(beatmap.Version);

            addTextDisplayElement("Mapper");
            textElements[3].Update(beatmap.Mapper);

            addTextDisplayElement("Difficulty");
            textElements[4].Update(string.Format("{0:0.00}", beatmap.Difficulty));
        }

        protected override void setConfigAndSection()
        {
            config = "song_select";
            section = "Metadata";
        }

        public override void Draw()
        {
            base.Draw();
            diffBar.Draw();
        }

        /// <summary>
        /// The card moving in and out depending on its selected state.
        /// </summary>
        public void adjustClickDistance()
        {
            // If clicked, smoothly move to the clicked distance
            if (isClicked && currentClickDistance < clickedDistance)
            {
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, clickedDistance, (float)PulsarcTime.DeltaTime / 100f);
            }
            // Else if not clicked and currentClickDistacne is greater than 0, smoothly move to 0
            else if (!isClicked && currentClickDistance > 0)
            {
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, 0, (float)PulsarcTime.DeltaTime / 100f);
            }
            // Else, end the method.
            else
            {
                return;
            }

            float diff = lastClickDistance - currentClickDistance;
            lastClickDistance = currentClickDistance;

            switch (clickedDirection)
            {
                case "Left":
                    scaledMove(new Vector2(diff, 0));
                    break;
                case "Right":
                    scaledMove(new Vector2(-diff, 0));
                    break;
                case "Up":
                    scaledMove(new Vector2(0, diff));
                    break;
                case "Down":
                    scaledMove(new Vector2(0, -diff));
                    break;
                default:
                    goto case "Left";
            }
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
            diffBar.move(delta, scaledPositioning);
            base.move(delta, scaledPositioning);
        }

        public override void scaledMove(Vector2 delta)
        {
            diffBar.scaledMove(delta);
            base.scaledMove(delta);
        }
    }
}
