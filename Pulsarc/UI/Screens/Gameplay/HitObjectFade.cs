using System;
using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Gameplay
{
    /// <summary>
    /// A fading effect after a HitObject is destroyed.
    /// </summary>
    public class HitObjectFade : HitObject
    {
        float timeToFade;

        GameTime gameTime;

        double firstFrameTime;

        public HitObjectFade(HitObject parent, int timeToFade, GameTime gameTime, float opacity = .5f) : base(parent.time, parent.angle, 4, 0)
        {
            hittable = false;

            zLocation = parent.zLocation;

            this.timeToFade = timeToFade;

            this.gameTime = gameTime;
            firstFrameTime = gameTime.TotalGameTime.TotalMilliseconds;

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
            double currentFrameTime = gameTime.TotalGameTime.TotalMilliseconds;
            double deltaTime = currentFrameTime - firstFrameTime;

            return (timeToFade - (float)deltaTime) / timeToFade / 2;
        }
    }
}
