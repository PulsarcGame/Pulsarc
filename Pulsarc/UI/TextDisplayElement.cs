using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;

namespace Pulsarc.UI
{
    class TextDisplayElement : Drawable
    {
        bool centered;
        string name;

        SpriteFont font;
        string text;
        float fontScale;
        Color color;

        Vector2 processedPosition;

        public TextDisplayElement(string name, Vector2 position, int fontSize = 18, bool centered = false)
        {
            this.name = name;
            this.centered = centered;

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

            if(centered)
            {
                processedPosition.X = position.X - ((font.MeasureString(text).X * fontScale) / 2) * Pulsarc.getDimensions().X / 1920; 
                processedPosition.Y = position.Y - ((font.MeasureString(text).Y * fontScale) / 2) * Pulsarc.getDimensions().Y / 1080;
            } else
            {
                processedPosition.X = position.X;
                processedPosition.Y = position.Y;
            }
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
