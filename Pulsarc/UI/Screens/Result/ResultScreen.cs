using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.Gameplay;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreen : Screen
    {
        public override ScreenView View { get; protected set; }

        // All of the judgements obtained during the play
        public List<JudgementValue> judgements;

        // Keeps track of how many of each judgement was obtained.
        public Dictionary<string, int> judges_count;

        // A list of all hits obtained during the play.
        public List<KeyValuePair<double, int>> hits;

        // The score of the play.
        public int display_score;

        // The max combo obtained during play.
        public int combo;

        // The beatmap that was just played.
        public Beatmap beatmap;

        // Whether or not a full combo was achieved.
        public bool full_combo;

        /// <summary>
        /// The screen that summarizes a play.
        /// </summary>
        /// <param name="judgements_">All of the judgements from the play.</param>
        /// <param name="hits_">All the hits from the play.</param>
        /// <param name="display_score_">The score to be displayed.</param>
        /// <param name="combo_">The maximum combo achieved from the play.</param>
        /// <param name="beatmap_">The beatmap that was played.</param>
        public ResultScreen(List<JudgementValue> judgements_, List<KeyValuePair<double, int>> hits_, int display_score_, int combo_, Beatmap beatmap_)
        {
            // Add all the judgement names to judges_count
            judges_count = new Dictionary<string, int>();
            foreach(JudgementValue judge in Judgement.judgements)
            {
                judges_count.Add(judge.name, 0);
            }

            judgements = judgements_;
            hits = hits_;
            display_score = display_score_;
            combo = combo_;
            beatmap = beatmap_;

            // Calculate accuracy from the judgements
            double accuracyTotal = 0;
            foreach (JudgementValue judge in judgements)
            {
                accuracyTotal += judge.acc;
                judges_count[judge.name]++;
            }
            accuracyTotal /= judgements.Count;

            // Determine if a Full Combo was achieved.
            full_combo = judges_count["miss"] == 0;

            // Determine the grade achieved.
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
