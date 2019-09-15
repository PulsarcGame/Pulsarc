using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Settings.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Settings
{
    class SettingsScreenView : ScreenView
    {
        SettingsScreen GetSettingsScreen() { return (SettingsScreen)Screen; }

        Background background;
        ReturnButton button_back;
        SaveButton button_save;

        public List<SettingsGroup> groups;

        float currentFocus = 0;
        float lastFocus = 0;

        public SettingsScreenView(Screen screen) : base(screen)
        {
            background = new Background("settings_background");
            button_back = new ReturnButton("settings_button_back", new Vector2(0, 1080));
            button_save = new SaveButton("settings_button_save", new Vector2(1920, 1080));

            groups = new List<SettingsGroup>();

            groups.Add(new GameplaySettings(new Vector2(100, 100)));
            groups.Add(new AudioSettings(new Vector2(100, 1000)));
        }

        public override void Destroy()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw();
            
            foreach(SettingsGroup settingsGroup in groups)
            {
                settingsGroup.Draw();
            }

            button_back.Draw();
            button_save.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            float selectedFocus = GetSettingsScreen().selectedFocus;

            // Move Settings Groups if focus has changed.
            if (currentFocus != selectedFocus)
            {
                currentFocus = PulsarcMath.Lerp(currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);

                float diff = lastFocus - currentFocus;
                lastFocus = currentFocus;

                foreach (SettingsGroup settings in groups)
                {
                    settings.move(new Vector2(0, 200 * diff));
                }
            }

            while (InputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.keyboardPresses.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                {
                    ScreenManager.RemoveScreen(true);
                }
            }

            // Check if we released a previously held item
            // TODO: only check when there was a held item
            if(MouseManager.CurrentState.LeftButton == ButtonState.Released)
            {
                foreach (SettingsGroup settingsGroup in groups)
                {
                    if (settingsGroup.focusedHoldSetting != null)
                    {
                        settingsGroup.focusedHoldSetting = null;
                    }
                }
            }

            // Handle Single click inputs
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
                foreach (SettingsGroup settingsGroup in groups)
                {
                    if (settingsGroup.clicked(MouseManager.CurrentState.Position))
                    {
                        settingsGroup.onClick(MouseManager.CurrentState.Position, false);
                    }
                }
            }

            // Handle holding mouse inputs
            if(MouseManager.CurrentState.LeftButton == ButtonState.Pressed)
            {
                foreach (SettingsGroup settingsGroup in groups)
                {
                    if (settingsGroup.clicked(MouseManager.CurrentState.Position))
                    {
                        settingsGroup.onClick(MouseManager.CurrentState.Position, true);
                    }
                }
            }
        }
    }
}
