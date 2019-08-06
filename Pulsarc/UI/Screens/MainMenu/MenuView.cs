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
using Wobble.Screens;

namespace Pulsarc.UI.Screens.MainMenu
{
    class MenuView : ScreenView
    {

        Background background;
        GameIcon gameIcon;

        public List<NavigationButton> navButtons;


        public MenuView(Screen screen) : base(screen)
        {
            background = new Background("menu_background");

            gameIcon = new GameIcon(new Vector2(GetSkinnableInt("IconX"), GetSkinnableInt("IconY")));

            navButtons = new List<NavigationButton>();
            navButtons.Add(new NavigationButton(new SongSelection(), GetSkinnableInt("PlayType"), "Play Game", new Vector2(GetSkinnableInt("PlayX"), GetSkinnableInt("PlayY"))));
            navButtons.Add(new NavigationButton(new InProgressScreen(), GetSkinnableInt("MultiType"), "Multiplayer", new Vector2(GetSkinnableInt("MultiX"), GetSkinnableInt("MultiY"))));
            navButtons.Add(new NavigationButton(new InProgressScreen(), GetSkinnableInt("EditorType"), "Editor", new Vector2(GetSkinnableInt("EditorX"), GetSkinnableInt("EditorY"))));
            navButtons.Add(new NavigationButton(new SettingsScreen(), GetSkinnableInt("SettingsType"), "Settings", new Vector2(GetSkinnableInt("SettingsX"), GetSkinnableInt("SettingsY"))));
            navButtons.Add(new NavigationButton(new QuitScreen(), GetSkinnableInt("QuitType"), "Quit", new Vector2(GetSkinnableInt("QuitX"), GetSkinnableInt("QuitY"))));
        }

        private float GetSkinnablePosition(string key)
        {
            return Skin.GetConfigFloat("main_menu", "Positions", key);
        }

        private int GetSkinnableInt(string key)
        {
            return Skin.GetConfigInt("main_menu", "Positions", key);
        }

        private Anchor GetSkinnableAnchor(string key)
        {
            return Skin.GetConfigAnchor("main_menu", "Positions", key);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.IsLeftClick())
            {
                Point pos = InputManager.lastMouseClick.Key.Position;
                foreach (NavigationButton button in navButtons)
                {
                    if (button.Clicked(pos))
                    {
                        button.Navigate();
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