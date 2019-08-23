using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class HitObject : Drawable
    {
        // Whether or not this HitObject can be hit
        public bool hittable = true;

        // The time (in ms) from the start of the audio to a Perfect hit
        public int time;

        // The direction this HitObject is "falling" from.
        internal double angle;

        // The theoritical z-axis poisition of this arc to assist with imitating a "falling" effect from the screen to the crosshair.
        internal double zLocation;

        // The user-defined base speed.
        internal double baseSpeed;


        // Optimization
        // Whether this hitobject is marked for destruction in gameplay
        public bool erase;

        /// <summary>
        /// HitObject is an "Arc", the main gameplay element for Pulsarc.
        /// </summary>
        /// <param name="time">The time (in ms) from the start of the audio to a Perfect hit</param>
        /// <param name="angle">The direction this HitObject is "falling" from.</param>
        /// <param name="keys">How many keys are in the current Beatmap. Only 4 keys is working right now.</param>
        /// <param name="baseSpeed">The user-defined base speed for this arc.</param>
        public HitObject(int time, double angle, int keys, double baseSpeed) : base(Skin.assets["arcs"])
        {
            this.time = time;
            this.angle = angle;
            this.baseSpeed = baseSpeed;
            erase = false;

            // Vector representing the base screen of Pulsarc.
            Vector2 screen = Pulsarc.getBaseScreenDimensions();

            // Find the origin (center) of this HitObject
            origin.X = (screen.X / 2) + ((texture.Width - screen.X) / 2);
            origin.Y = (screen.Y / 2) + ((texture.Height - screen.Y) / 2);

            // Determine the position for this HitObject
            position.X = screen.X / 2;
            position.Y = screen.Y / 2;

            // What part of this HitObject should be drawn?
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

            // Set the rotation of the object
            // TODO: Make this customizeable by the beatmap.
            rotation = (float)(45 * (Math.PI / 180));

            // Set the HitObject's position
            changePosition(position);
        }

        /// <summary>
        /// Calculates the new z-axis poisition this HitObject should be in
        /// and updates its size accordingly.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since
        /// the start ofthe audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        public virtual void recalcPos(int currentTime, double speedModifier, double crosshairZLoc)
        {
            // Update the size of the object depending on how close (in time) it is from reaching the HitPosition
            setZLocation(currentTime, speedModifier * baseSpeed, crosshairZLoc);
            Resize(findArcRadius(), false);
        }

        /// <summary>
        /// Calculate and set the current z-axis position for this object.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since the start of the audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        private void setZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            zLocation = calcZLocation(currentTime, speed, crosshairZLoc);
        }

        /// <summary>
        /// Calculate the current z-axis position for this object.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since the start of the audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        private double calcZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            int deltaTime = currentTime - time;

            double zLocation = deltaTime * speed + speed + crosshairZLoc;

            return zLocation;
        }
        
        /// <summary>
        /// Find this object's current arc radius using its current z-axis position.
        /// </summary>
        public float findArcRadius()
        {
            Vector2 screen = Pulsarc.getDimensions();

            float radius = (float)(960 / zLocation * (screen.X / 2));

            return radius;
        }
        
        /// <summary>
        /// Return the time (in ms) since the start of the audio
        /// when this HitObject should first be drawn.
        /// </summary>
        /// <param name="speed">Arc Speed TODO: Figure out if this needs to consider
        /// all speed changes before, or if it only needs to consider the speed of
        /// the HitObject's time.</param>
        /// <param name="crosshairZLoc">The z-axis position of the crosshair.</param>
        public int IsSeenAt(double speed, double crosshairZLoc)
        {
            // Reverse formula for determining when an arc will first appear on screen
            return (int)(time - (crosshairZLoc / speed));
        }

        /// <summary>
        /// Returns whether this HitObject is currently being drawn.
        /// </summary>
        public bool IsSeen()
        {
            return zLocation > 0;
        }
    }
}
