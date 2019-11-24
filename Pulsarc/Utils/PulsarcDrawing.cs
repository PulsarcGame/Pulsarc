using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsarc.Utils
{
    public static class PulsarcDrawing
    {
        /// <summary>
        /// Creates a tinted texture out of the provided texture. Useful for
        /// selecting, dimming, changing colors, etc.
        /// </summary>
        /// <param name="textureToTint">The reference texture</param>
        /// <param name="tint">The color to use on this texture. The alpha of this is ignored.</param>
        /// <param name="tintAmount">How much the tint should change the texture.
        /// Use a value between 0 and 1. Default is .3</param>
        /// <param name="keepOriginalAlpha">Whether to use the original alpha of textureToTint
        /// or allow mixing of alphas.</param>
        /// <returns></returns>
        public static Texture2D CreateTintedTexture(Texture2D textureToTint, Color tint, float tintAmount = .3f, bool keepOriginalAlpha = true)
        {
            int width = textureToTint.Width;
            int height = textureToTint.Height;

            Color[] data = new Color[width * height];
            textureToTint.GetData(data, 0, data.Length);

            // Dim each pixel by the color and the amount desired
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    // The index of data we're at right now.
                    int i = (y * width) + x;

                    // Get the current color of this index
                    Color color = data[i];

                    // If it's fully transparent, don't bother with this pixel.
                    byte alpha = color.A;
                    if (alpha == 0)
                        continue;

                    // Make sure the tint color uses the right alpha.
                    Color trueTint = tint;
                    if (keepOriginalAlpha & alpha != 255)
                        trueTint.A = alpha;

                    // Lerp the original color by the tint color
                    data[i] = Color.Lerp(color, trueTint, tintAmount);
                }

            // Create a new texture, set its data to the new data and return it.
            Texture2D texture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, width, height);
            texture.SetData(data);

            return texture;
        }

        public static Texture2D CreateDimTexture(Texture2D textureToDim, float dimAmount = .3f)
        {
            return CreateTintedTexture(textureToDim, Color.Black, dimAmount);
        }

        public static Texture2D CreateSelectTexture(Texture2D textureToSelect)
        {
            return CreateTintedTexture(textureToSelect, Color.Blue, .5f);
        }
    }
}
