using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Settings
{
    class SettingsScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        public float SelectedFocus { get; private set; } = 0;
        private int lastScrollValue = 0;

        public override void Init()
        {
            base.Init();

            DiscordDetails = "In the menus";

            View = new SettingsScreenView(this);
        }

        public override void Update(GameTime gameTime)
        {
            handleMouseInput();

            View?.Update(gameTime);
        }

        /// <summary>
        /// Change the focus when the mouse wheel scrolls.
        /// </summary>
        /// <param name="ms">The mouse state.</param>
        private void changeFocus(MouseState ms)
        {
            // If the scroll wheel's state has changed, change the focus
            if (ms.ScrollWheelValue < lastScrollValue)
                SelectedFocus += 0.3f;
            else if (ms.ScrollWheelValue > lastScrollValue)
                SelectedFocus -= 0.3f;

            lastScrollValue = ms.ScrollWheelValue;
        }

        private void handleMouseInput()
        {
            MouseState ms = Mouse.GetState();

            changeFocus(ms);
        }
    }
}
