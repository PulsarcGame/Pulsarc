using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class HitObject : Drawable
    {
        public int time;

        double angle;
        //double radius;

        double zLocation;
        double baseSpeed;

        // Optimization
        // Whether this hitobject is marked for destruction in gameplay
        public bool erase;

        public HitObject(int time, int angle, int keys, double baseSpeed) : base(Skin.assets["arcs"])
        {
            this.time = time;
            this.angle = angle;
            this.baseSpeed = baseSpeed;
            erase = false;

            Vector2 screen = new Vector2(Pulsarc.xBaseRes, Pulsarc.xBaseRes / Pulsarc.baseRatio);
            //radius = (200f / 1920f) * screen.X;

            origin.X = (screen.X / 2) + ((texture.Width - screen.X) / 2);
            origin.Y = (screen.Y / 2) + ((texture.Height - screen.Y) / 2);

            position.X = screen.X / 2;
            position.Y = screen.Y / 2;

            drawnPart.Width = texture.Width / 2;
            drawnPart.Height = texture.Height / 2;

            // Should be changed if we want different than 4 keys.
            // Currently no solution available with drawn rectangles
            switch (angle)
            {
                case 0:
                    drawnPart.X = texture.Width / 2;
                    origin.X -= texture.Width / 2;
                    break;
                case 180:
                    drawnPart.Y = texture.Height / 2;
                    origin.Y -= texture.Height / 2;
                    break;
                case 90:
                    drawnPart.X = texture.Width / 2;
                    drawnPart.Y = texture.Height / 2;
                    origin.X -= texture.Width / 2;
                    origin.Y -= texture.Height / 2;
                    break;
            }

            rotation = (float)(45 * (Math.PI / 180));
            changePosition(position);
        }


        public void recalcPos(int currentTime, double speed, double crosshairZLoc)
        {
            // Update the size of the object depending on how close (in time) it is from reaching the HitPosition
            setZLocation(currentTime, speed * baseSpeed, crosshairZLoc);

            Resize(findArcRadius(), false);
        }

        public void setZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            zLocation = calcZLocation(currentTime, speed, crosshairZLoc);
        }

        public double calcZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            int deltaTime = currentTime - time;

            double zLocation = deltaTime * speed + speed + crosshairZLoc;

            return zLocation;
        }
        
        public float findArcRadius()
        {
            Vector2 screen = Pulsarc.getDimensions();

            float radius = (float)(960 / zLocation * (screen.X / 2));

            return radius;
        }
        
        public int IsSeenAt(double speed, double crosshairZLoc)
        {
            // Reverse formula for determining when an arc will first appear on screen
            return (int)(time - (crosshairZLoc / speed));
        }

        public bool IsSeen()
        {
            return zLocation > 0;
        }
    }
}
