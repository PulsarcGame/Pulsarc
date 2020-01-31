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

        private Background background;
        private ReturnButton button_back;
        private SaveButton button_save;

        public List<SettingsGroup> Groups { get; private set; }

        private float currentFocus = 0;
        private float lastFocus = 0;

        public SettingsScreenView(Screen screen) : base(screen)
        {
            background = new Background("settings_background");
            button_back = new ReturnButton("settings_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
            button_save = new SaveButton("settings_button_save", AnchorUtil.FindScreenPosition(Anchor.BottomRight));

            Groups = new List<SettingsGroup>();

            Groups.Add(new GameplaySettings(new Vector2(400, GetNextGroupPos())));
            Groups.Add(new AudioSettings(new Vector2(400, GetNextGroupPos())));
            Groups.Add(new BindingsSettings(new Vector2(400, GetNextGroupPos())));
        }

        public override void Destroy()
        { }

        /// <summary>
        /// Get the position for the next SettingsGroup to use.
        /// </summary>
        /// <returns></returns>
        public int GetNextGroupPos()
        {
            int posY = 0;
             
            foreach(SettingsGroup group in Groups)
                posY = (int) group.GetNextPosition().Y;

            return posY;
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw();
            
            foreach(SettingsGroup settingsGroup in Groups)
                settingsGroup.Draw();

            button_back.Draw();
            button_save.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            // Move Settings Groups if focus has changed.
            updateFocus();

            listenForKeys();

            // Check if we released a previously held item
            // TODO: only check when there was a held item
            checkHeldItem();

            // Handle Single click inputs
            handleSingleClicks();

            // Handle holding mouse inputs
            handleMouseHoldInput();
        }

        private void updateFocus()
        {
            float selectedFocus = GetSettingsScreen().SelectedFocus;

            if (currentFocus != selectedFocus)
            {
                currentFocus = PulsarcMath.Lerp(currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);

                float diff = lastFocus - currentFocus;
                lastFocus = currentFocus;

                foreach (SettingsGroup settings in Groups)
                    settings.Move(new Vector2(0, 200 * diff));
            }
        }

        private void listenForKeys()
        {
            while (InputManager.PressActions.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.PressActions.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                    ScreenManager.RemoveScreen(true);

                foreach (SettingsGroup settingsGroup in Groups)
                    foreach (KeyValuePair<string, Setting> settingP in settingsGroup.Settings)
                        if (settingP.Value.KeyListen)
                            settingP.Value.HandleKeyEvent(press.Value);
            }
        }

        private void checkHeldItem()
        {
            if (MouseManager.CurrentState.LeftButton == ButtonState.Released)
                foreach (SettingsGroup settingsGroup in Groups)
                    if (settingsGroup.FocusedHoldSetting != null)
                        settingsGroup.ResetFocusedHoldSetting();
        }

        private void handleSingleClicks()
        {
            Point pos = InputManager.LastMouseClick.Key.Position;

            if (InputManager.IsLeftClick())
            {
                if (button_back.Hovered(pos))
                    button_back.OnClick();

                if (button_save.Hovered(pos))
                    button_save.OnClick(this);

                foreach (SettingsGroup settingsGroup in Groups)
                    if (settingsGroup.Hovered(pos))
                        settingsGroup.OnClick(pos, false);
            }
        }

        private void handleMouseHoldInput()
        {
            Point pos = InputManager.MouseState.Position;

            if (MouseManager.CurrentState.LeftButton == ButtonState.Pressed)
                foreach (SettingsGroup settingsGroup in Groups)
                    if (settingsGroup.Hovered(pos))
                        settingsGroup.OnClick(pos, true);
        }
    }
}
