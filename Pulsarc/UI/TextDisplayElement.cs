using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System;
using System.Text;

namespace Pulsarc.UI
{
    public enum Anchor
    {
        Center = 0,
        TopRight = 1,
        CenterRight = 2,
        BottomRight = 3,
        TopLeft = 4,
        CenterLeft = 5,
        BottomLeft = 6,
    }
    class TextDisplayElement : Drawable
    {
        string name;

        SpriteFont font;
        StringBuilder text;
        float fontScale;
        public Color color;

        Vector2 processedPosition;

        public TextDisplayElement(string name, Vector2 position, int fontSize = 18, Anchor anchor = Anchor.TopLeft, Color? color = null)
        {
            this.name = name;
            this.anchor = anchor;
            this.color = color ?? Color.White;
            text = new StringBuilder(20);

            font = AssetsManager.fonts["DefaultFont"];
            color = Color.White;
            fontScale = fontSize / 64f * (Pulsarc.getDimensions().X / 1920);

            this.position = position;
            processedPosition = this.position;
            changePosition(position);
            Update("");
        }

        public void Update(string value)
        {
            text.Clear();
            text.Append(name).Append(value);

            float newX = position.X;
            float newY = position.Y;
            Vector2 size = font.MeasureString(text) * fontScale;
            size.X *= Pulsarc.getDimensions().X / Pulsarc.xBaseRes;
            size.Y *= Pulsarc.getDimensions().Y / (Pulsarc.xBaseRes / Pulsarc.baseRatio);

            switch (anchor)
            {
                case Anchor.Center:
                    newX  -= size.X / 2;
                    newY  -= size.Y / 2;
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
                case Anchor.TopLeft:
                default:
                    break;
            }

            processedPosition.X = newX;
            processedPosition.Y = newY;
        }

        public override void move(Vector2 position)
        {
            base.move(position);
            Update(text.ToString());
        }

        public override void Draw()
        {
            Pulsarc.spriteBatch.DrawString(font, text, processedPosition, color, 0, origin, fontScale, SpriteEffects.None, 0);
        }
    }
}
