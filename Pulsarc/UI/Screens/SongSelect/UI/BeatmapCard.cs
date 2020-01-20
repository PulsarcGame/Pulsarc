using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using Wobble.Logging;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class BeatmapCard : Card
    {
        public Beatmap Beatmap { get; set; }

        private static Texture2D DefaultTexture => Skin.Assets["beatmap_card"];

        // Stats
        private static readonly Anchor DefaultAnchor = Skin.GetConfigAnchor("song_select", "Properties", "BeatmapCardAnchor");

        private static readonly int OffsetX = Skin.GetConfigInt("song_select", "Properties", "BeatmapCardX");
        private static readonly int OffsetY = Skin.GetConfigInt("song_select", "Properties", "BeatmapCardY");
        private static readonly Vector2 StartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "BeatmapCardStartPos") + new Vector2(OffsetX, OffsetY);

        private static readonly int Margin = Skin.GetConfigInt("song_select", "Properties", "BeatmapCardMargin");
        public static readonly int TotalHeight = DefaultTexture.Height + Margin;
        public static readonly int TotalWidth = DefaultTexture.Width + Margin;

        public int Index { get; private set; }

        private Vector2 PersonalStartPosition => StartPosition + personalStartPosOffset;
        private Vector2 personalStartPosOffset;

        // How far this card will extend when selected
        private readonly float _clickedDistance;
        // What direction this card will extend in
        private readonly string _clickedDirection;

        private float _currentClickDistance;
        private float _lastClickDistance;

        // The difficulty of the map represented as a bar
        private BeatmapCardDifficulty _diffBar;
        private Vector2 _diffBarOffset;
        private Anchor _diffBarStartAnchor;

        /// <summary>
        /// A card displayed on the Song Select Screen. When clicked it loads the beatmap associated with this card.
        /// TODO: Cleanup
        /// </summary>
        /// <param name="beatmap">The beatmap associated with this card.</param>
        /// <param name="index"></param>
        public BeatmapCard(Beatmap beatmap, int index)
            : base(DefaultTexture, StartPosition, DefaultAnchor)
        {
            // set clicked distance and direction
            _clickedDistance = Skin.GetConfigFloat(Config, "Properties", "BeatmapCardSelectOffset");
            _clickedDirection = Skin.GetConfigString(Config, "Properties", "BeatmapCardSelectDirection");

            Index = index;

            personalStartPosOffset = new Vector2(0, TotalHeight * Pulsarc.HeightScale * Index);

            // set beatmap
            Beatmap = beatmap;

            // set diffbar
            SetDiffBar();

            // set metadata
            SetMetadata();

            ChangePosition(PersonalStartPosition);
        }

        public sealed override void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            base.ChangePosition(position, topLeftPositioning);
        }

        private void SetDiffBar()
        {
            float percent = (float)(Beatmap.Difficulty / 10f);

            Anchor startAnchor;
            Vector2 startPos = Skin.GetConfigStartPosition(Config, Section, "DiffBarStartPos", out startAnchor, this);

            Anchor diffAnchor = GetSkinnableAnchor("DiffBarAnchor");

            _diffBar = new BeatmapCardDifficulty
            (
                startPos,
                // diffbar displayed percentage is 0 if less than 0, and 10 if greater than 10
                percent <= 10 ? percent >= 0 ? percent : 0 : 10,
                diffAnchor
            );

            int diffBarXOffset = GetSkinnableInt("DiffBarX");
            int diffBarYOffset = GetSkinnableInt("DiffBarY");
            _diffBar.Move(diffBarXOffset, diffBarYOffset);
            _diffBarOffset = _diffBar.AnchorPosition - AnchorUtil.FindDrawablePosition(startAnchor, this);
            _diffBarStartAnchor = startAnchor;
        }

        private void SetMetadata()
        {
            AddTextDisplayElement("Title");
            TextElements[0].Update(Beatmap.Title);

            AddTextDisplayElement("Artist");
            TextElements[1].Update(Beatmap.Artist);

            AddTextDisplayElement("Version");
            TextElements[2].Update(Beatmap.Version);

            AddTextDisplayElement("Mapper");
            TextElements[3].Update(Beatmap.Mapper);

            AddTextDisplayElement("Difficulty");
            TextElements[4].Update($"{Beatmap.Difficulty:0.00}");
        }

        protected override void SetConfigAndSection()
        {
            Config = "song_select";
            Section = "Metadata";
        }

        public override void Draw()
        {
            if (!OnScreen())
                return;

            base.Draw();
            _diffBar.Draw();
        }

        /// <summary>
        /// The card moving in and out depending on its selected state.
        /// </summary>
        public void AdjustClickDistance()
        {
            // If clicked, smoothly move to the clicked distance
            if (IsClicked && Math.Round(_currentClickDistance, 3) < Math.Round(_clickedDistance, 3))
                _currentClickDistance = PulsarcMath.Lerp(_currentClickDistance, _clickedDistance, (float)PulsarcTime.DeltaTime / 100f);

            // Else if not clicked and currentClickDistacne is greater than 0, smoothly move to 0
            else if (!IsClicked && Math.Round(_currentClickDistance, 3) > 0)
                _currentClickDistance = PulsarcMath.Lerp(_currentClickDistance, 0, (float)PulsarcTime.DeltaTime / 100f);

            // Else, end the method.
            else
                return;

            float diff = _lastClickDistance - _currentClickDistance;
            _lastClickDistance = _currentClickDistance;

            switch (_clickedDirection)
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
            if (IsClicked)
            {
                GameplayEngine gameplay = new GameplayEngine();
                ScreenManager.AddScreen(gameplay);
                gameplay.Init(Beatmap);
            }
            else
            {
                string path = Beatmap.GetFullAudioPath();

                if (AudioManager.SongPath == path) return;
                AudioManager.SongPath = Beatmap.GetFullAudioPath();
                AudioManager.AudioRate = Utils.Config.GetFloat("Gameplay", "SongRate");
                AudioManager.StartLazyPlayer();

                if (Beatmap.PreviewTime != 0)
                    AudioManager.DeltaTime(Beatmap.PreviewTime);

                PulsarcLogger.Important($"Now Playing: {AudioManager.SongPath}", LogType.Runtime);
            }
        }

        public void SetClicked(bool set)
        {
            IsClicked = set;
        }

        /// <summary>
        /// Adds diffBar to the elements to be updated.
        /// </summary>
        protected override void UpdateElements()
        {
            // Don't bother updating if we aren't on screen.
            if (!OnScreen())
                return;

            base.UpdateElements();

            _diffBar?.ChangePosition(AnchorUtil.FindDrawablePosition(_diffBarStartAnchor, this) + _diffBarOffset);
        }
    }
}
