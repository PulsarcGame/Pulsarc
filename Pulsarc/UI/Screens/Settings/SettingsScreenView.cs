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
        SaveButton button_save;

        public List<SettingsGroup> groups;

        public SettingsScreenView(Screen screen) : base(screen)
        {
            background = new Background("settings_background");
            button_back = new ReturnButton("settings_button_back", new Vector2(0, 1080));
            button_save = new SaveButton("settings_button_save", new Vector2(1920, 1080));

            groups = new List<SettingsGroup>();

            groups.Add(new GameplaySettings(new Vector2(100,100)));
        }

        public override void Destroy()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw();
            button_back.Draw();
            button_save.Draw();
            
            foreach(SettingsGroup settingsGroup in groups)
            {
                settingsGroup.Draw();
            }
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
                if (button_save.clicked(MouseManager.CurrentState.Position))
                {
                    button_save.onClick(this);
                }
            }

            if(MouseManager.CurrentState.LeftButton == ButtonState.Pressed)
            {
                foreach (SettingsGroup settingsGroup in groups)
                {
                    if (settingsGroup.clicked(MouseManager.CurrentState.Position))
                    {
                        settingsGroup.onClick(MouseManager.CurrentState.Position);
                    }
                }
            }
        }
    }
}
