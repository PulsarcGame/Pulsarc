using System.Diagnostics;

namespace Pulsarc.UI.Screens.Gameplay
{
    /// <summary>
    /// A fading effect after a HitObject is destroyed.
    /// </summary>
    public class HitObjectFade : HitObject
    {
        float timeToFade;

        Stopwatch timer;

        // Used to determine how the Fade Arc should move.
        double lastCrosshairPosition = -1;

        public HitObjectFade(HitObject parent, int timeToFade, int keys, float opacity = .5f) : base(parent.time, parent.angle, keys, 0, false)
        {
            hittable = false;

            zLocation = parent.zLocation;

            this.timeToFade = timeToFade;

            timer = new Stopwatch();
            timer.Start();

            this.opacity = opacity;
        }

        protected override double calcZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            // Crosshair position cannot be negative, so we use the -1 set earlier to set lastCrosshairPosition for the first time properly.
            if (lastCrosshairPosition == -1) lastCrosshairPosition = crosshairZLoc;

            // If there was no change in crosshair, don't move the arc
            if (lastCrosshairPosition == crosshairZLoc) return zLocation;

            // Find out how much the crosshair moved
            double deltaCrosshairZLoc = lastCrosshairPosition - crosshairZLoc;
            lastCrosshairPosition = crosshairZLoc;

            // Move this arc the same amount the crosshair did.
            return zLocation - deltaCrosshairZLoc;
        }

        public override void Draw()
        {
            opacity = findCurrentFade();

            erase = opacity <= 0;

            if (!erase)
                base.Draw();
        }

        private float findCurrentFade()
        {
            return (timeToFade - timer.ElapsedMilliseconds) / timeToFade / 2;
        }
    }
}
