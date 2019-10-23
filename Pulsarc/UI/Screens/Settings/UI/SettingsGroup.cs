using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class SettingsGroup : Drawable
    {
        public string name;
        public Drawable icon;
        public Dictionary<String, Setting> settings;
        public Setting focusedHoldSetting;

        public SettingsGroup(string name, Vector2 position)
        {
            this.name = name;
            settings = new Dictionary<String, Setting>();
            icon = new Drawable(Skin.assets["settings_icon_" + name.ToLower()], new Vector2(position.X + 250, position.Y + 250), new Vector2(200,200),anchor: Anchor.Center);
            changePosition(position);
            drawnPart = new Rectangle(new Point((int) position.X, (int) position.Y), new Point(500, 500));
        }

        public void addSetting(string key, Setting setting)
        {
            settings.Add(key, setting);
            drawnPart.Height += setting.drawnPart.Height;
        }

        public override void move(Vector2 position, bool scaledPositioning = true)
        {
            base.move(position, scaledPositioning);
            icon.move(position, scaledPositioning);

            foreach(KeyValuePair<string, Setting> settp in settings)
            {
                settp.Value.move(position);
            }
        }

        public Vector2 getNextPosition()
        {
            return new Vector2(truePosition.X, truePosition.Y + drawnPart.Height);
        }

        public override void Draw()
        {
            icon.Draw();

            foreach(KeyValuePair<string, Setting> entry in settings)
            {
                entry.Value.Draw();
            }
        }

        public void resetFocusedHoldSetting()
        {
            focusedHoldSetting = null;
        }

        public void onClick(Point mousePosition, bool hold)
        {
            foreach (KeyValuePair<string, Setting> entry in settings)
            {
                // Check if this setting corresponds to the current input type
                if(entry.Value.hold == hold)
                {
                    // Single Input
                    if(!hold)
                    {
                        if (entry.Value.clicked(mousePosition))
                        {
                            entry.Value.onClick(mousePosition);
                        }
                    } else
                    // Hold Input
                    {
                        // Start to hold this item, keep it in memory
                        if (entry.Value.clicked(mousePosition) && focusedHoldSetting == null)
                        {
                            focusedHoldSetting = entry.Value;
                        }

                        // Continue to update the held item, even if out of range
                        if (focusedHoldSetting == entry.Value)
                        {
                            entry.Value.onClick(mousePosition);
                        }
                    }
                }
            }
        }

        public void Save()
        {
            foreach (KeyValuePair<string, Setting> entry in settings)
            {
                entry.Value.Save(name, entry.Key);
            }
        }
    }
}
