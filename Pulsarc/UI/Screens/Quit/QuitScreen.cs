using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Quit
{
    class QuitScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        public QuitScreen()
        {
            View = new QuitScreenView(this);
        }

        public override void Init()
        {
            Pulsarc.Quit();
            Environment.Exit(0);
        }
    }
}
