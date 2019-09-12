using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI
{
    public class Cursor : Drawable
    {
        public float cursorScale = 1; //TODO: Make this adjustable by the user

        public Cursor() : base(Skin.assets["cursor"])
        {
            Resize(30 * cursorScale, false); // Size does not scale with resolution
        }

        public void setPos(Point position)
        {
            setPos(new Vector2(position.X, position.Y));
        }

        public void setPos(Vector2 position)
        {
            this.drawPosition.X = position.X;
            this.drawPosition.Y = position.Y;
            this.position = drawPosition;
        }
    }
}
