using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Utils.SQLite
{
    public class ScoreDB : SQLiteStore
    {
        public ScoreDB() : base("scores") { }
        public override void initDB()
        {
            exec("CREATE TABLE scoredata (map text, datet datetime, score int, grade varchar(2), max mediumint, perfect mediumint, great mediumint, good mediumint, bad mediumint, miss mediumint)");
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

        public List<ScoreData> getScores()
        {
            List<ScoreData> scores = new List<ScoreData>();

            return scores;
        }
    }
}
