using Microsoft.Xna.Framework;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class GameplaySettings : SettingsGroup
    {
        public GameplaySettings(Vector2 position) : base("Gameplay", position)
        {
            addSetting("SongRate", new RateSlider("Song rate", "The rate at which songs are played", getNextPosition(), "float", 100, 50, 200, 5, 100));
        }
    }
}
