using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Pulsarc.Utils.SQLite
{
    public class ScoreDB : SQLiteStore
    {
        public ScoreDB() : base("scores") { }

        public override void initTables()
        {
            tables.Add(new ScoreData());
            tables.Add(new ReplayData());
        }

        public void addScore(ScoreData score)
        {
            score.SaveData(this);
        }

        public void addReplay(ReplayData replay)
        {
            replay.SaveData(this);
        }

        public List<ScoreData> getScores(string map)
        {
            List<ScoreData> scores = new List<ScoreData>();

            SQLiteDataReader r = query("SELECT * FROM scoredata WHERE map = '" + map + "' ORDER BY score DESC, datet ASC");
            while(r.Read())
            {
                scores.Add(new ScoreData(r));
            }

            foreach(ScoreData score in scores)
            {
                Console.WriteLine(score.ToString());
            }

            return scores;
        }
    }
}
