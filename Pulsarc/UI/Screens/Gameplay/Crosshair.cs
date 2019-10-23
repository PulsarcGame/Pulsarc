using System;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class Crosshair : Drawable
    {
        // The current diameter of this Crosshair.
        public float diameter;

        // The current radius of this Crosshair.
        public float radius;

        /// <summary>
        /// The crosshair, or "Judgement Circle" of Pulsarc.
        /// </summary>
        /// <param name="baseCrosshairDiameter">The base diameter for this Crosshair.</param>
        public Crosshair(float baseCrosshairDiameter) : base(Skin.assets["crosshair"])
        {
            // Find the origin (center) of this Crosshair
            int width = Pulsarc.CurrentWidth;
            int height = Pulsarc.CurrentHeight;

            origin.X = (width / 2) + ((Texture.Width - width) / 2);
            origin.Y = (height / 2) + ((Texture.Height - height) / 2);

            // Set the diameter and resize
            Resize(baseCrosshairDiameter * Pulsarc.HeightScale);
            changePosition(AnchorUtil.FindScreenPosition(Anchor.Center));

            // Set the rotation of the object.
            // TODO: Make this customizeable by the beatmap.
            rotation = (float)(45 * (Math.PI / 180));
        }

        /// <summary>
        /// Returns the z-axis Position of this Crosshair.
        /// </summary>
        public float getZLocation()
        {
            return ((Pulsarc.CurrentWidth / 2) * Texture.Width / 2) / diameter;
        }
        
        /// <summary>
        /// Resizes the crosshair, and sets diameter to the size
        /// </summary>
        /// <param name="size">The size/diameter to resize this crosshair to</param>
        public override void Resize(float size)
        {
            diameter = size;
            radius = diameter / 2;
            base.Resize(diameter);
        }
    }
}
