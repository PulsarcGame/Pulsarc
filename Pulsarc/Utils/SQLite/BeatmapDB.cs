using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Pulsarc.Utils.SQLite
{
    public class BeatmapDB : SQLiteStore
    {
        public BeatmapDB() : base("beatmap") { }

        public override void initTables()
        {
            tables.Add(new BeatmapData());
        }

        public void addBeatmap(BeatmapData map)
        {
            map.SaveData(this);
        }

        public void clearBeatmaps()
        {
            exec("DELETE FROM beatmapdata");
        }

        public List<BeatmapData> getBeatmaps()
        {
            List<BeatmapData> maps = new List<BeatmapData>();

            SQLiteDataReader r = query("SELECT * FROM beatmapdata");
            while(r.Read())
            {
                maps.Add(new BeatmapData(r));
            }

            return maps;
        }
    }
}
