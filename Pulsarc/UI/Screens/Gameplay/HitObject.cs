using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class HitObject : Drawable
    {
        // Whether or not this HitObject can be hit
        public bool hittable = true;

        // Whether or not this HitObject should fade before reaching the crosshair
        public bool hidden = false;

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
        /// <param name="hidden">Whether or not this arc should fade before reaching the crosshair.</param>
        public HitObject(int time, double angle, int keys, double baseSpeed, bool hidden) : base(Skin.assets["arcs"])
        {
            this.time = time;
            this.angle = angle;
            this.baseSpeed = baseSpeed;
            this.hidden = hidden;
            erase = false;

            // Vector representing the base screen of Pulsarc.
            Vector2 screen = Pulsarc.getDimensions();

            // Find the origin (center) of this HitObject
            origin.X = (screen.X / 2) + ((Texture.Width - screen.X) / 2);
            origin.Y = (screen.Y / 2) + ((Texture.Height - screen.Y) / 2);

            // Determine the position for this HitObject
            truePosition.X = screen.X / 2;
            truePosition.Y = screen.Y / 2;

            // What part of this HitObject should be drawn?
            drawnPart.Width = Texture.Width / 2;
            drawnPart.Height = Texture.Height / 2;

            // Should be changed if we want different than 4 keys.
            // Currently no solution available with drawn rectangles
            switch (angle)
            {
                case 0:
                    drawnPart.X = Texture.Width / 2;
                    origin.X -= Texture.Width / 2;
                    break;
                case 180:
                    drawnPart.Y = Texture.Height / 2;
                    origin.Y -= Texture.Height / 2;
                    break;
                case 90:
                    drawnPart.X = Texture.Width / 2;
                    drawnPart.Y = Texture.Height / 2;
                    origin.X -= Texture.Width / 2;
                    origin.Y -= Texture.Height / 2;
                    break;
            }

            // Set the rotation of the object
            // TODO: Make this customizeable by the beatmap.
            rotation = (float)(45 * (Math.PI / 180));

            // Set the HitObject's position
            changePosition(truePosition);
        }

        /// <summary>
        /// Calculates the new z-axis poisition this HitObject should be in
        /// and updates its size accordingly. Also sets stransparency if
        /// hidden is activated.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since
        /// the start ofthe audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        public virtual void recalcPos(int currentTime, double speedModifier, double crosshairZLoc)
        {
            // Update the size of the object depending on how close (in time) it is from reaching the HitPosition
            setZLocation(currentTime, speedModifier * baseSpeed, crosshairZLoc);
            Resize(findArcRadius());
        }

        /// <summary>
        /// Calculate and set the current z-axis position for this object.
        /// Also sets transparency if Hidden is activated.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since the start of the audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        private void setZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            zLocation = calcZLocation(currentTime, speed, crosshairZLoc);

            // Set hidden status here
            if (hidden)
            {
                double fullFadeLocation = crosshairZLoc - (crosshairZLoc / 3);
                float newOpacity = (float)(fullFadeLocation - zLocation) / (float)(fullFadeLocation);

                if (zLocation > fullFadeLocation)
                {
                    newOpacity = 0f;
                }
                // This is for zooms, which can make hitobjects gain opacity which hurts reading.
                else if (newOpacity > opacity)
                {
                    newOpacity = opacity;
                }

                opacity = newOpacity;
            }
        }

        /// <summary>
        /// Calculate the current z-axis position for this object.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since the start of the audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        protected virtual double calcZLocation(int currentTime, double speed, double crosshairZLoc)
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
