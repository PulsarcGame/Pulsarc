using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI
{
    public class Cursor : Drawable
    {
        public Cursor() : base(Skin.assets["cursor"])
        {
            Resize(texture.Width / (texture.Width / (48 / (float) 1920 * Pulsarc.getDimensions().X)));
        }

        public void SetPos(Point position)
        {
            this.drawPosition.X = position.X;
            this.drawPosition.Y = position.Y;
            this.position = drawPosition;
        }

        public void SetPos(Vector2 position)
        {
            this.drawPosition.X = position.X;
            this.drawPosition.Y = position.Y;
            this.position = drawPosition;
        }
    }
}