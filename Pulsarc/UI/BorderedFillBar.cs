using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsarc.UI
{
    public abstract class BorderedFillBar : FillBar
    {
        public Drawable border;
        protected Vector2 borderOffset;

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
            this.border =
                new Drawable(border, position,
                    borderSize ?? size ?? new Vector2(texture.Width, texture.Height),
                    anchor: anchor, heightScaled: heightScaled);

            this.borderOffset = borderOffset ?? Vector2.Zero;
            this.border.Move(this.borderOffset);
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            border.Move(delta, heightScaled);
        }

        public override void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            base.ChangePosition(position, topLeftPositioning);
            border?.ChangePosition(position, topLeftPositioning);
            border?.Move(borderOffset);
        }

        public override void Draw()
        {
            base.Draw();
            border.Draw();
        }
    }
}
