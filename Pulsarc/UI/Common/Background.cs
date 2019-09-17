using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Microsoft.Xna.Framework;
using Pulsarc.Utils.Graphics;
using Wobble.Logging;
using Pulsarc.Utils;

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

                /* Debug

                Vector2 screenCenter = ScreenAnchor.FindPosition(Anchor.Center);
                Vector2 baseCenter = ScreenAnchor.FindBasePosition(Anchor.Center);

                Vector2 dimensions = Pulsarc.getDimensions();

                Logger.Debug("", LogType.Runtime);

                Logger.Debug(   "Expected Output: " +
                                screenCenter + " " +
                                baseCenter + " " +
                                dimensions + " " +
                                screenCenter +
                                " {X: 0 Y: 0}",
                                LogType.Runtime);

                Logger.Debug(   "Output:          " +
                                screenCenter + " " +
                                baseCenter + " " +
                                currentSize + " " +
                                anchorPosition + " " +
                                truePosition,
                                LogType.Runtime);

                Logger.Debug(   "Difference:      " +
                                (screenCenter - screenCenter) + " " + 
                                (baseCenter - baseCenter) + " " +
                                (dimensions - currentSize) + " " +
                                (screenCenter - anchorPosition) + " " +
                                (Vector2.Zero - truePosition),
                                LogType.Runtime);*/
            }
        }

        /// <summary>
        /// Create a background using the Skin-asset name to find the image.
        /// </summary>
        /// <param name="skinAsset">The name of the asset this background will use.</param>
        /// <param name="dim">Optional parameter to change the background dim. 0 is no dim, 1 is full dim.</param>
        public Background(string skinAsset, float dim = 0f) : base(Skin.assets[skinAsset], anchor: Anchor.Center)
        {
            makeDimTexture(dim);
            changeBackground(Texture);
        }

        /// <summary>
        /// Create a blank, center-anchored background. Can be dimmed. Meant to be used for map backgrounds.
        /// </summary>
        /// <param name="dim">Optional parameter to change the background dim. 0 is no dim, 1 is full dim.</param>
        public Background(float dim = 0f) : base(Skin.DefaultTexture, anchor: Anchor.Center)
        {
            makeDimTexture(dim);
            changeBackground(Texture);
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
                dimTexture = new Drawable(GraphicsUtils.CreateFillTexture(Pulsarc.CurrentWidth, Pulsarc.CurrentHeight, Color.Black));
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

            heightScaled = !Pulsarc.isWiderThan16by9();

            if (!heightScaled)
            {
                Resize(Pulsarc.CurrentWidth);
            }
            else
            {
                Resize(Pulsarc.CurrentHeight);
            }

            changePosition(ScreenAnchor.FindPosition(Anchor.Center));
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);
            if (dim)
            {
                dimTexture.Resize(size);
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
