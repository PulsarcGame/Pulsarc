using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using Pulsarc.Utils.Graphics;

namespace Pulsarc.UI
{
    public class Background : Drawable
    {
        public Drawable DimTexture { get; protected set; }

        public bool Dimmed { get; set; }

        public sealed override Texture2D Texture
        {
            get => base.Texture;

            set
            {
                base.Texture = value;

                if (Texture == Skin.DefaultTexture)
                    Texture = GraphicsUtils.CreateSolidColorTexture(Pulsarc.CurrentWidth, Pulsarc.CurrentHeight, Color.Black);
                
                // Make dimTexture
                if (DimTexture != null)
                    MakeDimTexture(DimTexture.Opacity);
            }
        }

        public override bool HeightScaled { get => WiderThanOrSameAsPulsarc(); }

        /// <summary>
        /// Create a background using the Skin-asset name to find the image.
        /// </summary>
        /// <param name="skinAsset">The name of the asset this background will use.</param>
        /// <param name="dim">Optional parameter to change the background dim. 0 is no dim, 1 is full dim.</param>
        public Background(string skinAsset, float dim = 0f) : base(Skin.Assets[skinAsset], anchor: Anchor.Center)
        {
            MakeDimTexture(dim);
            ChangeBackground(Texture);
        }

        /// <summary>
        /// Create a blank, center-anchored background. Can be dimmed. Meant to be used for map backgrounds.
        /// </summary>
        /// <param name="dim">Optional parameter to change the background dim. 0 is no dim, 1 is full dim.</param>
        public Background(float dim = 0f) : base(Skin.DefaultTexture, anchor: Anchor.Center)
        {
            MakeDimTexture(dim);
            ChangeBackground(Texture);
        }
        
        /// <summary>
        /// Makes the dim texture if needed for this Background instance.
        /// </summary>
        /// <param name="dim">The amount of background dim to use. 0 is no dim, 1 is full dim.</param>
        private void MakeDimTexture(float dim)
        {
            if (!(dim > 0)) return;
            Dimmed = true;
            DimTexture = new Drawable(GraphicsUtils.CreateSolidColorTexture(Pulsarc.CurrentWidth, Pulsarc.CurrentHeight, Color.Black));
            DimTexture.Opacity = dim;
        }

        /// <summary>
        /// Change this background's texture to a new texture.
        /// </summary>
        /// <param name="newBackground">The texture to use for the background</param>
        public void ChangeBackground(Texture2D newBackground)
        {
            Texture = newBackground;

            Resize(!HeightScaled ? Pulsarc.CurrentWidth : Pulsarc.CurrentHeight);

            ChangePosition(AnchorUtil.FindScreenPosition(Anchor.Center));
        }

        private bool WiderThanOrSameAsPulsarc()
        {
            return (float)Texture.Width / Texture.Height >= Pulsarc.CurrentAspectRatio;
        }

        public override void Resize(Vector2 size)
        {
            base.Resize(size);

            if (Dimmed)
                DimTexture.Resize(size);
        }

        public override void Draw()
        {
            base.Draw();

            if (Dimmed)
                DimTexture.Draw();
        }
    }
}
