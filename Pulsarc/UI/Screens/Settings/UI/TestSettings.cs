using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class TestSettings : SettingsGroup
    {
        public TestSettings(Vector2 position) : base("test", position)
        {
            addSetting(new Slider("Test slider", "Just a test slider", getNextPosition()));
        }
    }
}
