using System.Collections.Generic;
using System.Data.SQLite;
using Wobble.Logging;

namespace Pulsarc.Utils.SQLite
{
    public class ScoreDb : SqLiteStore
    {
        public ScoreDb() : base("scores") { }

        protected override void InitTables()
        {
            Tables.Add(new ScoreData());
            Tables.Add(new ReplayData());
        }

        public void AddScore(ScoreData score)
        {
            score.SaveData(this);
        }

        public void AddReplay(ReplayData replay)
        {
            replay.SaveData(this);
        }

        public List<ScoreData> GetScores(string map)
        {
            List<ScoreData> scores = new List<ScoreData>();

            SQLiteDataReader r = Query($"SELECT * FROM scoredata WHERE map = '{map}' ORDER BY score DESC, datet ASC");

            // r can be null due to the current issues with ScoreDB
            while (r != null && r.Read())
                scores.Add(new ScoreData(r));

            foreach (ScoreData score in scores)
                PulsarcLogger.Debug(score.ToString(), LogType.Runtime);

            return scores;
        }
    }
}
