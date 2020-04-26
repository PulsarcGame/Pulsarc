using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Settings.UI
{
    class SaveButton : Drawable
    {
        /// <summary>
        /// A button on the settings screen that saves current settings state to Config
        /// </summary>
        /// <param name="skinAsset"></param>
        /// <param name="position">The position of the Save Button.</param>
        /// <param name="anchor">The anchor of the Save Button. Defaults to BottomRight instead of TopLeft.</param>
        public SaveButton(string skinAsset, Vector2 position, Anchor anchor = Anchor.BottomRight)
            : base(Skin.Assets[skinAsset], position, anchor: anchor)
        { }

        /// <summary>
        /// This method is called when the SaveButton is clicked, initiating the saving process for all settings groups.
        /// </summary>
        /// <param name="settings">The Settings View containing the current settings states.</param>
        public void OnClick(SettingsScreenView settings)
        {
            foreach (SettingsGroup group in settings.Groups)
                group.Save();

            Config.SaveConfig();
        }
    }
}
