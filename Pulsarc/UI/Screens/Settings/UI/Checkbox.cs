using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Settings.UI
{
    class Checkbox : Setting
    {
        Drawable cross;
        public Checkbox(string title, string more, Vector2 position, string type, bool startingValue) : 
            base(title, more, position, Skin.assets["settings_checkbox"], -1, Anchor.TopLeft, startingValue, type)
        {
            cross = new Drawable(Skin.assets["settings_checkbox_cross"], position);

            cross.changePosition(position);
        }
        public override dynamic getSaveValue()
        {
            return value;
        }

        public override void onClick(Point mousePosition)
        {
            value = !(bool) value;
            Console.WriteLine("click " + value);
        }

        public override void move(Vector2 position)
        {
            base.move(position);
            cross.move(position);
        }

        public override void Draw()
        {
            base.Draw();
            if(value)
            {
                cross.Draw();
            }
        }
    }
}
