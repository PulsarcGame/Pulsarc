using Microsoft.Xna.Framework;
using Pulsarc.UI;

namespace Pulsarc.Utils
{
    /// <summary>
    /// A static class used to find the position of a particular Anchor point.
    /// </summary>
    public static class AnchorUtil
    {
        /// <summary>
        /// Finds the position of the provided anchor on the provided drawable.
        /// </summary>
        /// <param name="anchor">The anchor of the drawable to find the position of.</param>
        /// <param name="drawable">The drawable to find the anchorposition of.</param>
        /// <returns>A Vector2 representing the Coordinate of the anchor point on the provided drawable.</returns>
        public static Vector2 FindDrawablePosition(Anchor anchor, in Drawable drawable)
        {
            float x = drawable.TruePosition.X;
            float y = drawable.TruePosition.Y;

            float width = drawable.CurrentSize.X;
            float height = drawable.CurrentSize.Y;

            switch (anchor)
            {
                case Anchor.Center:
                    x += width / 2f;
                    y += height / 2f;
                    break;
                case Anchor.TopRight:
                    x += width;
                    y += 0;
                    break;
                case Anchor.CenterRight:
                    x += width;
                    y += height / 2f;
                    break;
                case Anchor.BottomRight:
                    x += width;
                    y += height;
                    break;
                case Anchor.CenterLeft:
                    x += 0;
                    y += height / 2f;
                    break;
                case Anchor.BottomLeft:
                    x += 0;
                    y += height;
                    break;
                case Anchor.CenterTop:
                    x += width / 2f;
                    y += 0;
                    break;
                case Anchor.CenterBottom:
                    x += width / 2f;
                    y += height;
                    break;

                default:
                    x += 0;
                    y += 0;
                    break;
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// Finds the position of the provided anchor on the current screen.
        /// </summary>
        /// <param name="anchor">The anchor of the screen to find the position of.</param>
        /// <returns>A Vector2 representing the Coordinate of the anchor point on the current screen.</returns>
        public static Vector2 FindScreenPosition(Anchor anchor)
        {
            float x;
            float y;

            int currentWidth = Pulsarc.CurrentWidth;
            int currentHeight = Pulsarc.CurrentHeight;

            switch (anchor)
            {
                case Anchor.Center:
                    x = currentWidth / 2f;
                    y = currentHeight / 2f;
                    break;
                case Anchor.TopRight:
                    x = currentWidth;
                    y = 0;
                    break;
                case Anchor.CenterRight:
                    x = currentWidth;
                    y = currentHeight / 2f;
                    break;
                case Anchor.BottomRight:
                    x = currentWidth;
                    y = currentHeight;
                    break;
                case Anchor.CenterLeft:
                    x = 0;
                    y = currentHeight / 2f;
                    break;
                case Anchor.BottomLeft:
                    x = 0;
                    y = currentHeight;
                    break;
                case Anchor.CenterTop:
                    x = currentWidth / 2f;
                    y = 0;
                    break;
                case Anchor.CenterBottom:
                    x = currentWidth / 2f;
                    y = currentHeight;
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }

            return new Vector2(x, y);
        }
        
        /// <summary>
        /// Finds the position of the provided anchor on the base screen (1920x1080).
        /// Legacy?
        /// </summary>
        /// <param name="anchor">The anchor of the screen to find the position of.</param>
        /// <returns>A Vector2 representing the Coordinate of the anchor point on the current screen.</returns>
        public static Vector2 FindBaseScreenPosition(Anchor anchor)
        {
            float x;
            float y;

            int xBaseRes = Pulsarc.BASE_WIDTH;
            int yBaseRes = Pulsarc.BASE_HEIGHT;

            switch (anchor)
            {
                case Anchor.Center:
                    x = xBaseRes / 2f;
                    y = yBaseRes / 2f;
                    break;
                case Anchor.TopRight:
                    x = xBaseRes;
                    y = 0;
                    break;
                case Anchor.CenterRight:
                    x = xBaseRes;
                    y = yBaseRes / 2f;
                    break;
                case Anchor.BottomRight:
                    x = xBaseRes;
                    y = yBaseRes;
                    break;
                case Anchor.CenterLeft:
                    x = 0;
                    y = yBaseRes / 2f;
                    break;
                case Anchor.BottomLeft:
                    x = 0;
                    y = yBaseRes;
                    break;
                case Anchor.CenterTop:
                    x = xBaseRes / 2f;
                    y = 0;
                    break;
                case Anchor.CenterBottom:
                    x = xBaseRes / 2f;
                    y = yBaseRes;
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }

            return new Vector2(x, y);
        }
    }
}
