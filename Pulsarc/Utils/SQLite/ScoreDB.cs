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
            exec("CREATE TABLE score (map bigint, score int, grade char, max mediumint, perfect mediumint, great mediumint, good mediumint, bad mediumint, miss mediumint)");
            exec("CREATE TABLE replay (map bigint, replaydata BLOB, float performance)");
        }

        public void addScore(int map, int score, char grade, int max, int perfect, int great, int good, int bad, int miss)
        {
            exec("INSERT INTO score (map, score, grade, max, perfect, great, good, bad, miss) VALUES " +
                "( " + map + "," + score + ",'" + grade + "'," + max + "," + perfect + "," + great + "," + good + "," + bad + "," + miss + ")");
        }
    }
}
