using System.Collections.Generic;
using Pulsarc.Utils.SQLite;

namespace Pulsarc.UI.Screens.Gameplay
{
    static class Scoring
    {
        // Max visible score
        public static int MaxScore { get; private set; } = 1000000;
        
        // Max hidden combo multiplier
        static public int MaxComboMultiplier { get; private set; } = 100;

        static public int GetMaxScore(int objectsCount)
        {
            // Simple formula due to the combo system starting at max and being capped
            // Changing it wrongly would require parsing and other stuff

            return objectsCount * Judgement.Judgements[0].Score * MaxComboMultiplier;
        }

        static public KeyValuePair<long, int> ProcessHitResults(JudgementValue judge, long currentScore, int currentComboMultiplier)
        {
            // Get the next score from a score and a new hit
            long score = currentScore + judge.Score * currentComboMultiplier;
            int comboMultiplier = currentComboMultiplier + judge.ComboAddition;

            if(comboMultiplier > MaxComboMultiplier)
                comboMultiplier = MaxComboMultiplier;
            else if (comboMultiplier < 1)
                comboMultiplier = 1;

            return new KeyValuePair<long, int>(score, comboMultiplier);
        }

        /// <summary>
        /// Find the grade based on accuracy, judge hits, etc.
        /// </summary>
        /// <param name="score">ScoreData of the play</param>
        /// <returns></returns>
        static public string GetGrade(ScoreData score)
        {
            // If acc is over 97.5%
            if (score.Accuracy >= 0.975)
            {
                // If made at least one miss, get AAA
                if (score.Miss >= 1)
                    return "AAA";
                // But if no misses
                // And no Greats, Goods, or Bads
                if (score.Great + score.Good + score.Bad == 0)
                {
                    // And no Perfects, get X
                    if (score.Perfect == 0)
                        return "X";
                    // Otherwise
                    // If the ratio between MAX and Perfect hits is 10:1 or greater
                    // Get SSS
                    return score.Max / score.Perfect >= 10 ? "SSS" : "SS";
                    // Otherwise get SS
                }
                // If there is at least one Great, Good, or Bad

                // If there's more than 10 Greats, get AAA
                if (score.Great >= 10)
                    return "AAA";
                // But if there's less than 10 greats
                // And there's no perfects
                // OR the MAX : Perfect ratio is 10:1 or greater
                // Get SS
                if (score.Perfect == 0 || score.Max / score.Perfect >= 10)
                    return "SS";
                // Otherwise get S
                return "S";
            }

            // If acc is greater than 95%, get AA
            if (score.Accuracy >= 0.95)
                return "AA";

            // If acc is greater than 90%, get A
            if (score.Accuracy >= 0.9)
                return "A";

            // If acc is greater than 80%, get B
            if (score.Accuracy >= 0.8)
                return "B";

            // If acc is greater than 70%, get C
            return score.Accuracy >= 0.7 ? "C" : "D";

            // If acc is lower than 70%, get D
        }
    }
}
