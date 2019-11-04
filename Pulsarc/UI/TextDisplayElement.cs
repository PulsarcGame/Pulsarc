using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System.Text;
using Wobble.Logging;

namespace Pulsarc.UI
{
    public class TextDisplayElement : Drawable
    {
        public string Name { get; set; }

        // Text data
        private SpriteFont font => AssetsManager.Fonts["DefaultFont"];
        private float fontScale;
        public StringBuilder Text { get; protected set; }
        public Color Color { get; set; }

        public Vector2 ProcessedPosition;

        private bool caught = false;

        /// <summary>
        /// A text-based Drawable.
        /// TODO: Fix positioning with different Anchors and resolutions/aspect ratios.
        /// </summary>
        /// <param name="name">The text to be displayed.</param>
        /// <param name="position">The position of this TDE</param>
        /// <param name="fontSize">The size of the text in pt. Default is 18.</param>
        /// <param name="anchor">The anchor of this TDE, Default is Anchor.TopLeft</param>
        /// <param name="color">The color of the text, default is White.</param>
        public TextDisplayElement(string name, Vector2 position, int fontSize = 18, Anchor anchor = Anchor.CenterLeft, Color? color = null)
        {
            Name = name;
            Anchor = anchor;
            Color = color ?? Color.White;
            Text = new StringBuilder(20);

            fontScale = fontSize / 64f * (Pulsarc.GetDimensions().Y / Pulsarc.BaseHeight);

            ChangePosition(position);
            ProcessedPosition = TruePosition;
            Update("");
        }

        /// <summary>
        /// Update the text with new text added to name.
        /// </summary>
        /// <param name="value">The text to change to</param>
        public void Update(string value)
        {
            Text.Clear();
            Text.Append(Name).Append(value);
            ReprocessPosition();

            caught = false;
        }

        /// <summary>
        /// Reprocess the position of this TDE.
        /// </summary>
        public void ReprocessPosition()
        {

            float newX = TruePosition.X;
            float newY = TruePosition.Y;
            Vector2 size = font.MeasureString(Text) * fontScale;
            size.X *= Pulsarc.GetDimensions().X / Pulsarc.BaseWidth;
            size.Y *= Pulsarc.GetDimensions().Y / Pulsarc.BaseHeight;

            switch (Anchor)
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

            ProcessedPosition.X = newX;
            ProcessedPosition.Y = newY;
        }

        public override void Move(Vector2 position, bool scaledPositioning = true)
        {
            float x = position.X / Pulsarc.WidthScale;
            float y = position.Y * Pulsarc.HeightScale;

            base.Move(new Vector2(x, y), scaledPositioning);
            ReprocessPosition();
        }

        public override void Draw()
        {
            if (caught)
                return;

            try
            {
                Pulsarc.SpriteBatch.DrawString(font, Text, ProcessedPosition, Color, 0, Origin, fontScale, SpriteEffects.None, 0);
            }
            catch
            {
                Logger.Error($"Could not write {Text}, Aborting Draw() calls for this TextDisplayElement until Update() has been called.", LogType.Runtime);
                caught = true; // Don't need to spam the console with Errors
            }
        }
    }
}
