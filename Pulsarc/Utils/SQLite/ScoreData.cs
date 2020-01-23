using System;
using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class ScoreData : SQLiteData
    {
        // Play Data
        public string Map;
        public string DateT;
        public string Username;

        // Player modifiers
        public double Rate;
        public int Mods;

        // Play Results
        public int Score;
        public double Accuracy;
        public int MaxCombo;
        public string Grade;

        // Judgements
        public int Max;
        public int Perfect;
        public int Great;
        public int Good;
        public int Bad;
        public int Miss;

        public ScoreData()
        { }

        public ScoreData(SQLiteDataReader data) : base(data) { }

        public ScoreData(string map, string username, double rate, int mods, int score, double accuracy, int maxCombo, string grade, int max, int perfect, int great, int good, int bad, int miss) :
            this(map, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), username, rate, mods, score, accuracy, maxCombo, grade, max, perfect, great, good, bad, miss)
        { }

        public ScoreData(string map, string date, string username, double rate, int mods, int score, double accuracy, int maxCombo, string grade, int max, int perfect, int great, int good, int bad, int miss)
        {
            Map = map;
            DateT = date;
            Username = username;
            Rate = rate;
            Mods = mods;
            Score = score;
            Accuracy = accuracy;
            MaxCombo = maxCombo;
            Grade = grade;
            Max = max;
            Perfect = perfect;
            Great = great;
            Good = good;
            Bad = bad;
            Miss = miss;
        }

        public override string ToString()
        {
            double acc = Math.Round(Accuracy * 100, 2);

            return $"{DateT} - {Score} | {acc}% | x{MaxCombo}";
        }
    }
}
