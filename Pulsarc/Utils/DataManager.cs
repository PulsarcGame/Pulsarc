using Pulsarc.Utils.SQLite;

namespace Pulsarc.Utils
{
    static public class DataManager
    {
        // DBs
        public static ScoreDB ScoreDB { get; private set; }
        public static BeatmapDB BeatmapDB { get; private set; }

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
