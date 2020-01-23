using System;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Quit
{
    class QuitScreen : PulsarcScreen
    {
        public sealed override ScreenView View { get; protected set; }

        public QuitScreen()
        {
            View = new QuitScreenView(this);
        }

        public override void Init()
        {
            base.Init();
            Pulsarc.Quit();
            Environment.Exit(0);
        }

        protected override void UpdateDiscord() { }
    }
}
