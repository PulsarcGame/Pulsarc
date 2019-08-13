using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class Crosshair : Drawable
    {
        // The current diameter of this Crosshair.
        public double diameter;

        /// <summary>
        /// The crosshair, or "Judgement Circle" of Pulsarc.
        /// </summary>
        /// <param name="baseCrosshairDiameter">The base diameter for this Crosshair.</param>
        public Crosshair(double baseCrosshairDiameter) : base(Skin.assets["crosshair"])
        {
            // Vector representing the base screen of Pulsarc.
            Vector2 screen = Pulsarc.getBaseScreenDimensions();

            // Set the diameter.
            diameter = baseCrosshairDiameter;

            // Find the origin (center) of this Crosshair
            origin.X = (screen.X / 2) + ((texture.Width - screen.X) / 2);
            origin.Y = (screen.Y / 2) + ((texture.Height - screen.Y) / 2);

            // Determine the position for this Crosshair
            position.X = screen.X / 2;
            position.Y = screen.Y / 2;

            Resize((float)diameter);
            changePosition(position);

            // Set the rotation of the object.
            // TODO: Make this customizeable by the beatmap.
            rotation = (float)(45 * (Math.PI / 180));
        }

        /// <summary>
        /// Returns the z-axis Position of this Crosshair.
        /// </summary>
        public double getZLocation()
        {
            return 960 * (texture.Width / 2) / diameter;
        }
    }
}
