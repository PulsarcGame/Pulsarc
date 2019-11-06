using Pulsarc.UI.Screens.Gameplay;

namespace Pulsarc.UI.Screens.Editor
{
    public abstract class EditorHitObject : HitObject
    {
        private int lastFrameTime = 0;
        private double lastFrameScale = 0;
        private double lastFrameCrosshairZLoc = 0;

        public EditorHitObject(int time, double angle, int keys)
            : base(time, angle, keys, 0, false)
        {
            Hittable = false;
            SetLayoutTexture();
        }

        // Set the texture of this HitObject based on the layout of the Editor
        public abstract void SetLayoutTexture();

        public override void RecalcPos(int currentTime, double currentScale, double crosshairZLoc)
        {
            if (SameAsLastFrame(currentTime, currentScale, crosshairZLoc))
                return;

            SetZLocation(currentTime, currentScale, crosshairZLoc);

            Resize(FindArcRadius());
        }

        public bool SameAsLastFrame(int time, double scale, double crosshairZLoc)
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
    }
}
