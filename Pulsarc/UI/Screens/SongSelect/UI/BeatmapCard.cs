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
        public static Texture2D DefaultTexture => Skin.Assets["beatmap_card"];

        public Beatmap Beatmap { get; set; }

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
        public BeatmapCard(Beatmap beatmap, Vector2 position, Anchor anchor = Anchor.CenterRight) : base(DefaultTexture, position, anchor)
        {
            // set clicked distance and direction
            clickedDistance = Skin.GetConfigFloat(config, "Properties", "BeatmapCardSelectOffset");
            clickedDirection = Skin.GetConfigString(config, "Properties", "BeatmapCardSelectDirection");

            // set beatmap
            Beatmap = beatmap;

            // set diffbar
            SetDiffBar();

            // set metadata
            SetMetadata();
        }

        private void SetDiffBar()
        {
            float percent = (float)(Beatmap.Difficulty / 10f);

            Vector2 startPos = Skin.GetConfigStartPosition(config, section, "DiffBarStartPos", this);

            Anchor diffAnchor = GetSkinnableAnchor("DiffBarAnchor");

            diffBar = new BeatmapCardDifficulty
            (
                startPos,
                // diffbar displayed percentage is 0 if less than 0, and 10 if greater than 10
                percent <= 10 ? percent >= 0 ? percent : 0 : 10,
                diffAnchor
            );

            int diffBarXOffset = GetSkinnableInt("DiffBarX");
            int diffBarYOffset = GetSkinnableInt("DiffBarY");
            diffBar.ScaledMove(diffBarXOffset, diffBarYOffset);
        }

        private void SetMetadata()
        {
            AddTextDisplayElement("Title");
            textElements[0].Update(Beatmap.Title);

            AddTextDisplayElement("Artist");
            textElements[1].Update(Beatmap.Artist);

            AddTextDisplayElement("Version");
            textElements[2].Update(Beatmap.Version);

            AddTextDisplayElement("Mapper");
            textElements[3].Update(Beatmap.Mapper);

            AddTextDisplayElement("Difficulty");
            textElements[4].Update(string.Format("{0:0.00}", Beatmap.Difficulty));
        }

        protected override void SetConfigAndSection()
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
        public void AdjustClickDistance()
        {
            // If clicked, smoothly move to the clicked distance
            if (isClicked && currentClickDistance < clickedDistance)
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, clickedDistance, (float)PulsarcTime.DeltaTime / 100f);

            // Else if not clicked and currentClickDistacne is greater than 0, smoothly move to 0
            else if (!isClicked && currentClickDistance > 0)
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, 0, (float)PulsarcTime.DeltaTime / 100f);

            // Else, end the method.
            else
                return;

            float diff = lastClickDistance - currentClickDistance;
            lastClickDistance = currentClickDistance;

            switch (clickedDirection)
            {
                case "Left":
                    Move(new Vector2(diff, 0));
                    break;
                case "Right":
                    Move(new Vector2(-diff, 0));
                    break;
                case "Up":
                    Move(new Vector2(0, diff));
                    break;
                case "Down":
                    Move(new Vector2(0, -diff));
                    break;
                default:
                    goto case "Left";
            }
        }

        /// <summary>
        /// When clicked, start playing the beatmap.
        /// </summary>
        public void OnClick()
        {
            if (isClicked)
            {
                GameplayEngine gameplay = new GameplayEngine();
                ScreenManager.AddScreen(gameplay);
                gameplay.Init(Beatmap);
            }
            else
            {
                string path = Beatmap.GetFullAudioPath();

                if (AudioManager.songPath != path)
                {
                    AudioManager.songPath = Beatmap.GetFullAudioPath();
                    AudioManager.audioRate = Config.GetFloat("Gameplay", "SongRate");
                    AudioManager.StartLazyPlayer();

                    if (Beatmap.PreviewTime != 0)
                        AudioManager.DeltaTime(Beatmap.PreviewTime);

                    Console.WriteLine("Now Playing: " + AudioManager.songPath);
                }
            }
        }

        public void SetClicked(bool set)
        {
            isClicked = set;
        }

        /// <summary>
        /// Move this card and all related drawables by the provided delta.
        /// </summary>
        /// <param name="delta">How much to move from the current position.</param>
        public override void Move(Vector2 delta, bool scaledPositioning = true)
        {
            diffBar.Move(delta, scaledPositioning);
            base.Move(delta, scaledPositioning);
        }

        public override void ScaledMove(Vector2 delta)
        {
            diffBar.ScaledMove(delta);
            base.ScaledMove(delta);
        }
    }
}
