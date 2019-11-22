using System;
using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class ScoreData : SQLiteData
    {
        // Play Data
        public string map;
        public string dateT;
        public string username;

        // Player modifiers
        public double rate;
        public int mods;

        // Play Results
        public int score;
        public double accuracy;
        public int maxCombo;
        public string grade;

        // Judgements
        public int max;
        public int perfect;
        public int great;
        public int good;
        public int bad;
        public int miss;

        public ScoreData() : base() { }

        public ScoreData(SQLiteDataReader data) : base(data) { }

        public ScoreData(string map_, string username_, double rate_, int mods_, int score_, double accuracy_, int maxcombo_, string grade_, int max_, int perfect_, int great_, int good_, int bad_, int miss_) :
            this(map_, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), username_, rate_, mods_, score_, accuracy_, maxcombo_, grade_, max_, perfect_, great_, good_, bad_, miss_)
        { }

        public ScoreData(string map_, string datet_, string username_, double rate_, int mods_, int score_, double accuracy_, int maxcombo_, string grade_, int max_, int perfect_, int great_, int good_, int bad_, int miss_)
        {
            map = map_;
            dateT = datet_;
            username = username_;
            rate = rate_;
            mods = mods_;
            score = score_;
            accuracy = accuracy_;
            maxCombo = maxcombo_;
            grade = grade_;
            max = max_;
            perfect = perfect_;
            great = great_;
            good = good_;
            bad = bad_;
            miss = miss_;
        }

        public override string ToString()
        {
            double acc = Math.Round(accuracy * 100, 2);

            return $"{dateT} - {score} | {acc}% | x{maxCombo}";
        }
    }
}
