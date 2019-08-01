using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class HitObject : Drawable
    {
        public int time;

        double angle;
        double radius;

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
            radius = (200f / 1920f) * screen.X;

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


        public void recalcPos(int currentTime, double speed, int crosshairRadius)
        {
            // Update the size of the object depending on how close (in time) it is from reaching the HitPosition
            Vector2 screen = Pulsarc.getDimensions();

            zLocation = findZLocation(currentTime, speed * baseSpeed, crosshairRadius);

            Resize(findArcRadius(zLocation, screen), false);
        }

        //
        public double findZLocation(int currentTime, double speed, int crosshairRadius)
        {
            double crosshairZLoc = 960 * 960 / crosshairRadius; //One of these 960s is half of the base width (1920), the other is probably half of the current arc/crosshair texture width (also 1920)
            //Note: screen.X / 2 is not needed for this second 960, I tried that and it screwed with the offset of the arcs. - FRUP
            int deltaTime = currentTime - time;

            double zLocation = deltaTime * speed + crosshairZLoc;

            return zLocation;
        }
        
        public float findArcRadius(double zLocation, Vector2 screen)
        {
            float radius = (float)(960 / zLocation * (screen.X / 2));

            return radius;
        }

        //I'm not sure what this is supposed to do and nothing I changed here seemed to change anything.
        //I kept the original comment/formula just in case its needed for reference. - FRUP
        public int IsSeenAt(double distance, double speed)
        {
            /* Reverse formula for determining when an arc will first appear on screen
            return (int) (Math.Pow(curveFixing * (distance * speed + targetDisappearDistance), 1 / exponent) - disappearDistanceFixing);*/

            return (int) ((960 * (Pulsarc.xBaseRes / 2)) / distance); //I probably shouldn't use Pulsarc.xBaseRes here should I? lol
        }

        //Things do change if I screw with this method, but idk why. - FRUP
        public bool IsSeen()
        {
            return zLocation > 0;
        }
    }
}
