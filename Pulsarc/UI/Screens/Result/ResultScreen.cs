﻿using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.Gameplay;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreen : Screen
    {
        public override ScreenView View { get; protected set; }

        public List<JudgementValue> judgements;
        public Dictionary<string, int> judges_count;
        public List<KeyValuePair<double, int>> hits;
        public int display_score;
        public int combo;
        public Beatmap beatmap;

        public bool full_combo;

        public ResultScreen(List<JudgementValue> judgements, List<KeyValuePair<double, int>> hits, int display_score, int combo, Beatmap beatmap)
        {
            judges_count = new Dictionary<string, int>();
            foreach(JudgementValue judge in Judgement.judgements)
            {
                judges_count.Add(judge.name, 0);
            }

            this.judgements = judgements;
            this.hits = hits;
            this.display_score = display_score;
            this.combo = combo;
            this.beatmap = beatmap;

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

            View = new ResultScreenView(this, accuracyTotal, grade, beatmap);
        }
    }
}