using Pulsarc.Utils.SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Utils
{
    static public class DataManager
    {
        // DBs
        static public ScoreDB scoreDB;
        static public BeatmapDB beatmapDB;

        static public void Initialize()
        {
            // Setup DBs
            scoreDB = new ScoreDB();
            beatmapDB = new BeatmapDB();
        }

        static public void Clean()
        {
            scoreDB.close();
            beatmapDB.close();
        }
    }
}
