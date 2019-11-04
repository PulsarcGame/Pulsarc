using Pulsarc.Utils.SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Utils
{
    static public class DataManager
    {
        // DBs
        public static ScoreDB ScoreDB;
        public static BeatmapDB BeatmapDB;

        public static void Initialize()
        {
            // Setup DBs
            ScoreDB = new ScoreDB();
            BeatmapDB = new BeatmapDB();
        }

        public static void Clean()
        {
            ScoreDB.Close();
            BeatmapDB.Close();
        }
    }
}
