using Microsoft.Xna.Framework;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class AudioSettings : SettingsGroup
    {
        public AudioSettings(Vector2 position) : base("Audio", position)
        {
            AddSetting("MusicVolume", new Slider("Music Volume", "The general Music volume", GetNextPosition(), "int", Config.GetInt(Name, "MusicVolume"), 0, 100));
            AddSetting("GlobalOffset", new Slider("Global Audio Offset", "The offset (in ms) applied to the beatmaps", GetNextPosition(), "int", Config.GetInt(Name, "GlobalOffset"), -300, 300));
            AddSetting("RatePitch", new Checkbox("Rate Pitch", "Enables the the picth when using rates", GetNextPosition(), "bool", Config.GetBool(Name, "RatePitch")));
        }
    }
}
