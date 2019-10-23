using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI
{
    public class Cursor : Drawable
    {
        public float cursorScale = 1; //TODO: Make this adjustable by the user

        public Cursor() : base(Skin.assets["cursor"])
        {
            Resize(30 * cursorScale);
        }

        public void setPos(Point position)
        {
            setPos(new Vector2(position.X, position.Y));
        }

        public void setPos(Vector2 position)
        {
            this.truePosition = position;
        }
    }
}
