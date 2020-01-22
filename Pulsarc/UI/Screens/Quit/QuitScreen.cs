using System;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Quit
{
    class QuitScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        public QuitScreen() => View = new QuitScreenView(this);

        public override void Init()
        {
            base.Init();
            Pulsarc.Quit();
        }

        public override void UpdateDiscord() { }
    }
}
