using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Settings
{
    class SettingsScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        public override void Init()
        {
            View = new SettingsScreenView(this);
        }
    }
}
