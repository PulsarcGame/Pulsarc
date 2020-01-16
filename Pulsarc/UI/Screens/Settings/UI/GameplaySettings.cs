using Microsoft.Xna.Framework;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class GameplaySettings : SettingsGroup
    {
        public GameplaySettings(Vector2 position) : base("Gameplay", position)
        {
            AddSetting("SongRate", new RateSlider("Song rate", "The rate at which songs are played", GetNextPosition(), "float", (int) (Config.SongRate.Value * 100), 50, 200, 5, 100));
            AddSetting("ApproachSpeed", new Slider("Approach Speed", "The speed at which arcs are approaching the crosshair", GetNextPosition(), "double", (int) Config.ApproachSpeed.Value, 1, 40));
            AddSetting("BackgroundDim", new Slider("Background Dim", "Opacity (in %) of the Dim applied to a beatmap's Background while playing", GetNextPosition(), "int", Config.BackgroundDim.Value, 1, 100));
            AddSetting("FadeTime", new Slider("Fade Time", "Amount of time (in ms) each arc takes to disappear after being pressed", GetNextPosition(), "int", Config.FadeTime.Value, 0, 1000));
            AddSetting("Hidden", new Checkbox("Hidden", "Enables the Hidden mod", GetNextPosition(), "bool", Config.Hidden.Value));
            AddSetting("Autoplay", new Checkbox("Autoplay", "Enables the Autoplay mod", GetNextPosition(), "bool", Config.Autoplay.Value));
        }
    }
}
