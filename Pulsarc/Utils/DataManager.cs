using Pulsarc.Utils.SQLite;

namespace Pulsarc.Utils
{
    public static class DataManager
    {
        // DBs
        public static ScoreDb ScoreDb { get; private set; }
        public static BeatmapDb BeatmapDb { get; private set; }

        public static void Initialize()
        {
            // Setup DBs
            ScoreDb = new ScoreDb();
            BeatmapDb = new BeatmapDb();
        }

        public static void Clean()
        {
            ScoreDb.Close();
            BeatmapDb.Close();
        }
    }
}
