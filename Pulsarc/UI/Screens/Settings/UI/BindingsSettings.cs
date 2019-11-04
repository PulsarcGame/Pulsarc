using Microsoft.Xna.Framework;
using Pulsarc.Utils;
namespace Pulsarc.UI.Screens.Settings.UI
{
    public class BindingsSettings : SettingsGroup
    {
        public BindingsSettings(Vector2 position) : base("Bindings", position)
        {
            AddSetting("Left", new Binding("Left", "Left Arc Key", GetNextPosition(), Config.Get["Bindings"]["Left"]));
            AddSetting("Up", new Binding("Up", "Up Arc Key", GetNextPosition(), Config.Get["Bindings"]["Up"]));
            AddSetting("Down", new Binding("Down", "Down Arc Key", GetNextPosition(), Config.Get["Bindings"]["Down"]));
            AddSetting("Right", new Binding("Right", "Right Arc Key", GetNextPosition(), Config.Get["Bindings"]["Right"]));
            AddSetting("Pause", new Binding("Pause", "Pause Key", GetNextPosition(), Config.Get["Bindings"]["Pause"]));
            AddSetting("Continue", new Binding("Continue", "Continue Key", GetNextPosition(), Config.Get["Bindings"]["Continue"]));
            AddSetting("Retry", new Binding("Retry", "Retry Key", GetNextPosition(), Config.Get["Bindings"]["Retry"]));
            AddSetting("Convert", new Binding("Convert", "Convert Key", GetNextPosition(), Config.Get["Bindings"]["Convert"]));
        }
    }
}
