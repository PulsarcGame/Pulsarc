﻿using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.SQLite;
using System;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreen : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        // All of the judgements obtained during the play
        public List<JudgementValue> Judgements { get; private set; }

        // Keeps track of how many of each judgement was obtained.
        public Dictionary<string, int> JudgesCount { get; private set; }

        // A list of all hits obtained during the play.
        public List<KeyValuePair<double, int>> Hits { get; private set; }

        // The score of the play.
        public int DisplayScore { get; private set; }

        // The max combo obtained during play.
        public int Combo { get; private set; }

        // The beatmap that was just played.
        public Beatmap Beatmap { get; private set; }

        // Whether or not a full combo was achieved.
        public bool FullCombo { get; private set; }

        private ScoreData scoreData;

        /// <summary>
        /// The screen that summarizes the last play.
        /// </summary>
        /// <param name="judgements">All of the judgements from the play.</param>
        /// <param name="hits">All the hits from the play.</param>
        /// <param name="displayScore">The score to be displayed.</param>
        /// <param name="combo">The maximum combo achieved from the play.</param>
        /// <param name="beatmap">The beatmap that was played.</param>
        public ResultScreen(List<JudgementValue> judgements, List<KeyValuePair<double, int>> hits, int displayScore, int combo, double rate, int mods, Beatmap beatmap, Background mapBackground, bool newScore = false)
        {
            // Add all the judgement names to judges_count
            JudgesCount = new Dictionary<string, int>();

            foreach(JudgementValue judge in Judgement.Judgements)
                JudgesCount.Add(judge.Name, 0);

            Judgements = judgements;
            Hits = hits;
            DisplayScore = displayScore;
            Combo = combo;
            Beatmap = beatmap;

            // Calculate accuracy from the judgements
            double accuracyTotal = 0;

            foreach (JudgementValue judge in Judgements)
            {
                accuracyTotal += judge.Acc;
                JudgesCount[judge.Name]++;
            }

            accuracyTotal /= Judgements.Count;

            // Determine if a Full Combo was achieved.
            FullCombo = JudgesCount["miss"] == 0;

            // Determine the grade achieved.
            scoreData = new ScoreData(Beatmap.GetHash(), Config.GetString("Profile","Username"), rate, mods, DisplayScore, (float)accuracyTotal, Combo, "D", JudgesCount["max"], JudgesCount["perfect"], JudgesCount["great"], JudgesCount["good"], JudgesCount["bad"], JudgesCount["miss"]);
            scoreData.grade = Scoring.GetGrade(scoreData);

            // Save the score locally
            if (newScore)
                DataManager.ScoreDB.AddScore(scoreData);

            View = new ResultScreenView(this, accuracyTotal, scoreData.grade, Beatmap, mapBackground);
        }

        public override void UpdateDiscord()
        {
            string details = $"Finished {Beatmap.Title}";
            string state = $"Grade: {scoreData.grade} | Score: {scoreData.score} | Acc = {Math.Round(scoreData.accuracy * 100, 2)}%";

            PulsarcDiscord.SetStatus(details, state);
        }
    }
}
