using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;

namespace Pulsarc.UI
{
    public abstract class FillBar : Drawable
    {
        // Value of this bar
        private float currentValue;
        public float CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value > MaxValue ? MaxValue : value < MinValue ? MinValue : value;
                UpdateDrawnPart();
            }
        }
        protected float MinValue;
        protected float MaxValue;

        // Cosmetics of this bar
        protected FillBarDirection FillDirection;

        // Drawing variables for this bar
        protected bool VerticalDrawing =>
            FillDirection == FillBarDirection.DownToUp || FillDirection == FillBarDirection.UpToDown;

        protected float CurrentPercentage => (CurrentValue - MinValue) / (MaxValue - MinValue);

        public FillBar
        (
            Texture2D texture,
            Vector2 position,
            float min,
            float max,
            float startingValue,
            Vector2? size = null,
            Anchor anchor = Anchor.TopLeft,
            bool heightScaled = true,
            FillBarDirection fillDirection = FillBarDirection.LeftToRight
        )
            : base(texture, position, size ?? new Vector2(texture.Width, texture.Height),
                  anchor: anchor, heightScaled: heightScaled)
        {
            FillDirection = fillDirection;

            MinValue = min;
            MaxValue = max;
            CurrentValue = startingValue;
        }

        public virtual void UpdateDrawnPart() => drawnPart = FindFill();

        protected virtual Rectangle FindFill()
        {
            int height = (int)(Texture.Height * CurrentPercentage);
            int width = (int)(Texture.Width * CurrentPercentage);

            switch (FillDirection)
            {
                case FillBarDirection.DownToUp:
                    return new Rectangle(0, height, Texture.Width, height);
                case FillBarDirection.UpToDown:
                    return new Rectangle(0, 0, Texture.Width, height);
                case FillBarDirection.LeftToRight:
                    return new Rectangle(0, 0, width, Texture.Height);
                case FillBarDirection.RightToLeft:
                    return new Rectangle(width, 0, Texture.Width - width, Texture.Height);
                default:
                    return drawnPart;
            }
        }
    }

    public enum FillBarDirection
    {
        DownToUp,
        UpToDown,
        LeftToRight,
        RightToLeft,
    }
}
