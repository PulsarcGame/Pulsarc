using System.Collections.Generic;
using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class BeatmapDB : SQLiteStore
    {
        public BeatmapDB() : base("beatmap") { }

        public override void InitTables() => Tables.Add(new BeatmapData());

        public void AddBeatmap(BeatmapData map) => map.SaveData(this);

        public void ClearBeatmaps() => Exec("DELETE FROM beatmapdata");

        public List<BeatmapData> GetBeatmaps()
        {
            List<BeatmapData> maps = new List<BeatmapData>();

            SQLiteDataReader r = Query("SELECT * FROM beatmapdata");

            while (r.Read())
            {
                maps.Add(new BeatmapData(r));
            }

            return maps;
        }
    }
}
