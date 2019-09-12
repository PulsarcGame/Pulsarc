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

        public override Texture2D Texture
        {
            get => base.Texture;

            set
            {
                base.Texture = value;

                // Make dimTexture
                if (dimTexture != null)
                {
                    makeDimTexture(dimTexture.opacity);
                }
            }
        }

        /// <summary>
        /// Create a background using the Skin-asset name to find the image.
        /// </summary>
        /// <param name="skinAsset">The name of the asset this background will use.</param>
        /// <param name="dim">Optional parameter to change the background dim.</param>
        public Background(string skinAsset, float dim = 0f) : base(Skin.assets[skinAsset])
        {
            makeDimTexture(dim);
        }

        /// <summary>
        /// Create a blank, center-anchored background. Can be made transparent. Meant to be used for map backgrounds.
        /// </summary>
        /// <param name="dim">Optional parameter to change the background dim.</param>
        public Background(float dim = 0f) : base(Skin.defaultTexture, -1, Anchor.Center)
        {
            makeDimTexture(dim);
        }
        
        /// <summary>
        /// Makes the dim texture if needed for this Background instance.
        /// </summary>
        /// <param name="dim">The amount of background dim to use. 0 is no dim, 1 is full dim.</param>
        private void makeDimTexture(float dim)
        {
            if (dim > 0)
            {
                this.dim = true;
                dimTexture = new Drawable(GraphicsUtils.CreateFillTexture(Texture.Width, Texture.Height, Color.Black));
                dimTexture.opacity = dim;
            }
        }

        /// <summary>
        /// Change this background's texture to a new texture.
        /// </summary>
        /// <param name="newBackground">The texture to use for the background</param>
        public void changeBackground(Texture2D newBackground)
        {
            Texture = newBackground;

            bool sameAspectAsBase = (float)Texture.Width / Texture.Height == Pulsarc.baseRatio;
            float multiplier = Pulsarc.xBaseRes / Texture.Width;

            // If the texture has the same aspect ratio as the base, resize to base res,
            // if not, resize x to base res and then multiply y by the amount x increased.
            Resize(new Vector2(Pulsarc.xBaseRes, sameAspectAsBase ? Pulsarc.yBaseRes : Texture.Height * multiplier));

            drawnPart = new Rectangle(new Point(0, 0), new Point(Texture.Width, Texture.Height));

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
