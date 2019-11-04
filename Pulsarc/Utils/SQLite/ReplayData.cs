using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class ReplayData : SQLiteData
    {
        public string Map { get; private set; }
        public string Replaydata { get; private set; }

        public ReplayData() : base() { }

        public ReplayData(SQLiteDataReader data) : base(data) { }

        public ReplayData(string map_, string replaydata_)
        {
            Map = map_;
            Replaydata = replaydata_;
        }
    }
}
