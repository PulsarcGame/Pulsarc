using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Buttons;
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

        private readonly Background _background;
        private readonly ReturnButton _buttonBack;
        private readonly SaveButton _buttonSave;

        public List<SettingsGroup> Groups { get; private set; }

        private float _currentFocus;
        private float _lastFocus;

        public SettingsScreenView(Screen screen) : base(screen)
        {
            _background = new Background("settings_background");
            _buttonBack = new ReturnButton("settings_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
            _buttonSave = new SaveButton("settings_button_save", AnchorUtil.FindScreenPosition(Anchor.BottomRight));

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
        private int GetNextGroupPos()
        {
            int posY = 0;
             
            foreach(SettingsGroup group in Groups)
                posY = (int) group.GetNextPosition().Y;

            return posY;
        }

        public override void Draw(GameTime gameTime)
        {
            _background.Draw();
            
            foreach(SettingsGroup settingsGroup in Groups)
                settingsGroup.Draw();

            _buttonBack.Draw();
            _buttonSave.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            // Move Settings Groups if focus has changed.
            UpdateFocus();

            ListenForKeys();

            // Check if we released a previously held item
            // TODO: only check when there was a held item
            CheckHeldItem();

            // Handle Single click inputs
            HandleSingleClicks();

            // Handle holding mouse inputs
            HandleMouseHoldInput();
        }

        private void UpdateFocus()
        {
            float selectedFocus = GetSettingsScreen().SelectedFocus;

            if (_currentFocus == selectedFocus) return;
            _currentFocus = PulsarcMath.Lerp(_currentFocus, selectedFocus, (float)PulsarcTime.DeltaTime / 100f);

            float diff = _lastFocus - _currentFocus;
            _lastFocus = _currentFocus;

            foreach (SettingsGroup settings in Groups)
                settings.Move(new Vector2(0, 200 * diff));
        }

        private void ListenForKeys()
        {
            while (InputManager.KeyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.KeyboardPresses.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                    ScreenManager.RemoveScreen(true);

                foreach (var settingP in from settingsGroup in Groups from settingP in settingsGroup.Settings where settingP.Value.KeyListen select settingP)
                    settingP.Value.HandleKeyEvent(press.Value);
            }
        }

        private void CheckHeldItem()
        {
            if (MouseManager.CurrentState.LeftButton != ButtonState.Released) return;
            foreach (var settingsGroup in Groups.Where(settingsGroup => settingsGroup.FocusedHoldSetting != null))
                settingsGroup.ResetFocusedHoldSetting();
        }

        private void HandleSingleClicks()
        {
            if (MouseManager.IsUniqueClick(MouseButton.Left))
            {
                if (_buttonBack.Hovered(MouseManager.CurrentState.Position))
                    _buttonBack.OnClick();

                if (_buttonSave.Hovered(MouseManager.CurrentState.Position))
                    _buttonSave.OnClick(this);

                foreach (SettingsGroup settingsGroup in Groups)
                    if (settingsGroup.Hovered(MouseManager.CurrentState.Position))
                        settingsGroup.OnClick(MouseManager.CurrentState.Position, false);
            }
        }

        private void HandleMouseHoldInput()
        {
            if (MouseManager.CurrentState.LeftButton != ButtonState.Pressed) return;
            foreach (var settingsGroup in Groups.Where(settingsGroup => settingsGroup.Hovered(MouseManager.CurrentState.Position)))
                settingsGroup.OnClick(MouseManager.CurrentState.Position, true);
        }
    }
}
