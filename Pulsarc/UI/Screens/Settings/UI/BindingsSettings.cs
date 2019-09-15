using Microsoft.Xna.Framework;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class BindingsSettings : SettingsGroup
    {
        public BindingsSettings(Vector2 position) : base("Bindings", position)
        {
            addSetting("Left", new Binding("Left", "Left arc key", getNextPosition(), Config.get["Bindings"]["Left"]));
            addSetting("Up", new Binding("Up", "Up arc key", getNextPosition(), Config.get["Bindings"]["Up"]));
            addSetting("Down", new Binding("Down", "Down arc key", getNextPosition(), Config.get["Bindings"]["Down"]));
            addSetting("Right", new Binding("Right", "Right arc key", getNextPosition(), Config.get["Bindings"]["Right"]));
            addSetting("Pause", new Binding("Pause", "Pause key", getNextPosition(), Config.get["Bindings"]["Pause"]));
            addSetting("Continue", new Binding("Continue", "Continue key", getNextPosition(), Config.get["Bindings"]["Continue"]));
            addSetting("Retry", new Binding("Retry", "Retry key", getNextPosition(), Config.get["Bindings"]["Retry"]));
            addSetting("Convert", new Binding("Convert", "Convert key", getNextPosition(), Config.get["Bindings"]["Convert"]));
        }
    }
}
