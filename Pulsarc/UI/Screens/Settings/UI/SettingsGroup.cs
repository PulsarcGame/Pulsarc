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
        // Name of this group
        public string Name { get; protected set; }
        
        // The icon to use for this group.
        public Drawable Icon { get; protected set; }
        
        // Each setting and the key they change
        public Dictionary<string, Setting> Settings { get; protected set; }
        
        //
        public Setting FocusedHoldSetting { get; protected set; }

        public SettingsGroup(string name, Vector2 position)
        {
            Name = name;
            Settings = new Dictionary<string, Setting>();
            Icon = new Drawable(Skin.Assets["settings_icon_" + name.ToLower()], new Vector2(position.X + 250, position.Y + 250), new Vector2(200,200),anchor: Anchor.Center);
            ChangePosition(position);
            drawnPart = new Rectangle(new Point((int) position.X, (int) position.Y), new Point(500, 500));
        }

        /// <summary>
        /// Add a new setting to this group.
        /// </summary>
        /// <param name="key">The Key to modify in the config.</param>
        /// <param name="setting">The setting object itself.</param>
        public void AddSetting(string key, Setting setting)
        {
            Settings.Add(key, setting);

            drawnPart.Height += setting.drawnPart.Height;
        }

        public override void Move(Vector2 position, bool scaledPositioning = true)
        {
            base.Move(position, scaledPositioning);
            Icon.Move(position, scaledPositioning);

            foreach (KeyValuePair<string, Setting> settp in Settings)
                settp.Value.Move(position);
        }

        /// <summary>
        /// Get the next position for a setting to use.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNextPosition()
        {
            return new Vector2(truePosition.X, truePosition.Y + drawnPart.Height);
        }

        public override void Draw()
        {
            Icon.Draw();

            foreach (KeyValuePair<string, Setting> entry in Settings)
                entry.Value.Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetFocusedHoldSetting()
        {
            FocusedHoldSetting = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="hold"></param>
        public void OnClick(Point mousePosition, bool hold)
        {
            foreach (KeyValuePair<string, Setting> entry in Settings)
            {
                // Check if this setting corresponds to the current input type
                if (entry.Value.Hold == hold)
                {
                    // Single Input
                    if (!hold)
                    {
                        if (entry.Value.Hovered(mousePosition))
                            entry.Value.OnClick(mousePosition);
                    }
                    // Hold Input
                    else
                    {
                        // Start to hold this item, keep it in memory
                        if (entry.Value.Hovered(mousePosition) && FocusedHoldSetting == null)
                            FocusedHoldSetting = entry.Value;

                        // Continue to update the held item, even if out of range
                        if (FocusedHoldSetting == entry.Value)
                            entry.Value.OnClick(mousePosition);
                    }
                }
            }
        }

        /// <summary>
        /// Save all the settings into the config.ini
        /// </summary>
        public void Save()
        {
            foreach (KeyValuePair<string, Setting> entry in Settings)
                entry.Value.Save(Name, entry.Key);
        }
    }
}
