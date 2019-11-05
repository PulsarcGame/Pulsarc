using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class HitObject : Drawable
    {
        // Whether or not this HitObject can be hit
        public bool Hittable { get; protected set; } = true;

        // Whether or not this HitObject should fade before reaching the crosshair
        public bool Hidden { get; protected set; } = false;

        // The time (in ms) from the start of the audio to a Perfect hit
        public int Time { get; protected set; }

        // The direction this HitObject is "falling" from.
        public double Angle { get; protected set; }

        // The theoritical z-axis poisition of this arc.
        // Used to imitate a "falling" effect from the screen to the crosshair.
        public double ZLocation { get; protected set; }

        // The user-defined base speed.
        public double BaseSpeed { get; protected set; }

        // Optimization
        // Whether this hitobject is marked for destruction in gameplay
        public bool ToErase { get; set; } = false;

        /// <summary>
        /// HitObject is an "Arc", the main gameplay element for Pulsarc.
        /// They move from the outer edges of the screen towards the center to
        /// the crosshair. The player can press corresponding keys to "hit" these arcs.
        /// </summary>
        /// <param name="time">The time (in ms) from the start of the audio to a Perfect hit</param>
        /// <param name="angle">The direction this HitObject is "falling" from.</param>
        /// <param name="keys">How many keys are in the current Beatmap. Only 4 keys is working right now.</param>
        /// <param name="baseSpeed">The user-defined base speed for this arc.</param>
        /// <param name="hidden">Whether or not this arc should fade before reaching the crosshair.</param>
        public HitObject(int time, double angle, int keys, double baseSpeed, bool hidden) : base(Skin.Assets["arcs"])
        {
            Time = time;
            Angle = angle;
            BaseSpeed = baseSpeed;
            Hidden = hidden;

            // Find the origin (center) of this Crosshair
            int width = Pulsarc.CurrentWidth;
            int height = Pulsarc.CurrentHeight;

            origin.X = (width / 2) + ((Texture.Width - width) / 2);
            origin.Y = (height / 2) + ((Texture.Height - height) / 2);

            // Position this HitOjbect
            ChangePosition(AnchorUtil.FindScreenPosition(Anchor.Center));

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
            Rotation = (float)(45 * (Math.PI / 180));

            // Set the HitObject's position
            ChangePosition(truePosition);
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
        public virtual void RecalcPos(int currentTime, double speedModifier, double crosshairZLoc)
        {
            // Update the size of the object depending on how close (in time) it is from reaching the HitPosition
            SetZLocation(currentTime, speedModifier * BaseSpeed, crosshairZLoc);
            Resize(FindArcRadius());
        }

        /// <summary>
        /// Calculate and set the current z-axis position for this object.
        /// Also sets transparency if Hidden is activated.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since the start of the audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        protected virtual void SetZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            ZLocation = CalcZLocation(currentTime, speed, crosshairZLoc);

            HandleHiddenState(crosshairZLoc);
        }

        /// <summary>
        /// Set the current transparency of the HitObject if Hidden is activated.
        /// </summary>
        /// <param name="crosshairZLoc">Current crosshair Z-Location</param>
        protected virtual void HandleHiddenState(double crosshairZLoc)
        {
            if (Hidden)
            {
                // When arcs are fully hidden, currently at the 2/3rds mark (between first being seen and being hit)
                double fullFadeLocation = crosshairZLoc - (crosshairZLoc / 3);

                // New opacity is calculated by looking at how far the arc has gone.
               float newOpacity = (float)(fullFadeLocation - ZLocation) / (float)(fullFadeLocation);

               if (ZLocation > fullFadeLocation)
                    newOpacity = 0f;
                // This is for playing with Hidden. Zooms can make 
                // the arcs more opaque with the current implementation
                // of hidden. This makes sure that arcs don't gain opacity.
                else if (newOpacity > Opacity)
                    newOpacity = Opacity;

                Opacity = newOpacity;
            }
        }

        /// <summary>
        /// Calculate the current z-axis position for this object.
        /// </summary>
        /// <param name="currentTime">The current time (in ms) since the start of the audio.</param>
        /// <param name="speedModifier">The current speed modifier.</param>
        /// <param name="crosshairZLoc">The current z-axis poisition of the crosshair.</param>
        protected virtual double CalcZLocation(int currentTime, double speed, double crosshairZLoc)
        {
            int deltaTime = currentTime - Time;

            double zLocation = deltaTime * speed + speed + crosshairZLoc;

            return zLocation;
        }
        
        /// <summary>
        /// Find this object's current arc radius using its current z-axis position.
        /// </summary>
        public float FindArcRadius()
        {
            Vector2 screen = Pulsarc.GetDimensions();

            float radius = (float)(960 / ZLocation * (screen.X / 2));

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
            return (int)(Time - (crosshairZLoc / speed));
        }

        /// <summary>
        /// Returns whether this HitObject can be drawn.
        /// </summary>
        public bool IsSeen()
        {
            return ZLocation > 0;
        }
    }
}
