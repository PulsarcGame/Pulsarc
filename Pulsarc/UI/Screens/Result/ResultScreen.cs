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
        public List<KeyValuePair<long, Double>> hits;
        public int display_score;

        public ResultScreen(List<JudgementValue> judgements_, List<KeyValuePair<long, Double>> hits_, int display_score_)
        {
            judgements = judgements_;
            hits = hits_;
            display_score = display_score_;

            double accuracyTotal = 0;
            foreach (JudgementValue judge in judgements)
            {
                Console.WriteLine(judge.acc);
                accuracyTotal += judge.acc;
            }
            accuracyTotal /= judgements.Count;

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
