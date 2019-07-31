using Wobble.Screens;

namespace Pulsarc.UI.Screens.MainMenu
{
    class Menu : Screen
    {

        public override ScreenView View { get; protected set; }

        public Menu()
        {
            View = new MenuView(this);
        }
    }
}
