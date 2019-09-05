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
        public override void initDB()
        {
            exec("CREATE TABLE scoredata (map text, datet datetime, score int, accuracy float, maxcombo int, grade varchar(2), max int, perfect int, great int, good int, bad int, miss int)");
            exec("CREATE TABLE replaydata (map text, replaydata BLOB)");
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
