using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pulsarc.Utils
{
    public enum OutlineStyle
    {
        // The shape has no outline
        NoOutline,
        // The shape's outline is within the proportions of the shape
        Inside,
        // The shape's outline is outside the proportions of the shape
        Outside,
        // The shape's outline is split evenly outside and inside the shape's proportions.
        Split,
    }

    /// <summary>
    /// A collection of static methods to draw lines and shapes.
    /// </summary>
    public static class PulsarcDrawing
    {
        #region Squares and Rectangles
        /// <summary>
        /// Creates a Texture2D of a rectangle with an outline.
        /// </summary>
        /// <param name="width">The width of the rectangle, ignoring the outline.</param>
        /// <param name="height">The height of the rectangle, ignoring the outline.</param>
        /// <param name="outlineThickness">The thickness of the outline around the rectangle.</param>
        /// <param name="outlineStyle">How the outline is positioned on the shape.</param>
        /// <param name="fillColor">The color of the rectangle.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawRectangle(int width, int height, int outlineThickness, OutlineStyle outlineStyle, Color fillColor, Color outlineColor)
        {
            // Ignore outline thickness if we were told to have no outline.
            if (outlineStyle == OutlineStyle.NoOutline)
                outlineThickness = 0;
            // Ignore outline style if outline thickness is 0
            else if (outlineThickness == 0)
                outlineStyle = OutlineStyle.NoOutline;

            // Find out how much the outline adds to the proportions.
            int extraWidth, extraHeight;

            switch (outlineStyle)
            {
                case OutlineStyle.Outside:
                    extraWidth = outlineThickness;
                    extraHeight = outlineThickness;
                    break;

                case OutlineStyle.Split:
                    extraWidth = outlineThickness / 2;
                    extraHeight = outlineThickness / 2;
                    break;

                default:
                    extraWidth = 0;
                    extraHeight = 0;
                    break;
            }

            int fullWidth = width + extraWidth;
            int fullHeight = height + extraHeight;

            Texture2D texture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, fullWidth, fullHeight);
            Color[] data = new Color[fullWidth * fullHeight];

            // Draw the rectangle
            for (int x = 0; x < fullWidth; x++)
                for (int y = 0; y < fullHeight; y++)
                {
                    // If the point is on the outline, draw the outline color, otherwise draw the fill color.
                    data[(y * fullWidth) + x] =
                        IsPointOnRectangleInsideOutline(x, y, fullWidth, fullHeight, outlineThickness) ?
                        outlineColor :
                        fillColor;
                }

            texture.SetData(data);

            return texture;
        }

        /// <summary>
        /// Creates a Texture2D of a rectangle with no outline.
        /// </summary>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <returns></returns>
        public static Texture2D DrawRectangle(int width, int height, Color color)
        {
            return DrawRectangle(width, height, 0, OutlineStyle.NoOutline, color, Color.Black);
        }

        /// <summary>
        /// Creates a Texture2D of a rectangle with an outline but no fill.
        /// </summary>
        /// <param name="width">Width of the rectangle, ignoring the outline.</param>
        /// <param name="height">Height of the rectangle, ignoring the outline.</param>
        /// <param name="outlineThickness">Thickness of the outline.</param>
        /// <param name="outlineStyle">The style of the outline.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawRectangleOutline(int width, int height, int outlineThickness, OutlineStyle outlineStyle, Color outlineColor)
        {
            return DrawRectangle(width, height, outlineThickness, outlineStyle, Color.Transparent, outlineColor);

        }

        /// <summary>
        /// Creates a Texture2D of a rectangle with an outline but no fill.
        /// </summary>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="outlineThickness">Thickness of the outline.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawRectangleOutline(int width, int height, int outlineThickness, Color outlineColor)
        {
            return DrawRectangleOutline(width, height, outlineThickness, OutlineStyle.Inside, outlineColor);
        }

        /// <summary>
        /// Creates a Texture2D of a square with an outline.
        /// </summary>
        /// <param name="size">The length of each side on the square, ignoring the outline.</param>
        /// <param name="outlineThickness">The thickness of the outline around the square.</param>
        /// <param name="outlineStyle">How the outline is positioned on the shape.</param>
        /// <param name="fillColor">The color of the rectangle.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawSquare(int size, int outlineThickness, OutlineStyle outlineStyle, Color fillColor, Color outlineColor)
        {
            return DrawRectangle(size, size, outlineThickness, outlineStyle, fillColor, outlineColor);
        }

        /// <summary>
        /// Creates a Texture2D of a square with no outline.
        /// </summary>
        /// <param name="size">The length of each side on the square.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <returns></returns>
        public static Texture2D DrawSquare(int size, Color color)
        {
            return DrawRectangle(size, size, color);
        }

        /// <summary>
        /// Creates a Texture2D of a square with an outline but no fill.
        /// </summary>
        /// <param name="size">The length of each side on the square, ignoring the outline.</param>
        /// <param name="outlineThickness">Thickness of the outline.</param>
        /// <param name="outlineStyle">The style of the outline.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawSquare(int size, int outlineThickness, OutlineStyle outlineStyle, Color outlineColor)
        {
            return DrawSquare(size, outlineThickness, outlineStyle, Color.Transparent, outlineColor);

        }

        /// <summary>
        /// Creates a Texture2D of a square with an outline but no fill.
        /// </summary>
        /// <param name="size">The length of each side on the square.</param>
        /// <param name="outlineThickness">Thickness of the outline.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawSquare(int size, int outlineThickness, Color outlineColor)
        {
            return DrawSquare(size, outlineThickness, OutlineStyle.Inside, outlineColor);
        }

        /// <summary>
        /// Find out whether or not the coordinates are on the outline or not.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="outlineThickness"></param>
        /// <returns></returns>
        private static bool IsPointOnRectangleInsideOutline(int x, int y, int width, int height, int outlineThickness)
        {
            // If there's not outline, then it's obviously false
            if (outlineThickness <= 0)
                return false;

            // Find the margins
            int leftMargin = outlineThickness;
            int rightMargin = width - outlineThickness;
            int topMargin = outlineThickness;
            int bottomMargin = height - outlineThickness;

            // Find what margins this point is inside of
            bool insideLeftMargin = x < leftMargin;
            bool insideRightMargin = x >= rightMargin;
            bool insideTopMargin = y < topMargin;
            bool insideBottomMargin = y >= bottomMargin;

            // Return true if this poitn is within any of the margins
            return insideLeftMargin || insideRightMargin || insideTopMargin || insideBottomMargin;
        }
        #endregion

        #region Circles and Eclipses
        /// <summary>
        /// Creates a Texture2D of a circle with an outline.
        /// </summary>
        /// <param name="radius">The radius of the circle, ignoring the outline.</param>
        /// <param name="outlineThickness">The thickness of the outline around the circle.</param>
        /// <param name="outlineStyle">How the outline is positioned on the shape.</param>
        /// <param name="fillColor">The color of the circle.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawCircle(int radius, int outlineThickness, OutlineStyle outlineStyle, Color fillColor, Color outlineColor)
        {
            // Ignore outline thickness if we were told to have no outline.
            if (outlineStyle == OutlineStyle.NoOutline)
                outlineThickness = 0;
            // Ignore outline style if outline thickness is 0
            else if (outlineThickness == 0)
                outlineStyle = OutlineStyle.NoOutline;

            // Find out how much the outline adds to the proportions.
            int extraRadius;

            switch (outlineStyle)
            {
                case OutlineStyle.Outside:
                    extraRadius = outlineThickness;
                    break;

                case OutlineStyle.Split:
                    extraRadius = outlineThickness / 2;
                    break;

                default:
                    extraRadius = 0;
                    break;
            }

            int fullRadius = radius + extraRadius;
            int diameter = fullRadius * 2;

            Texture2D texture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            // Draw the Circle
            for (int x = 0; x < diameter; x++)
                for (int y = 0; y < diameter; y++)
                    if (IsPointInsideCircle(x, y, fullRadius))
                    {
                        // If the point is on the outline, draw the outline color, otherwise draw the fill color.
                        data[(y * diameter) + x] =
                            IsPointOnCircleInsideOutline(x, y, outlineThickness, fullRadius) ?
                            outlineColor :
                            fillColor;
                    }
                    else
                        data[(y * diameter) + x] = Color.Transparent;

            texture.SetData(data);

            return texture;
        }

        /// <summary>
        /// Creates a Texture2D of a circle with no outline.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color of the circle.</param>
        /// <returns></returns>
        public static Texture2D DrawCircle(int radius, Color color)
        {
            return DrawCircle(radius, 0, OutlineStyle.NoOutline, color, Color.Transparent);
        }

        /// <summary>
        /// Creates a Texture2D of a circle with an outline but no fill.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="outlineThickness">The thickness of the outline.</param>
        /// <param name="outlineStyle">How the outline is positioned on the shape.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        /// <returns></returns>
        public static Texture2D DrawCircle(int radius, int outlineThickness, OutlineStyle outlineStyle, Color outlineColor)
        {
            return DrawCircle(radius, outlineThickness, outlineStyle, Color.Transparent, outlineColor);
        }

        /// <summary>
        /// Creates a Texture2D of a circle with an outline but no fill.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="outlineThickness">The thickness of the outline.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        public static Texture2D DrawCircle(int radius, int outlineThickness, Color outlineColor)
        {
            return DrawCircle(radius, outlineThickness, OutlineStyle.Inside, outlineColor);
        }

        /// <summary>
        /// Finds out whether or not this point is within the outline circle
        /// but not the fill circle.
        /// </summary>
        /// <param name="x">X position of the point.</param>
        /// <param name="y">Y position of the point.</param>
        /// <param name="outlineThickness">Thickness of the outline.</param>
        /// <param name="radius">Radius of the full circle.</param>
        /// <returns></returns>
        private static bool IsPointOnCircleInsideOutline(int x, int y, int outlineThickness, int radius)
        {
            int fillCircleRadius = radius - outlineThickness;

            return IsPointInsideCircle(x, y, radius) && !IsPointInsideCircle(x - outlineThickness, y - outlineThickness, fillCircleRadius);
        }

        /// <summary>
        /// Finds out whether or not this point is within a circle with
        /// the provided radius.
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <param name="y">The y position of the point</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns></returns>
        private static bool IsPointInsideCircle(int x, int y, int radius)
        {
            // Code is based on this SatckExchange answer:
            // https://math.stackexchange.com/a/198769

            bool xIsPositive = x > radius;
            bool yIsPositive = y < radius;

            // Find differences between points.
            int xDiffFromCenter = x - radius;
            int yDiffFromCenter = y - radius;

            // Pythagorean theorem to find distance.
            double distanceFromCenter = Math.Sqrt((xDiffFromCenter * xDiffFromCenter) + (yDiffFromCenter * yDiffFromCenter));

            // We know the point is inside the circle if
            // distance is less than or equal to radius
            return distanceFromCenter < radius;
        }
        #endregion
    }
}
