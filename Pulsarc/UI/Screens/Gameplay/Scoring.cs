using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Gameplay
{
    static class Scoring
    {
        static public int max_score = 1000000;
        static public int max_combo_multiplier = 50;

        static public int getMaxScore(int objectsCount)
        {
            return objectsCount * Judgement.judgements[0].score * max_combo_multiplier;
        }

        static public KeyValuePair<long, int> processHitResults(JudgementValue judge, long currentScore, int currentComboMultiplier)
        {
            long score = currentScore + judge.score * currentComboMultiplier;
            int comboMultiplier = currentComboMultiplier + judge.combo_addition;

            if(comboMultiplier > max_combo_multiplier)
            {
                comboMultiplier = max_combo_multiplier;
            } else if (comboMultiplier < 1)
            {
                comboMultiplier = 1;
            }

            return new KeyValuePair<long, int>(score, comboMultiplier);
        }
    }
}
