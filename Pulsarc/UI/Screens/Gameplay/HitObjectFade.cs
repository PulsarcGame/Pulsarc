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

        public HitObjectFade(HitObject parent, int timeToFade, int keys, float opacity = .5f) : base(parent.time, parent.angle, keys, 0, false)
        {
            hittable = false;

            zLocation = parent.zLocation;

            this.timeToFade = timeToFade;

            timer = new Stopwatch();
            timer.Start();

            this.opacity = opacity;
        }

        public override void recalcPos(int currentTime, double speedModifier, double crosshairZLoc)
        {
            Resize(findArcRadius(), false);
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
