using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class ReplayData : SQLiteData
    {
        public string map;
        public string replaydata;

        public ReplayData() : base() { }

        public ReplayData(SQLiteDataReader data) : base(data) { }

        public ReplayData(string map_, string replaydata_)
        {
            map = map_;
            replaydata = replaydata_;
        }
    }
}
