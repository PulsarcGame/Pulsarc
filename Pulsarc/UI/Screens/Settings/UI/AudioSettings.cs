using Microsoft.Xna.Framework;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class AudioSettings : SettingsGroup
    {
        public AudioSettings(Vector2 position) : base("Audio", position)
        {
            addSetting("MusicVolume", new Slider("Music Volume", "The general Music volume", getNextPosition(), "int", Config.getInt(name, "MusicVolume"), 0, 100));
            addSetting("GlobalOffset", new Slider("Global Audio Offset", "The offset (in ms) applied to the beatmaps", getNextPosition(), "int", Config.getInt(name, "GlobalOffset"), -300, 300));
            addSetting("RatePitch", new Checkbox("Rate Pitch", "Enables the the picth when using rates", getNextPosition(), "bool", Config.getBool(name, "RatePitch")));
        }
    }
}
