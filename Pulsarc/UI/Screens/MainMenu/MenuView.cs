using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.MainMenu.UI;
using Pulsarc.UI.Screens.Quit;
using Pulsarc.UI.Screens.Settings;
using Pulsarc.UI.Screens.SongSelect;
using Pulsarc.Utils;
using Wobble.Input;
using Wobble.Logging;
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

            gameIcon = new GameIcon(Skin.getStartPosition("main_menu", "Positions", "IconStartPos"), getSkinnablePositionAnchor("IconAnchor"));

            Vector2 offset = new Vector2(
                getSkinnablePositionInt("IconX"),
                getSkinnablePositionInt("IconY"));

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
        /// Find a float position from the Position section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePositionFloat(string key)
        {
            return Skin.getConfigFloat("main_menu", "Positions", key);
        }

        /// <summary>
        /// Find a int from the Position section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePositionInt(string key)
        {
            return Skin.getConfigInt("main_menu", "Positions", key);
        }

        /// <summary>
        /// Find an Anchor from the Position section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePositionAnchor(string key)
        {
            return Skin.getConfigAnchor("main_menu", "Positions", key);
        }

        /// <summary>
        /// Find a string from the Position section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string getSkinnablePositionString(string key)
        {
            return Skin.getConfigString("main_menu", "Positions", key);
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
            string textStr = getSkinnablePositionString(typeName +"Text"); // string text
            Vector2 position = Skin.getStartPosition("main_menu", "Positions", typeName + "StartPos"); // Vector2 position;
            int fontSize = getSkinnablePositionInt(typeName + "TextFontSize");
            Anchor textAnchor = getSkinnablePositionAnchor(typeName + "TextAnchor"); // Anchor textAnchor;
            Color textColor = Skin.getColor("main_menu", "Positions", typeName + "TextColor"); // Color textColor;

            // Make TDE
            TextDisplayElement text = new TextDisplayElement(textStr, position, fontSize, textAnchor, textColor);

            // Make NavButton
            NavigationButton navButton = new NavigationButton(screen, getSkinnablePositionInt(typeName + "Type"), position, text);

            // Offset
            Vector2 offset = new Vector2(
                getSkinnablePositionInt(typeName + "X"),
                getSkinnablePositionInt(typeName + "Y"));

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
