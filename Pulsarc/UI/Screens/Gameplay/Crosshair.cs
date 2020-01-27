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

        // Time it takes (in ms) for the crosshair to fade to nothing in ms when Hidden is enabled.
        private const int HIDDEN_FADE_OUT_TIME = 1000;

        private bool hidden;
        private double hidingAnimationStartTime = -1;
        private bool FullyHidden => Opacity <= 0;

        /// <summary>
        /// The crosshair, or "Judgement Circle" of Pulsarc.
        /// </summary>
        /// <param name="baseCrosshairDiameter">The base diameter for this Crosshair. Default is 300 (the diameter of Intralism's crosshair)</param>
        public Crosshair(float baseCrosshairDiameter = 300, bool hidden = false) : base(Skin.Assets["crosshair"])
        {
            this.hidden = hidden;

            // Find the origin (center) of this Crosshair
            int width = Pulsarc.CurrentWidth;
            int height = Pulsarc.CurrentHeight;

            origin.X = (width / 2) + ((Texture.Width - width) / 2);
            origin.Y = (height / 2) + ((Texture.Height - height) / 2);

            // Set the diameter and resize
            Resize(baseCrosshairDiameter * Pulsarc.HeightScale);

            ChangePosition(AnchorUtil.FindScreenPosition(Anchor.Center));

            // Set the rotation of the object.
            // TODO: Make this customizeable by the beatmap/user setting.
            Rotation = (float)(45 * (Math.PI / 180));
        }

        public override void Draw()
        {
            base.Draw();

            // NOTE: Current implementation of Hidden gives less "oomph" to zooms
            // because the crosshair gives you a reference point to recognize the zooms.
            // When zooms happen with hidden and no crosshair, it looks like weird SV movement.
            if (hidden && !FullyHidden)
            {
                Hide();
            }
        }

        private void Hide()
        {
            double currentTime = PulsarcTime.CurrentElapsedTime;

            if (hidingAnimationStartTime < 0)
            {
                hidingAnimationStartTime = currentTime;
            }

            double delta = currentTime - hidingAnimationStartTime;

            Opacity = Math.Max((HIDDEN_FADE_OUT_TIME - (float)delta) / HIDDEN_FADE_OUT_TIME, 0);
        }

        /// <summary>
        /// Returns the z-axis Position of this Crosshair.
        /// </summary>
        public float GetZLocation() => Pulsarc.CurrentWidth / 2 * Texture.Width / 2 / Diameter;
        
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
