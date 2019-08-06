﻿using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Gameplay
{
    static class Scoring
    {
        public static int max_score = 1000000;
        public static int max_combo_multiplier = 100;

        public static int GetMaxScore(int objectsCount)
        {
            // Simple formula due to the combo system starting at max and being capped
            // Changing it wrongly would require parsing and other stuff

            return objectsCount * Judgement.judgements[0].score * max_combo_multiplier;
        }

        public static KeyValuePair<long, int> processHitResults(JudgementValue judge, long currentScore, int currentComboMultiplier)
        {
            // Get the next score from a score and a new hit

            long score = currentScore + judge.score * currentComboMultiplier;
            int comboMultiplier = currentComboMultiplier + judge.combo_addition;

            if(comboMultiplier > max_combo_multiplier)
            {
                comboMultiplier = max_combo_multiplier;
            }
            else if (comboMultiplier < 1)
            {
                comboMultiplier = 1;
            }

            return new KeyValuePair<long, int>(score, comboMultiplier);
        }
    }
}