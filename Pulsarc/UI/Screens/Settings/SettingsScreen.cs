using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Utils;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Settings
{
    class SettingsScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        private float selectedFocus = 0;
        public float SelectedFocus
        {
            get => selectedFocus;
            private set
            {
                // Don't set this higher or lower than the min/max focus
                selectedFocus =
                    value > MAX_FOCUS ? MAX_FOCUS : value < MIN_FOCUS ? MIN_FOCUS : value;

                // Can uncomment this to help find the new max.
                //PulsarcLogger.Debug(SelectedFocus.ToString());
            }
        }
        private int lastScrollValue = 0;

        private const float MAX_FOCUS = 11;
        private const float MIN_FOCUS = 0;

        public override void Init()
        {
            base.Init();

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

        public override void UpdateDiscord() { }
    }
}
