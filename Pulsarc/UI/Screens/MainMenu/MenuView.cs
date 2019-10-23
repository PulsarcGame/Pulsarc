using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
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
        Background background;

        // The logo used for the main menu.
        GameIcon gameIcon;

        // The buttons used to navigate from the main menu.
        public List<NavigationButton> navButtons;

        /// <summary>
        /// The Main Menu of Pulsarc
        /// </summary>
        /// <param name="screen">The screen to draw on.</param>
        public MenuView(Screen screen) : base(screen)
        {
            setUpBackgroundAndIcon();
            setUpNavigationButtons();
        }

        private void setUpBackgroundAndIcon()
        {
            background = new Background("menu_background");

            gameIcon = new GameIcon(Skin.getConfigStartPosition("main_menu", "Properties", "IconStartPos"), getSkinnablePropertyAnchor("IconAnchor"));

            Vector2 offset = new Vector2(
                getSkinnablePropertyInt("IconX"),
                getSkinnablePropertyInt("IconY"));

            gameIcon.move(offset);
        }

        private void setUpNavigationButtons()
        {
            navButtons = new List<NavigationButton>();
            addNavButton(Pulsarc.songScreen, "Play");
            addNavButton(new InProgressScreen(), "Multi");
            addNavButton(new InProgressScreen(), "Editor");
            addNavButton(new SettingsScreen(), "Settings");
            addNavButton(new QuitScreen(), "Quit");
        }

        /// <summary>
        /// Find a float from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePropertyFloat(string key)
        {
            return Skin.getConfigFloat("main_menu", "Properties", key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePropertyInt(string key)
        {
            return Skin.getConfigInt("main_menu", "Properties", key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePropertyAnchor(string key)
        {
            return Skin.getConfigAnchor("main_menu", "Properties", key);
        }

        /// <summary>
        /// Find a string from the Properties section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string getSkinnablePropertyString(string key)
        {
            return Skin.getConfigString("main_menu", "Properties", key);
        }

        /// <summary>
        /// Add a navigation button to the main menu, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="screen">The Screen the nav button will move to.</param>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        private void addNavButton(PulsarcScreen screen, string typeName)
        {
            // Find variables for TDE
            string textStr = getSkinnablePropertyString(typeName + "Text"); // string text
            Vector2 position = Skin.getConfigStartPosition("main_menu", "Properties", typeName + "StartPos"); // Vector2 position;
            int fontSize = getSkinnablePropertyInt(typeName + "TextFontSize");
            Anchor textAnchor = getSkinnablePropertyAnchor(typeName + "TextAnchor"); // Anchor textAnchor;
            Color textColor = Skin.getConfigColor("main_menu", "Properties", typeName + "TextColor"); // Color textColor;

            // Make TDE
            TextDisplayElement text = new TextDisplayElement(textStr, position, fontSize, textAnchor, textColor);

            // Make NavButton
            NavigationButton navButton = new NavigationButton(screen, getSkinnablePropertyInt(typeName + "Type"), position, text);

            // Offset
            Vector2 offset = new Vector2(
                getSkinnablePropertyInt(typeName + "X"),
                getSkinnablePropertyInt(typeName + "Y"));

            navButton.move(offset);

            // Add
            navButtons.Add(navButton);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.isLeftClick())
            {
                Point pos = InputManager.lastMouseClick.Key.Position;
                foreach (NavigationButton button in navButtons)
                {
                    if (button.clicked(pos))
                    {
                        button.navigate();
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw();
            foreach(NavigationButton button in navButtons)
            {
                button.Draw();
            }
            gameIcon.Draw();
        }

        public override void Destroy()
        {
        }
    }
}
