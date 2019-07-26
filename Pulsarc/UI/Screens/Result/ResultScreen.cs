using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreen : Screen
    {
        public override ScreenView View { get; protected set; }

        public List<JudgementValue> judgements;
        public Dictionary<string, int> judges_count;
        public List<KeyValuePair<long, int>> hits;
        public int display_score;

        public bool full_combo;

        public ResultScreen(List<JudgementValue> judgements_, List<KeyValuePair<long, int>> hits_, int display_score_)
        {
            judges_count = new Dictionary<string, int>();
            foreach(JudgementValue judge in Judgement.judgements)
            {
                judges_count.Add(judge.name, 0);
            }

            judgements = judgements_;
            hits = hits_;
            display_score = display_score_;

            double accuracyTotal = 0;
            foreach (JudgementValue judge in judgements)
            {
                accuracyTotal += judge.acc;
                judges_count[judge.name]++;
            }
            accuracyTotal /= judgements.Count;

            full_combo = judges_count["miss"] == 0;

            string grade = "D";

            if(accuracyTotal == 1.0)
            {
                grade = "X";
            }
            else if (accuracyTotal >= 0.95)
            {
                grade = "S";
            }
            else if (accuracyTotal >= 0.90)
            {
                grade = "A";
            }
            else if (accuracyTotal >= 0.80)
            {
                grade = "B";
            }
            else if (accuracyTotal >= 0.70)
            {
                grade = "C";
            }

            View = new ResultScreenView(this, accuracyTotal, grade);
        }
    }
}
