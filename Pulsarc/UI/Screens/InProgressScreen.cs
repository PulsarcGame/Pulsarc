using Wobble.Screens;

namespace Pulsarc.UI.Screens
{
    class InProgressScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        public InProgressScreen()
        {
            View = new InProgressScreenView(this);
        }
        public override void Init()
        {
            ScreenManager.RemoveScreen();
        }
    }
}