using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Common
{
    public class Background : Drawable
    {
        /// <summary>
        /// Create a background using the Skin-asset name to find the image.
        /// </summary>
        /// <param name="skinAsset">The name of the asset this background will use.</param>
        public Background(string skinAsset) : base(Skin.assets[skinAsset]) {}

        /// <summary>
        /// Create a blank, center-anchored background. Can be made transparent. Meant to be used for map backgrounds.
        /// </summary>
        /// <param name="opacity">Optional parameter to change the opacity of the background.</param>
        public Background(float opacity = 1f) : base(Skin.defaultTexture, -1, Anchor.Center)
        {
            this.opacity = opacity;
        }

        /// <summary>
        /// Change this background's texture to a new texture.
        /// </summary>
        /// <param name="path">The folder to look in.</param>
        /// <param name="asset">The filename of the picture to use.</param>
        public void changeBackground(string path, string asset)
        {
            try
            {
                texture = Texture2D.FromStream(Pulsarc.graphics.GraphicsDevice, File.Open(path + "/" + asset, FileMode.Open));

                bool sameAspectAsBase = (float)texture.Width / texture.Height == Pulsarc.baseRatio;
                float multiplier = Pulsarc.xBaseRes / texture.Width;

                // If the texture has the same aspect ratio as the base, resize to base res,
                // if not, resize x to base res and then multiply y by the amount x increased.
                Resize(new Vector2(Pulsarc.xBaseRes, sameAspectAsBase ? Pulsarc.yBaseRes : (float)texture.Height * multiplier));
                
                drawnPart = new Rectangle(new Point(0, 0), new Point(texture.Width, texture.Height));
            }
            catch
            {
                Console.WriteLine("Failed to load " + asset + " in " + path);
                texture = Skin.defaultTexture;

                drawnPart = new Rectangle(new Point(0, 0), new Point(0, 0));
            }

            position = Pulsarc.getBaseScreenDimensions() / 2f;
            drawPosition = new Vector2(0, 0);
        }
    }
}
