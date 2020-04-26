using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsarc.UI
{
    public abstract class BorderedFillBar : FillBar
    {
        // The Drawable used to create the border around this BFB.
        protected Drawable Border;
        // The offset of position from the border to the fill.
        protected Vector2 BorderOffset;

        /// <summary>
        /// Creates a fill bar with a textured border.
        /// </summary>
        /// <param name="texture">The texture of the fill bar (not the border!)</param>
        /// <param name="position">The position fo the fill bar (and where the border starts)</param>
        /// <param name="min">The minimum value that this bar pays attention to.</param>
        /// <param name="max">The maximum value that this bar pays attention to.</param>
        /// <param name="startingValue">The first value this bar will have.</param>
        /// <param name="border">The texture for the border that will surround this bar.</param>
        /// <param name="size">The size of this bar, or null to use the texture. Default is null.</param>
        /// <param name="borderSize">The size of the bar border, or null to use fill's size.
        /// Default is null.</param>
        /// <param name="borderOffset">The offset of the position of the border with the positioning
        /// of the fill, or null to use the same positioning as the fill. Default is null.</param>
        /// <param name="anchor">The anchor of both the fill and the border.</param>
        /// <param name="heightScaled">Whether or not both the fill and border are height-scaled.</param>
        /// <param name="fillDirection">The direction that this bar fills itself in, default
        /// is Left To Right.</param>
        public BorderedFillBar
        (
            Texture2D texture,
            Vector2 position,
            float min,
            float max,
            float startingValue,
            Texture2D border,
            Vector2? size = null,
            Vector2? borderSize = null,
            Vector2? borderOffset = null,
            Anchor anchor = Anchor.TopLeft,
            bool heightScaled = true,
            FillBarDirection fillDirection = FillBarDirection.LeftToRight
        )
        : base
        (
              texture,
              position,
              min,
              max,
              startingValue,
              size ?? new Vector2(texture.Width, texture.Height),
              anchor,
              heightScaled,
              fillDirection
        )
        {
            Border = new Drawable(border, position,
                        // If borderSize is null, use size, if that's null too, use the texture
                        borderSize ?? size ?? new Vector2(texture.Width, texture.Height),
                        anchor: anchor, heightScaled: heightScaled);

            BorderOffset = borderOffset ?? Vector2.Zero;
            Border.Move(this.BorderOffset);
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            Border.Move(delta, heightScaled);
        }

        public override void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            base.ChangePosition(position, topLeftPositioning);
            Border?.ChangePosition(position, topLeftPositioning);
            Border?.Move(BorderOffset);
        }

        public override void Draw()
        {
            base.Draw();
            Border.Draw();
        }
    }
}
