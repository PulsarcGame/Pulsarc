using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;

namespace Pulsarc.UI
{
    public enum Anchor
    {
        Center = 0,
        TopRight = 1,
        CenterRight = 2,
        BottomRight = 3,
        CenterLeft = 4,
        TopLeft = 5,
    }
    class TextDisplayElement : Drawable
    {
        Anchor anchor;
        string name;

        SpriteFont font;
        string text;
        float fontScale;
        Color color;

        Vector2 processedPosition;

        public TextDisplayElement(string name, Vector2 position, int fontSize = 18, Anchor anchor = Anchor.TopLeft)
        {
            this.name = name;
            this.anchor = anchor;

            font = AssetsManager.fonts["DefaultFont"];
            color = Color.White;
            fontScale = fontSize / 64f * (Pulsarc.getDimensions().X / 1920);
            
            changePosition(position);
            processedPosition = this.position;
            Update("");
        }

        public void Update(string value)
        {
            text = name + value;

            float newX = position.X;
            float newY = position.Y;
            Vector2 size = font.MeasureString(text) * fontScale;
            size.X *= Pulsarc.getDimensions().X / 1920;
            size.Y *= Pulsarc.getDimensions().Y / 1080;

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
            Update(text);
        }

        public override void Draw()
        {
            Pulsarc.spriteBatch.DrawString(font, text, processedPosition, color, 0, origin, fontScale, SpriteEffects.None, 0);
        }
    }
}
