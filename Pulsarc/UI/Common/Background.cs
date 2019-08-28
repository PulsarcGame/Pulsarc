using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Pulsarc.Utils.Graphics;

namespace Pulsarc.UI.Common
{
    public class Background : Drawable
    {
        public Drawable dimTexture;
        public bool dim = false;

        /// <summary>
        /// Create a background using the Skin-asset name to find the image.
        /// </summary>
        /// <param name="skinAsset">The name of the asset this background will use.</param>
        public Background(string skinAsset, float dim = 0f) : base(Skin.assets[skinAsset])
        {
            if (dim > 0)
            {
                this.dim = true;
                dimTexture = new Drawable(GraphicsUtils.CreateFillTexture(1, 1, Color.Black));
                dimTexture.opacity = dim;
            }
        }

        /// <summary>
        /// Create a blank, center-anchored background. Can be made transparent. Meant to be used for map backgrounds.
        /// </summary>
        /// <param name="opacity">Optional parameter to change the opacity of the background.</param>
        public Background(float dim = 0f) : base(Skin.defaultTexture, -1, Anchor.Center)
        {
            if (dim > 0)
            {
                this.dim = true;
                dimTexture = new Drawable(GraphicsUtils.CreateFillTexture(1, 1, Color.Black));
                dimTexture.opacity = dim;
            }
        }

        /// <summary>
        /// Change this background's texture to a new texture.
        /// </summary>
        /// <param name="path">The folder to look in.</param>
        /// <param name="asset">The filename of the picture to use.</param>
        public void changeBackground(Texture2D newBackground)
        {
            texture = newBackground;

            bool sameAspectAsBase = (float)texture.Width / texture.Height == Pulsarc.baseRatio;
            float multiplier = Pulsarc.xBaseRes / texture.Width;

            // If the texture has the same aspect ratio as the base, resize to base res,
            // if not, resize x to base res and then multiply y by the amount x increased.
            Resize(new Vector2(Pulsarc.xBaseRes, sameAspectAsBase ? Pulsarc.yBaseRes : (float)texture.Height * multiplier));

            drawnPart = new Rectangle(new Point(0, 0), new Point(texture.Width, texture.Height));

            position = Pulsarc.getBaseScreenDimensions() / 2f;
            drawPosition = new Vector2(0, 0);
        }

        public override void Resize(Vector2 size, bool resolutionScale = true)
        {
            base.Resize(size, resolutionScale);
            if (dim)
            {
                dimTexture.Resize(size, resolutionScale);
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (dim)
            {
                dimTexture.Draw();
            }
        }
    }
}
