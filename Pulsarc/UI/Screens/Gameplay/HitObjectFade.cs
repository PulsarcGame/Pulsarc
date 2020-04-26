using Pulsarc.Utils;
using System.Diagnostics;

namespace Pulsarc.UI.Screens.Gameplay
{
    /// <summary>
    /// A fading effect after a HitObject is destroyed.
    /// </summary>
    public class HitObjectFade : HitObject
    {
        private float timeToFade;

        private double timeWhenSpawned;

        private float startingOpacity;

        // Used to determine how the Fade Arc should move.
        private double lastCrosshairPosition = -1;

        /// <summary>
        /// Create a new HitObjectFade, which will fade away based on the config
        /// and the parent's position.
        /// </summary>
        /// <param name="parent">The Parent HitObject that this Fade borrows stats from.</param>
        /// <param name="timeToFade">How long does it take for this object to fade away?</param>
        /// <param name="keys">The number of keys in gameplay.</param>
        /// <param name="startingOpacity">Starting opacity value from 0-1. Default value is .5</param>
        public HitObjectFade(HitObject parent, int timeToFade, int keys, float startingOpacity = .5f)
            : base(parent.Time, parent.Angle, keys, 0, false)
        {
            // This is a cosmetic arc, not a gameplay arc.
            // Turning Hittable off makes it so the GameplayEngine
            // Doesn't consider this an arc that can be hit.
            Hittable = false;

            // Set Stats
            ZLocation = parent.ZLocation;

            this.timeToFade = timeToFade;

            this.startingOpacity = startingOpacity;
            Opacity = startingOpacity;

            timeWhenSpawned = PulsarcTime.CurrentElapsedTime;
        }

        /// <summary>
        /// Override CalcZLocation so this fade object doesn't move on its own.
        /// </summary>
        /// <param name="currentTime">The current gameplay time</param>
        /// <param name="speed">Current gameplay speed</param>
        /// <param name="crosshairZLoc">Current Crosshair Z-Location</param>
        /// <returns></returns>
        protected override double CalcZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            // Crosshair position cannot be negative, so we check to see if it's
            // equal to -1 set earlier so we can properly set lastCrosshairPosition now.
            if (lastCrosshairPosition == -1)
                lastCrosshairPosition = crosshairZLoc;

            // If there was no change in crosshair, don't move the arc
            if (lastCrosshairPosition == crosshairZLoc)
                return ZLocation;

            // Find out how much the crosshair moved
            double deltaCrosshairZLoc = lastCrosshairPosition - crosshairZLoc;
            lastCrosshairPosition = crosshairZLoc;

            // Move this arc the same amount the crosshair did.
            return ZLocation - deltaCrosshairZLoc;
        }

        public override void Draw()
        {
            Opacity = FindCurrentFade();

            ToErase = Opacity <= 0;

            if (!ToErase)
                base.Draw();
        }

        private float FindCurrentFade()
        {
            double currentFadeTime = timeWhenSpawned + timeToFade - PulsarcTime.CurrentElapsedTime;
            float currentFade = (float)(currentFadeTime / timeToFade / (1 / startingOpacity));

            return currentFade;
        }
    }
}
