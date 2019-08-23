using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Settings.UI;
using Pulsarc.Utils;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Settings
{
    class SettingsScreenView : ScreenView
    {
        Background background;
        ReturnButton button_back;

        Slider testSlider;

        public SettingsScreenView(Screen screen) : base(screen)
        {
            background = new Background("settings_background");
            button_back = new ReturnButton("settings_button_back", new Vector2(0, 1080));

            testSlider = new Slider();
            testSlider.changePosition(Pulsarc.xBaseRes / 2 - testSlider.currentSize.X / 2, Pulsarc.yBaseRes / 2);
        }

        public override void Destroy()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw();
            button_back.Draw();
            testSlider.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            while (InputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.keyboardPresses.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                {
                    ScreenManager.RemoveScreen(true);
                }
            }
            if (MouseManager.IsUniqueClick(MouseButton.Left))
            {
                if (button_back.clicked(MouseManager.CurrentState.Position))
                {
                    button_back.onClick();
                }
            }
        }
    }
}
