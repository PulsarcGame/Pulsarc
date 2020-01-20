using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI
{
    public sealed class Cursor : Drawable
    {
        //TODO: Make this adjustable by the user
        private float cursorScale = 1;

        public Cursor() : base(Skin.Assets["cursor"])
        {
            Resize(30 * cursorScale);
        }

        public void SetPos(Point position)
        {
            SetPos(new Vector2(position.X, position.Y));
        }

        private void SetPos(Vector2 position)
        {
            TruePosition = position;
        }
    }
}
