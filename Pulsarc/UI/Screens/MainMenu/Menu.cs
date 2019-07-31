using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Screens.MainMenu.UI;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.MainMenu
{
    class Menu : Screen
    {

        bool leftClicking = false;
        Vector2 leftClickingPos;
        public override ScreenView View { get; protected set; }
        MenuView GetMenuView() { return (MenuView)View; }

        public Menu()
        {
            View = new MenuView(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (MouseManager.IsUniqueClick(MouseButton.Left))
            {
                foreach (NavigationButton button in GetMenuView().navButtons)
                {
                    if (button.clicked(MouseManager.CurrentState.Position))
                    {
                        button.navigate();
                    }
                }
            }

            View.Update(gameTime);
        }
    }
}
