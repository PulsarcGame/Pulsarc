using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Settings
{
    class SettingsScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        public float SelectedFocus { get; private set; }
        private int _lastScrollValue;

        public override void Init()
        {
            base.Init();

            View = new SettingsScreenView(this);
        }

        public override void Update(GameTime gameTime)
        {
            HandleMouseInput();

            View?.Update(gameTime);
        }

        /// <summary>
        /// Change the focus when the mouse wheel scrolls.
        /// </summary>
        /// <param name="ms">The mouse state.</param>
        private void ChangeFocus(MouseState ms)
        {
            // If the scroll wheel's state has changed, change the focus
            if (ms.ScrollWheelValue < _lastScrollValue)
                SelectedFocus += 0.3f;
            else if (ms.ScrollWheelValue > _lastScrollValue)
                SelectedFocus -= 0.3f;

            _lastScrollValue = ms.ScrollWheelValue;
        }

        private void HandleMouseInput()
        {
            MouseState ms = Mouse.GetState();

            ChangeFocus(ms);
        }

        protected override void UpdateDiscord() { }
    }
}
