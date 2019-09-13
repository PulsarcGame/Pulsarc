using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public abstract class Setting : Drawable
    {
        public dynamic value;
        public string type;
        public string text;
        public TextDisplayElement title;
        public TextDisplayElement more;

        public Setting(string title, string more, Vector2 position, Texture2D texture, float aspect, Anchor anchor, dynamic baseValue, string type) : base(texture, aspect, anchor)
        {
            value = baseValue;
            this.type = type;
            Console.WriteLine(type.ToString() + " set for " + title);
            this.text = title;
            this.title = new TextDisplayElement(title, position);
            changePosition(position);
        }
        public abstract void onClick(Point mousePosition);

        public override void Draw()
        {
            base.Draw();
            title.Draw();
        }

        public virtual void Save(string category, string key)
        {
            switch(type)
            {
                case "float":
                    Config.setFloat(category, key, getSaveValue());
                    break;
                case "int":
                    Config.setInt(category, key, getSaveValue());
                    break;
                case "double":
                    Config.setDouble(category, key, getSaveValue());
                    break;
                case "bool":
                    Config.setBool(category, key, getSaveValue());
                    break;
                default:
                    Console.WriteLine("Cannot save type " + type.ToString() + " in category "+category+" for setting "+key);
                    break;
            }
        }

        public abstract dynamic getSaveValue();
    }
}
