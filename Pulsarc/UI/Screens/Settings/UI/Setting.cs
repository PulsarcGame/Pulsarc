using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public abstract class Setting : Drawable
    {
        public dynamic value;
        public TextDisplayElement title;
        public TextDisplayElement more;

        public Setting(string title, string more, Vector2 position, Texture2D texture, float aspect, Anchor anchor) : base(texture, aspect, anchor)
        {
            this.title = new TextDisplayElement(title, position);
            changePosition(position);
        }
        public abstract void onClick(Point mousePosition);

        public override void Draw()
        {
            base.Draw();
            title.Draw();
        }
    }
}
