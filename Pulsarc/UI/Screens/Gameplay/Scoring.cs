using Pulsarc.Utils.SQLite;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Gameplay
{
    static class Scoring
    {
        static public int max_score = 1000000;
        static public int max_combo_multiplier = 100;

        static public int getMaxScore(int objectsCount)
        {
            // Simple formula due to the combo system starting at max and being capped
            // Changing it wrongly would require parsing and other stuff

            return objectsCount * Judgement.judgements[0].score * max_combo_multiplier;
        }

        static public KeyValuePair<long, int> processHitResults(JudgementValue judge, long currentScore, int currentComboMultiplier)
        {
            // Get the next score from a score and a new hit

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

        static public string getGrade(double accuracyTotal)
        {
            string grade = "D";

            if (accuracyTotal == 1.0)
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

            return grade;
        }
    }

    public class ScoreData : SQLiteData
    {
        public string map;
        public int score;
        public string grade;
        public int max;
        public int perfect;
        public int great;
        public int good;
        public int bad;
        public int miss;

        public ScoreData(string map_, int score_, string grade_, int max_, int perfect_, int great_, int good_, int bad_, int miss_)
        {
            map = map_;
            score = score_;
            grade = grade_;
            max = max_;
            perfect = perfect_;
            great = great_;
            good = good_;
            bad = bad_;
            miss = miss_;
        }
    }
}
