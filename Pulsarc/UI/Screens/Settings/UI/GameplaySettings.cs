using Microsoft.Xna.Framework;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class GameplaySettings : SettingsGroup
    {
        public GameplaySettings(Vector2 position) : base("Gameplay", position)
        {
            AddSetting("SongRate", new RateSlider("Song rate", "The rate at which songs are played", GetNextPosition(), "float", (int) (Config.GetFloat(Name,"SongRate") * 100), 50, 200, 5, 100));
            AddSetting("ApproachSpeed", new Slider("Approach Speed", "The speed at which arcs are approaching the crosshair", GetNextPosition(), "int", Config.GetInt(Name, "ApproachSpeed"), 1, 40));
            AddSetting("BackgroundDim", new Slider("Background Dim", "Opacity (in %) of the Dim applied to a beatmap's Background while playing", GetNextPosition(), "int", Config.GetInt(Name, "BackgroundDim"), 1, 100));
            AddSetting("FadeTime", new Slider("Fade Time", "Amount of time (in ms) each arc takes to disappear after being pressed", GetNextPosition(), "int", Config.GetInt(Name, "FadeTime"), 0, 1000));
            AddSetting("Hidden", new Checkbox("Hidden", "Enables the Hidden mod", GetNextPosition(), "bool", Config.GetBool(Name, "Hidden")));
            AddSetting("Autoplay", new Checkbox("Autoplay", "Enables the Autoplay mod", GetNextPosition(), "bool", Config.GetBool(Name, "Autoplay")));
        }
    }
}
