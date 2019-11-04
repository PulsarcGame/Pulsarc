using System;
using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class ScoreData : SQLiteData
    {
        // Play Data
        public string Map { get; private set; }
        public string DateT { get; private set; }

        // Play Results
        public int Score { get; private set; }
        public double Accuracy { get; private set; }
        public int MaxCombo { get; private set; }
        public string Grade;

        // Judgements
        public int Max { get; private set; }
        public int Perfect { get; private set; }
        public int Great { get; private set; }
        public int Good { get; private set; }
        public int Bad { get; private set; }
        public int Miss { get; private set; }

        public ScoreData() : base() { }

        public ScoreData(SQLiteDataReader data) : base(data) { }

        public ScoreData(string map_, int score_, double accuracy_, int maxcombo_, string grade_, int max_, int perfect_, int great_, int good_, int bad_, int miss_) :
            this(map_, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), score_, accuracy_, maxcombo_, grade_, max_, perfect_, great_, good_, bad_, miss_)
        { }

        public ScoreData(string map_, string datet_, int score_, double accuracy_, int maxcombo_, string grade_, int max_, int perfect_, int great_, int good_, int bad_, int miss_)
        {
            Map = map_;
            DateT = datet_;
            Score = score_;
            Accuracy = accuracy_;
            MaxCombo = maxcombo_;
            Grade = grade_;
            Max = max_;
            Perfect = perfect_;
            Great = great_;
            Good = good_;
            Bad = bad_;
            Miss = miss_;
        }

        public override string ToString()
        {
            double acc = Math.Round(Accuracy * 100, 2);

            return $"{DateT} - {Score} | {acc}% | x{MaxCombo}";
        }
    }
}
