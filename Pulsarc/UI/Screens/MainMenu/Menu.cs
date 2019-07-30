using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Screens.MainMenu.UI;
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
            MouseState ms = Mouse.GetState();

            if (!leftClicking && ms.LeftButton == ButtonState.Pressed)
            {
                leftClicking = true;
                leftClickingPos = new Vector2(ms.Position.X, ms.Position.Y);
            }
            else if (leftClicking && ms.LeftButton == ButtonState.Released)
            {
                leftClicking = false;
                Vector2 leftReleasePos = new Vector2(ms.Position.X, ms.Position.Y);
                foreach (NavigationButton button in GetMenuView().navButtons)
                {
                    if (button.clicked(leftClickingPos) && button.clicked(leftReleasePos))
                    {
                        button.navigate();
                    }
                }
            }
            else
            {
                View.Update(gameTime);
            }
        }
    }
}
