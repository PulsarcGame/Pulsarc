using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Editor.UI
{
    public class BeatCircle : BeatDisplay
    {
        private static readonly Texture2D DefaultTexture = Skin.Assets["beat_circle"];
        private static float TintAmount => 1f;

        private static readonly Texture2D wholeBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, WholeBeatColor, TintAmount);
        private static readonly Texture2D halfBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, HalfBeatColor, TintAmount);
        private static readonly Texture2D thirdBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, ThirdBeatColor, TintAmount);
        private static readonly Texture2D fourthBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, FourthBeatColor, TintAmount);
        private static readonly Texture2D sixthBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, SixthBeatColor, TintAmount);
        private static readonly Texture2D eighthBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, EighthBeatColor, TintAmount);
        private static readonly Texture2D twelvethBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, TwelvethBeatColor, TintAmount);
        private static readonly Texture2D sixteenthBeatTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, SixteenthBeatColor, TintAmount);
        private static readonly Texture2D timingPointTexture = PulsarcDrawing.CreateTintedTexture(DefaultTexture, TimingPointColor, TintAmount);

        // The theoretical z-axis position of this BeatCircle.
        // Used to imitate a falling effect from the screen to the crosshair.
        public double ZLocation { get; protected set; }

        public BeatCircle(Beat beat, int time, double scale) : base(beat, time, scale)
        {
            // Find the origin (center) of this BeatCircle
            int width = Pulsarc.CurrentWidth;
            int height = Pulsarc.CurrentHeight;

            origin.X = (width / 2) + ((Texture.Width - width) / 2);
            origin.Y = (height / 2) + ((Texture.Height - height) / 2);

            // Position this BeatCircle
            // TODO: This currently assumes that the Editor display is using the whole screen
            // This should be more dynamic as the editor becomes more dynamic.
            ChangePosition(AnchorUtil.FindScreenPosition(Anchor.Center), true);
        }

        protected override void SetBeatTexture()
        {
            switch (Beat)
            {
                case Beat.Whole:
                    Texture = wholeBeatTexture;
                    break;
                case Beat.Half:
                    Texture = halfBeatTexture;
                    break;
                case Beat.Third:
                    Texture = thirdBeatTexture;
                    break;
                case Beat.Fourth:
                    Texture = fourthBeatTexture;
                    break;
                case Beat.Sixth:
                    Texture = sixthBeatTexture;
                    break;
                case Beat.Eighth:
                    Texture = eighthBeatTexture;
                    break;
                case Beat.Twelveth:
                    Texture = twelvethBeatTexture;
                    break;
                case Beat.Sixteenth:
                    Texture = sixteenthBeatTexture;
                    break;
                case Beat.TimingPoint:
                    Texture = timingPointTexture;
                    break;
                default:
                    Texture = DefaultTexture;
                    break;
            }
        }

        public void RecalcPos(int currentTime, double scale, double crosshairZLoc)
        {
            if (SameAsLastFrame(currentTime, scale, crosshairZLoc)) { return; }

            SetZLocation(currentTime, scale, crosshairZLoc);

            Resize(FindRadius());
        }

        public void SetZLocation(int currentTime, double scale, double crosshairZLoc)
            => ZLocation = ((currentTime - Time) * scale) + scale + crosshairZLoc;

        public float FindRadius()
            => (float)(Texture.Width / 2d / ZLocation * (Pulsarc.CurrentWidth / 2d));

        public bool IsSeen()
            => ZLocation > 0 && ZLocation < 8000 && IsActive();

        public virtual int IsSeenAt(double speed, double crosshairZLoc)
            => (int)(Time - (crosshairZLoc / speed));

        private bool SameAsLastFrame(int time, double scale, double crosshairZLoc)
        {
            // The conditionals we're looking at
            bool timeSame = time == lastFrameTime;
            bool scaleSame = scale == lastFrameScale;
            bool crosshairZLocSame = crosshairZLoc == lastFrameCrosshairZLoc;

            // If any of the current values are different,
            // change the lastFrame values to the current value.
            lastFrameTime = !timeSame ? time : lastFrameTime;
            lastFrameScale = !scaleSame ? scale : lastFrameScale;
            lastFrameCrosshairZLoc = !crosshairZLocSame ? crosshairZLoc : lastFrameCrosshairZLoc;

            // If all our conditionals were true, then this frame is the same.
            // If at least one of our conditionals were false, this frame is different.
            return timeSame && scaleSame && crosshairZLocSame;
        }

        private int lastFrameTime = 0;
        private double lastFrameScale = 0;
        private double lastFrameCrosshairZLoc = 0;

        public bool IsActive()
        {
            switch (Editor.BeatSnapInterval)
            {
                case Beat.Whole:
                    return Beat == Beat.Whole || Beat == Beat.TimingPoint;

                case Beat.Half:
                    return Beat == Beat.Half || Beat == Beat.Whole || Beat == Beat.TimingPoint;
                case Beat.Fourth:
                    return Beat == Beat.Fourth || Beat == Beat.Half || Beat == Beat.Whole || Beat == Beat.TimingPoint;
                case Beat.Eighth:
                    return Beat == Beat.Eighth || Beat == Beat.Fourth || Beat == Beat.Half || Beat == Beat.Whole || Beat == Beat.TimingPoint;
                case Beat.Sixteenth:
                    return Beat == Beat.Sixteenth || Beat == Beat.Eighth || Beat == Beat.Fourth || Beat == Beat.Half || Beat == Beat.Whole || Beat == Beat.TimingPoint;

                case Beat.Third:
                    return Beat == Beat.Half || Beat == Beat.Whole || Beat == Beat.TimingPoint;
                case Beat.Sixth:
                    return Beat == Beat.Sixth || Beat == Beat.Half || Beat == Beat.Whole || Beat == Beat.TimingPoint;
                case Beat.Twelveth:
                    return Beat == Beat.Twelveth || Beat == Beat.Sixth || Beat == Beat.Half || Beat == Beat.Whole || Beat == Beat.TimingPoint;

                default:
                    return false;
            }
        }
    }
}
