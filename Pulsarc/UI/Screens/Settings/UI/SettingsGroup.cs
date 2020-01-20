using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class SettingsGroup : Drawable
    {
        // Name of this group
        protected string Name { get; set; }
        
        // The icon to use for this group.
        private Drawable Icon { get; set; }
        
        // Each setting and the key they change
        public Dictionary<string, Setting> Settings { get; protected set; }
        
        //
        public Setting FocusedHoldSetting { get; protected set; }

        protected SettingsGroup(string name, Vector2 position)
        {
            Name = name;
            Settings = new Dictionary<string, Setting>();
            Icon = new Drawable(Skin.Assets["settings_icon_" + name.ToLower()], new Vector2(position.X + 250, position.Y + 250), new Vector2(200,200),anchor: Anchor.Center);
            ChangePosition(position);
            DrawnPart = new Rectangle(new Point((int) position.X, (int) position.Y), new Point(500, 500));
        }

        /// <summary>
        /// Add a new setting to this group.
        /// </summary>
        /// <param name="key">The Key to modify in the config.</param>
        /// <param name="setting">The setting object itself.</param>
        internal void AddSetting(string key, Setting setting)
        {
            Settings.Add(key, setting);

            DrawnPart.Height += setting.DrawnPart.Height;
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            Icon.Move(delta, heightScaled);

            foreach (KeyValuePair<string, Setting> settp in Settings)
                settp.Value.Move(delta, heightScaled);
        }

        /// <summary>
        /// Get the next position for a setting to use.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNextPosition()
        {
            return new Vector2(TruePosition.X, TruePosition.Y + DrawnPart.Height);
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
            foreach (var entry in Settings.Where(entry => entry.Value.Hold == hold))
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
