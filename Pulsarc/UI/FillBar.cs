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

        // The percentage of the current value, also represents how "filled" the bar currently is.
        protected float CurrentPercentage => (CurrentValue - MinValue) / (MaxValue - MinValue);

        // The direction that this bar "fills" in
        protected FillBarDirection FillDirection;

        /// <summary>
        /// Creates a Drawable that "Fills" from one side to the other depending on its current value.
        /// </summary>
        /// <param name="texture">The texture this bar uses.</param>
        /// <param name="position">The position of this bar</param>
        /// <param name="min">The minimum value that this bar pays attention to.</param>
        /// <param name="max">The maximum value that this bar pays attention to.</param>
        /// <param name="startingValue">The first value of this bar will have.</param>
        /// <param name="size">The size of this bar, or null to use the texture. Default is null.</param>
        /// <param name="anchor">The anchor of this bar. Default is TopLeft</param>
        /// <param name="heightScaled">Whether or not this bar scales itself by the height
        /// of pulsarc. Default is true.</param>
        /// <param name="fillDirection">The direction that this bar fills itself in, default
        /// is Left To Right.</param>
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
        : base
        (
            texture,
            position,
            // If size wasn't provided, use the texture's size
            size ?? new Vector2(texture.Width, texture.Height),
            anchor: anchor,
            heightScaled: heightScaled
        )
        {
            FillDirection = fillDirection;

            MinValue = min;
            MaxValue = max;
            CurrentValue = startingValue;
        }

        /// <summary>
        /// Updates the drawn part of this bar for the filling effect.
        /// </summary>
        public virtual void UpdateDrawnPart() => drawnPart = FindFill();

        /// <summary>
        /// Finds the fill bounds from current data and returns it as a rectangle.
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// An enum that represents the direction of FillBars' fill.
    /// </summary>
    public enum FillBarDirection
    {
        DownToUp,
        UpToDown,
        LeftToRight,
        RightToLeft,
    }
}
