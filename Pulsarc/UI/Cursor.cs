using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI
{
    public class Cursor : Drawable
    {
        //TODO: Make this adjustable by the user
        public float cursorScale = 1;

        public Cursor() : base(Skin.Assets["cursor"])
        {
            Resize(30 * cursorScale);
        }

        public void SetPos(Point position)
        {
            SetPos(new Vector2(position.X, position.Y));
        }

        public void SetPos(Vector2 position)
        {
            truePosition = position;
        }
    }
}
