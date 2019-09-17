using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System;
using System.Text;

namespace Pulsarc.UI
{
    public class TextDisplayElement : Drawable
    {
        string name;

        // Text
        SpriteFont font;
        public StringBuilder text;
        float fontScale;
        public Color color;

        Vector2 processedPosition;

        /// <summary>
        /// A text-based Drawable.
        /// </summary>
        /// <param name="name">The text to be displayed.</param>
        /// <param name="position">The position of this TDE</param>
        /// <param name="fontSize">The size of the text in pt. Default is 18.</param>
        /// <param name="anchor">The anchor of this TDE, Default is Anchor.TopLeft</param>
        /// <param name="color">The color of the text, default is White.</param>
        public TextDisplayElement(string name, Vector2 position, int fontSize = 18, Anchor anchor = Anchor.CenterLeft, Color? color = null)
        {
            this.name = name;
            this.anchor = anchor;
            this.color = color ?? Color.White;
            text = new StringBuilder(20);

            font = AssetsManager.fonts["DefaultFont"];
            fontScale = fontSize / 64f * (Pulsarc.getDimensions().Y / Pulsarc.yBaseRes);

            changePosition(position);
            processedPosition = truePosition;
            Update("");
        }

        public void Update(string value)
        {
            text.Clear();
            text.Append(name).Append(value);
            reprocessPosition();
        }

        public void reprocessPosition()
        {

            float newX = truePosition.X;
            float newY = truePosition.Y;
            Vector2 size = font.MeasureString(text) * fontScale;
            size.X *= Pulsarc.getDimensions().X / Pulsarc.xBaseRes;
            size.Y *= Pulsarc.getDimensions().Y / Pulsarc.yBaseRes;

            switch (anchor)
            {
                case Anchor.Center:
                    newX -= size.X / 2;
                    newY -= size.Y / 2;
                    break;
                case Anchor.TopRight:
                    newX -= size.X;
                    break;
                case Anchor.CenterRight:
                    newX -= size.X;
                    newY -= size.Y / 2;
                    break;
                case Anchor.BottomRight:
                    newX -= size.X;
                    newY -= size.Y;
                    break;
                case Anchor.BottomLeft:
                    newY -= size.Y;
                    break;
                case Anchor.CenterLeft:
                    newY -= size.Y / 2;
                    break;
                case Anchor.CenterTop:
                    newX -= size.X / 2;
                    break;
                case Anchor.CenterBottom:
                    newX -= size.X / 2;
                    newY -= size.Y;
                    break;
                case Anchor.TopLeft:
                default:
                    break;
            }

            processedPosition.X = newX;
            processedPosition.Y = newY;
        }

        public override void move(Vector2 position, bool scaledPositioning = true)
        {
            base.move(position, scaledPositioning);
            reprocessPosition();
        }

        public override void Draw()
        {
            try
            {
                Pulsarc.spriteBatch.DrawString(font, text, processedPosition, color, 0, origin, fontScale, SpriteEffects.None, 0);
            } catch
            {
                Console.Write("Could not write " + text);
            }
        }
    }
}
