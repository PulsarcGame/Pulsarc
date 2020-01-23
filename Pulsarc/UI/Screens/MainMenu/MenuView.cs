using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Screens.MainMenu.UI;
using Pulsarc.UI.Screens.Quit;
using Pulsarc.UI.Screens.Settings;
using Pulsarc.Utils;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.MainMenu
{
    class MenuView : ScreenView
    {
        // The background image of the main menu.
        private Background _background;

        // The logo used for the main menu.
        private GameIcon _gameIcon;

        // The buttons used to navigate from the main menu.
        private List<NavigationButton> _navButtons;

        private VersionCounter _version;

        /// <summary>
        /// The Main Menu of Pulsarc
        /// </summary>
        /// <param name="screen">The screen to draw on.</param>
        public MenuView(Screen screen) : base(screen)
        {
            SetUpBackgroundAndIcon();
            SetUpNavigationButtons();
        }

        private void SetUpBackgroundAndIcon()
        {
            _background = new Background("menu_background");

            _gameIcon = new GameIcon(
                Skin.GetConfigStartPosition("main_menu", "Properties", "IconStartPos"),
                GetSkinnablePropertyAnchor("IconAnchor"));

            Vector2 offset = new Vector2(
                GetSkinnablePropertyInt("IconX"),
                GetSkinnablePropertyInt("IconY"));

            _gameIcon.Move(offset);

            _version = new VersionCounter(AnchorUtil.FindScreenPosition(Anchor.BottomRight));
        }

        private void SetUpNavigationButtons()
        {
            _navButtons = new List<NavigationButton>();
            AddNavButton(Pulsarc.SongScreen, "Play");
            AddNavButton(new InProgressScreen(), "Multi");
            AddNavButton(new InProgressScreen(), "Editor");
            AddNavButton(new SettingsScreen(), "Settings");
            AddNavButton(new QuitScreen(), "Quit");
        }

        /// <summary>
        /// Find a float from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float GetSkinnablePropertyFloat(string key)
        {
            return Skin.GetConfigFloat("main_menu", "Properties", key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int GetSkinnablePropertyInt(string key)
        {
            return Skin.GetConfigInt("main_menu", "Properties", key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor GetSkinnablePropertyAnchor(string key)
        {
            return Skin.GetConfigAnchor("main_menu", "Properties", key);
        }

        /// <summary>
        /// Find a string from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string GetSkinnablePropertyString(string key)
        {
            return Skin.GetConfigString("main_menu", "Properties", key);
        }

        /// <summary>
        /// Add a navigation button to the main menu, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="screen">The Screen the nav button will move to.</param>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        private void AddNavButton(PulsarcScreen screen, string typeName)
        {
            // Find variables for TDE
            string textStr = GetSkinnablePropertyString($"{typeName}Text"); // string text
            Vector2 position = Skin.GetConfigStartPosition("main_menu", "Properties", $"{typeName}StartPos"); // Vector2 position;
            int fontSize = GetSkinnablePropertyInt($"{typeName}TextFontSize");
            Anchor textAnchor = GetSkinnablePropertyAnchor($"{typeName}TextAnchor"); // Anchor textAnchor;
            Color textColor = Skin.GetConfigColor("main_menu", "Properties", $"{typeName}TextColor"); // Color textColor;

            // Make TDE
            TextDisplayElement text = new TextDisplayElement(textStr, position, fontSize, textAnchor, textColor);

            // Make NavButton that uses the TDE
            NavigationButton navButton = new NavigationButton(screen, GetSkinnablePropertyInt($"{typeName}Type"), position, text);

            // Offset the button
            Vector2 offset = new Vector2(
                GetSkinnablePropertyInt($"{typeName}X"),
                GetSkinnablePropertyInt($"{typeName}Y"));

            navButton.Move(offset);

            // Add the button
            _navButtons.Add(navButton);
        }

        public override void Update(GameTime gameTime)
        {
            if (!InputManager.IsLeftClick()) return;
            Point pos = InputManager.LastMouseClick.Key.Position;

            foreach (var button in _navButtons.Where(button => button.Hovered(pos)))
                button.Navigate();
        }

        public override void Draw(GameTime gameTime)
        {
            _background.Draw();

            foreach(NavigationButton button in _navButtons)
                button.Draw();

            _gameIcon.Draw();

            _version.Draw();
        }

        public override void Destroy() { }
    }
}
