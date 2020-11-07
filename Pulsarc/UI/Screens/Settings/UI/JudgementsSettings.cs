using Microsoft.Xna.Framework;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class JudgementsSettings : SettingsGroup
    {
        public JudgementsSettings(Vector2 position) : base("Judgements", position)
        {
            JudgeSlider Max = new JudgeSlider("Max", "Max judgement in ms", GetNextPosition(), "int", null, Config.GetInt(Name, "Max"), 1, 500);
            AddSetting("Max", Max);

            JudgeSlider Perfect = new JudgeSlider("Perfect", "Max judgement in ms", GetNextPosition(), "int", Max, Config.GetInt(Name, "Perfect"), 1, 500);
            AddSetting("Perfect", Perfect);
            Max.SetNext(Perfect);

            JudgeSlider Great = new JudgeSlider("Great", "Great judgement in ms", GetNextPosition(), "int", Perfect, Config.GetInt(Name, "Great"), 1, 500);
            AddSetting("Great", Great);
            Perfect.SetNext(Great);

            JudgeSlider Good = new JudgeSlider("Good", "Good judgement in ms", GetNextPosition(), "int", Great, Config.GetInt(Name, "Good"), 1, 500);
            AddSetting("Good", Good);
            Great.SetNext(Good);

            JudgeSlider Bad = new JudgeSlider("Bad", "Bad judgement in ms", GetNextPosition(), "int", Good, Config.GetInt(Name, "Bad"), 1, 500);
            AddSetting("Bad", Bad);
            Good.SetNext(Bad);

            JudgeSlider Miss = new JudgeSlider("Miss", "Miss judgement in ms", GetNextPosition(), "int", Bad, Config.GetInt(Name, "Miss"), 1, 500);
            AddSetting("Miss", Miss);
            Bad.SetNext(Miss);
        }
    }
}
