using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Editor.UI
{
    public class BeatCircle : BeatDisplay
    {
        private static Texture2D wholeBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 4, WholeBeatColor);
        private static Texture2D halfBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 2, HalfBeatColor);
        private static Texture2D thirdBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 2, ThirdBeatColor);
        private static Texture2D fourthBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 2, FourthBeatColor);
        private static Texture2D sixthBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 2, SixthBeatColor);
        private static Texture2D eighthBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 2, EightBeatColor);
        private static Texture2D twelvethBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 2, TwelvethBeatColor);
        private static Texture2D sixteenthBeat = PulsarcDrawing.DrawCircle(Pulsarc.BaseWidth, 2, SixteenthBeatColor);

        private int lastFrameTime = 0;
        private double lastFrameScale = 0;
        private double lastFrameCrosshairZLoc = 0;

        // The theoretical z-axis position of this arc.
        // Used to imitate a falling effect from the screen to the crosshair.
        public double ZLocation { get; protected set; }

        public BeatCircle(Beat beat, int time, float scale) : base(beat, time, scale)
        {
            // Find the origin (center) of this BeatCircle
            int width = Pulsarc.CurrentWidth;
            int height = Pulsarc.CurrentHeight;

            origin.X = (width / 2) + ((Texture.Width - width) / 2);
            origin.Y = (height / 2) + ((Texture.Height - height) / 2);

            // Position this BeatCircle
            // TODO: This currently assumes that the Editor display is using the whole screen
            // This should be more dynamic as the editor becomes more dynamic.
            ChangePosition(AnchorUtil.FindScreenPosition(Anchor.Center));
        }

        /// <summary>
        /// Set the texture of this BeatCircle to the appropriate texture.
        /// </summary>
        protected override void SetBeatTexture()
        {
            switch (Beat)
            {
                case Beat.Whole:
                    Texture = wholeBeat;
                    break;
                case Beat.Half:
                    Texture = halfBeat;
                    break;
                case Beat.Third:
                    Texture = thirdBeat;
                    break;
                case Beat.Fourth:
                    Texture = fourthBeat;
                    break;
                case Beat.Sixth:
                    Texture = sixthBeat;
                    break;
                case Beat.Eighth:
                    Texture = eighthBeat;
                    break;
                case Beat.Twelveth:
                    Texture = twelvethBeat;
                    break;
                case Beat.Sixteenth:
                    Texture = sixteenthBeat;
                    break;
            }
        }

        public void RecalcPos(int currentTime, float scale, double crosshairZLoc)
        {
            if (SameAsLastFrame(currentTime, scale, crosshairZLoc))
                return;

            SetZLocation(currentTime, scale, crosshairZLoc);

            Resize(FindRadius());
        }

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

        public void SetZLocation(int currentTime, float scale, double crosshairZLoc)
        {
            int deltaTime = currentTime - Time;

            ZLocation = deltaTime * scale + scale + crosshairZLoc;
        }

        public float FindRadius()
        {
            float radius = (float)(960 / ZLocation * (Pulsarc.CurrentHeight / 2));

            return radius;
        }

        public bool IsSeen(double crosshairZLoc)
        {
            return ZLocation > 0 && ZLocation < crosshairZLoc + (crosshairZLoc / 3);
        }
    }
}
