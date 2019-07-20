using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;

namespace Pulsarc.Gameplay
{
    class HitObject : Drawable
    {
        public int time;
        double baseSpeed;

        float angle;
        float radius;

        float distanceToCrosshair;

        public HitObject(int time, int part, double baseSpeed) : base(Skin.arcs)
        {
            this.time = time;
            this.baseSpeed = baseSpeed;

            Vector2 screen = Pulsarc.getDimensions();
            radius = (200f / 1920f) * screen.X;

            origin.X = (screen.X / 2) + ((texture.Width - screen.X) / 2);
            origin.Y = (screen.Y / 2) + ((texture.Height - screen.Y) / 2);

            position.X = screen.X / 2;
            position.Y = screen.Y / 2;
        }
        

        public void recalcPos(int currentTime, double speed, int crosshairRadius)
        {
            Vector2 screen = Pulsarc.getDimensions();

            distanceToCrosshair = getDistanceToCrosshair(currentTime, baseSpeed);

            if(distanceToCrosshair < crosshairRadius / 2 - crosshairRadius / 10)
            {
                distanceToCrosshair = crosshairRadius / 2 - crosshairRadius / 10;
            }

            Resize(getSizeFromDistanceToCrosshair());
        }

        public int getSizeFromDistanceToCrosshair()
        {
            return 90 + (int) (141.5 * (distanceToCrosshair/100));
        }

        public float getDistanceToCrosshair(int currentTime, double speed)
        {
            var distanceT = time - currentTime;
            return (float) Math.Pow(distanceT,1.1);
        }

        public bool IsSeen()
        {
            // true for testing even with bad performance
            return true || texture.Width < Pulsarc.getDimensions().X * 2;
        }
    }
}
