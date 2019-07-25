using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Button : Drawable
    {
        Icon icon;

        int rightPadding;

        public Button(string iconName, int rightPadding = 10) : base(Skin.assets["result_button"])
        {
            // Left-Center anchor
            icon = new Icon(iconName);

            this.rightPadding = rightPadding;
            updatePosition(new Vector2(0, 0));
        }

        public void updatePosition(Vector2 newPosition)
        {
            icon.changePosition(new Vector2(newPosition.X + texture.Width - icon.texture.Width - rightPadding, newPosition.Y + texture.Height / 2 - icon.texture.Height / 2));
            changePosition(newPosition);
        }

        public override void Draw()
        {
            base.Draw();
            icon.Draw();
        }
    }
}
