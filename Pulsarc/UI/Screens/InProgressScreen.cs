using Wobble.Screens;

namespace Pulsarc.UI.Screens
{
    /// <summary>
    /// A placeholder screen for not-yet-implemented screens.
    /// </summary>
    class InProgressScreen : PulsarcScreen
    {
        public sealed override ScreenView View { get; protected set; }

        public InProgressScreen()
        {
            View = new InProgressScreenView(this);
        }

        public override void Init()
        {
            ScreenManager.RemoveScreen();
        }

        protected override void UpdateDiscord() { }
    }
}
