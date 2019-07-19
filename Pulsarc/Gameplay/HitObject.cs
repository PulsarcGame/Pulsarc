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

        public HitObject(int time, int part, double baseSpeed) : base(Skin.leftArc)
        {
            this.time = time;
            this.baseSpeed = baseSpeed;

            Vector2 screen = Pulsarc.getDimensions();
            radius = (200 / 1080) * screen.X;
            origin.X = screen.X / 2;
            origin.Y = screen.Y / 2;

            position.X = origin.X - radius;
            position.Y = origin.Y - radius;
            
            // TODO: get arc texture depending on keycount
            switch (part)
            {
                case 0:
                    texture = Skin.leftArc;
                    angle = 180;
                    break;
                case 1:
                    texture = Skin.upArc;
                    angle = -90;
                    break;
                case 2:
                    texture = Skin.downArc;
                    angle = 90;
                    break;
                case 3:
                    texture = Skin.rightArc;
                    angle = 0;
                    break;
            }
            setRotation(45);
        }

        public void recalcArc(int currentTime, double speed, int crosshairRadius)
        {
            recalcPos(currentTime, speed, crosshairRadius);
            
            Resize(getSizeFromDistanceToCrosshair());
        }

        public void recalcPos(int currentTime, double speed, int crosshairRadius)
        {
            Vector2 screen = Pulsarc.getDimensions();
            //distance.X = (float)(Math.Cos(angle * (Math.PI / 180)) * (time - currentTime) * baseSpeed);
            //distance.Y = (float)(Math.Sin(angle * (Math.PI / 180)) * (time - currentTime) * baseSpeed);
            distanceToCrosshair = (float) ((time - currentTime) * baseSpeed);

            if(distanceToCrosshair < -50)
            {
                distanceToCrosshair = -50;
            }

            position.X = (float) (Math.Cos(angle * (Math.PI / 180)) * (crosshairRadius / Math.PI)) + screen.X / 2 + (texture.Width *scale) / 2 + (float)(Math.Cos(angle * (Math.PI / 180)) * distanceToCrosshair);
            position.Y = (float) (Math.Sin(angle * (Math.PI / 180)) * (crosshairRadius / Math.PI)) + screen.Y / 2 + (texture.Height*scale) / 2 + (float)(Math.Sin(angle * (Math.PI / 180)) * distanceToCrosshair);
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
