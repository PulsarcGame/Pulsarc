using Microsoft.Xna.Framework;
using Pulsarc.UI;

namespace Pulsarc.Utils
{
    /// <summary>
    /// A static class used to find the position of a particular Anchor point.
    /// </summary>
    static public class ScreenAnchor
    {
        /// <summary>
        /// Finds the position of the provided anchor on the current screen.
        /// </summary>
        /// <param name="anchor">The anchor of the screen to find the position of.</param>
        /// <returns>A Vector2 representing the Coordinate of the anchor point on the current screen.</returns>
        static public Vector2 FindPosition(Anchor anchor)
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
                case Anchor.TopLeft:
                default:
                    x = 0;
                    y = 0;
                    break;
            }

            return new Vector2(x, y);
        }
        
        /// <summary>
        /// Finds the position of the provided anchor on the base screen.
        /// </summary>
        /// <param name="anchor">The anchor of the screen to find the position of.</param>
        /// <returns>A Vector2 representing the Coordinate of the anchor point on the current screen.</returns>
        static public Vector2 FindBasePosition(Anchor anchor)
        {
            float x;
            float y;

            int xBaseRes = Pulsarc.xBaseRes;
            int yBaseRes = Pulsarc.yBaseRes;

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
                case Anchor.TopLeft:
                default:
                    x = 0;
                    y = 0;
                    break;
            }

            return new Vector2(x, y);
        }
    }
}
