using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class Crosshair : Drawable
    {
        // The current diameter of this Crosshair.
        public float Diameter { get; private set; }

        // The current radius of this Crosshair.
        public float Radius => Diameter / 2;

        /// <summary>
        /// The crosshair, or "Judgement Circle" of Pulsarc.
        /// </summary>
        /// <param name="baseCrosshairDiameter">The base diameter for this Crosshair. Default is 300 (the diameter of Intralism's crosshair)</param>
        public Crosshair(float baseCrosshairDiameter = 300) : base(Skin.Assets["crosshair"])
        {
            // Find the origin (center) of this Crosshair
            int width = Pulsarc.CurrentWidth;
            int height = Pulsarc.CurrentHeight;

            Origin = new Vector2
            (
                (width / 2) + ((Texture.Width - width) / 2),
                (height / 2) + ((Texture.Height - height) / 2)
            );

            // Set the diameter and resize
            Resize(baseCrosshairDiameter * Pulsarc.HeightScale);
            ChangePosition(AnchorUtil.FindScreenPosition(Anchor.Center));

            // Set the rotation of the object.
            // TODO: Make this customizeable by the beatmap/user setting.
            Rotation = (float)(45 * (Math.PI / 180));
        }

        /// <summary>
        /// Returns the z-axis Position of this Crosshair.
        /// </summary>
        public float GetZLocation()
        {
            return ((Pulsarc.CurrentWidth / 2) * Texture.Width / 2) / Diameter;
        }
        
        /// <summary>
        /// Resizes the crosshair, and sets diameter to the size
        /// </summary>
        /// <param name="size">The size/diameter to resize this crosshair to</param>
        public override void Resize(float size)
        {
            Diameter = size;
            base.Resize(Diameter);
        }
    }
}
