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

        double distanceToCrosshair;
        double baseSpeed;

        // Optimization
        // Whether this hitobject is marked for destruction in gameplay
        public bool erase;

        // Distance formula values
        static double exponent = 4; // Do not use decimals, or gameplay is weird
        static int targetDisappearDistance = 100;
        static double curveFixing = 1e9;
        static double disappearDistanceFixing = Math.Pow(curveFixing * targetDisappearDistance, 1/exponent);

        public HitObject(int time, int angle, int keys, double baseSpeed) : base(Skin.assets["arcs"])
        {
            this.time = time;
            this.angle = angle;
            this.baseSpeed = baseSpeed;
            erase = false;

            Vector2 screen = Pulsarc.getDimensions();
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

            rotation = (float) (45 * (Math.PI / 180));
        }
        

        public void recalcPos(int currentTime, double speed, int crosshairRadius)
        {
            // Update the size of the object depending on how close (in time) it is from reaching the HitPosition

            Vector2 screen = Pulsarc.getDimensions();

            distanceToCrosshair = getDistanceToCrosshair(currentTime, speed * baseSpeed);

            Resize(getSizeFromDistanceToCrosshair(crosshairRadius), false);
        }

        public int getSizeFromDistanceToCrosshair(int crosshairRadius)
        {
            return (int) (((crosshairRadius / 1920f) * Pulsarc.getDimensions().X) + distanceToCrosshair);
        }

        public double getDistanceToCrosshair(int currentTime, double speed)
        {
            // Formula determinig the route of the arcs on screen.
            var distanceT = time - currentTime;
            return (Math.Pow(distanceT + disappearDistanceFixing, exponent) / curveFixing - targetDisappearDistance) * speed;
        }

        public int IsSeenAt(double distance, double speed)
        {
            // Reverse formula for determining when an arc will first appear on screen
            return (int) (Math.Pow(curveFixing * (distance * speed + targetDisappearDistance), 1 / exponent) - disappearDistanceFixing);
        }

        public bool IsSeen()
        {
            return distanceToCrosshair < 2300;
        }
    }
}
