using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Settings
{
    class SettingsScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        public float selectedFocus = 0;
        int lastScrollValue = 0;

        public override void Init()
        {
            View = new SettingsScreenView(this);
        }

        public override void Update(GameTime gameTime)
        {
            handleMouseInput();

            View?.Update(gameTime);
        }

        private void changeFocus(MouseState ms)
        {
            // If the scroll wheel's state has changed, change the focus
            if (ms.ScrollWheelValue < lastScrollValue)
            {
                selectedFocus -= 0.3f;
            }
            else if (ms.ScrollWheelValue > lastScrollValue)
            {
                selectedFocus += 0.3f;
            }

            lastScrollValue = ms.ScrollWheelValue;
        }

        private void handleMouseInput()
        {
            MouseState ms = Mouse.GetState();

            changeFocus(ms);
        }
    }
}
