using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class SettingsGroup : Drawable
    {
        public Drawable icon;
        public List<Setting> settings;

        public SettingsGroup(string name, Vector2 position)
        {
            settings = new List<Setting>();
            icon = new Drawable(Skin.assets["settings_icon_" + name], new Vector2(position.X + 250, position.Y + 250), new Vector2(200,200),anchor: Anchor.Center);
            changePosition(position);
            drawnPart = new Rectangle(new Point((int) position.X, (int) position.Y), new Point(500, 500));
        }

        public void addSetting(Setting setting)
        {
            settings.Add(setting);
            drawnPart.Height += setting.drawnPart.Height;
        }

        public Vector2 getNextPosition()
        {
            return new Vector2(position.X, position.Y + drawnPart.Height);
        }

        public override void Draw()
        {
            icon.Draw();

            foreach(Setting setting in settings)
            {
                setting.Draw();
            }
        }

        public void onClick(Point mousePosition)
        {
            foreach(Setting setting in settings)
            {
                if(setting.clicked(mousePosition))
                {
                    setting.onClick(mousePosition);
                }
            }
        }
    }
}
