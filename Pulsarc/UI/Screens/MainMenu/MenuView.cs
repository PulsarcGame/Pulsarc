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

            gameIcon = new GameIcon(Skin.getStartPosition("main_menu", "Positions", "IconStartPos"));

            Vector2 offset = new Vector2(
                getSkinnablePositionInt("IconX"),
                getSkinnablePositionInt("IconY"));

            gameIcon.move(offset);
        }

        private void setUpNavigationButtons()
        {
            navButtons = new List<NavigationButton>();
            addNavButton(Pulsarc.songScreen, "Play", "Play Game");
            addNavButton(new InProgressScreen(), "Multi", "Multiplayer");
            addNavButton(new InProgressScreen(), "Editor", "Editor");
            addNavButton(new SettingsScreen(), "Settings", "Settings");
            addNavButton(new QuitScreen(), "Quit", "Quit");
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
        /// Find a NavButtonType from the Position section of the Main Menu config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The NavButtonType of the key provided.</returns>
        private NavButtonType getSkinnableNavButtonType(string key)
        {
            return (NavButtonType)getSkinnablePositionInt(key);
        }

        private void addNavButton(PulsarcScreen screen, string typeName, string text)
        {
            NavigationButton navButton = new NavigationButton(  screen, 
                                                                getSkinnableNavButtonType(typeName + "Type"),
                                                                text, Skin.getStartPosition("main_menu",
                                                                "Positions",
                                                                typeName + "StartPos"));

            Vector2 offset = new Vector2(
                getSkinnablePositionInt(typeName + "X"),
                getSkinnablePositionInt(typeName + "Y"));

            navButton.move(offset);

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
