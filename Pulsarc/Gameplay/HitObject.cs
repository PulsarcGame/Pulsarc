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

            origin.X = screen.X / 2;
            origin.Y = screen.Y * 0.9f;

            position.X = screen.X / 2;
            position.Y = screen.Y / 2;
        }
        

        public void recalcPos(int currentTime, double speed, int crosshairRadius)
        {
            Vector2 screen = Pulsarc.getDimensions();

            distanceToCrosshair = (float) ((time - currentTime) * baseSpeed);

            if(distanceToCrosshair < 0)
            {
                distanceToCrosshair = 0;
            }

            Resize(getSizeFromDistanceToCrosshair());
        }

        public int getSizeFromDistanceToCrosshair()
        {
            return 90 + (int) (141.5 * (distanceToCrosshair/100));
        }

        public bool IsSeen()
        {
            // true for testing even with bad performance
            return true || texture.Width < Pulsarc.getDimensions().X * 2;
        }
    }
}
