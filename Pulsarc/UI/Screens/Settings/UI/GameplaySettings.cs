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
            addSetting("SongRate", new RateSlider("Song rate", "The rate at which songs are played", getNextPosition(), "float", (int) (Config.getFloat(name,"SongRate") * 100), 50, 200, 5, 100));
            addSetting("ApproachSpeed", new Slider("Approach Speed", "The speed at which arcs are approaching the crosshair", getNextPosition(), "int", Config.getInt(name, "ApproachSpeed"), 1, 40));
            addSetting("BackgroundDim", new Slider("Background Dim", "Opacity (in %) of the Dim applied to a beatmap's Background while playing", getNextPosition(), "int", Config.getInt(name, "BackgroundDim"), 1, 100));
            addSetting("FadeTime", new Slider("Fade Time", "Amount of time (in ms) each arc takes to disappear after being pressed", getNextPosition(), "int", Config.getInt(name, "FadeTime"), 0, 1000));
        }
    }
}
