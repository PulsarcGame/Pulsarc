using Pulsarc.Beatmaps;
using Pulsarc.UI.Common;
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

        // The map background
        Background mapBackGround;

        /// <summary>
        /// The screen that summarizes a play.
        /// </summary>
        /// <param name="judgements_">All of the judgements from the play.</param>
        /// <param name="hits_">All the hits from the play.</param>
        /// <param name="display_score_">The score to be displayed.</param>
        /// <param name="combo_">The maximum combo achieved from the play.</param>
        /// <param name="beatmap_">The beatmap that was played.</param>
        public ResultScreen(List<JudgementValue> judgements_, List<KeyValuePair<double, int>> hits_, int display_score_, int combo_, Beatmap beatmap_, Background mapBackground, bool newScore = false)
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
            string grade = Scoring.getGrade(accuracyTotal);

            if(newScore)
            {
                // Save the score locally
                Pulsarc.scoreDB.addScore(new ScoreData(beatmap.getHash(), display_score,(float) accuracyTotal, combo, grade, judges_count["max"], judges_count["perfect"], judges_count["great"], judges_count["good"], judges_count["bad"], judges_count["miss"]));
            }

            View = new ResultScreenView(this, accuracyTotal, grade, beatmap, mapBackground);
        }
    }
}
