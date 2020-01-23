using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class ReplayData : SQLiteData
    {
        private string _map;
        private string _replaydata;

        public ReplayData()
        { }

        public ReplayData(SQLiteDataReader data) : base(data) { }

        public ReplayData(string map, string replaydata)
        {
            _map = map;
            _replaydata = replaydata;
        }
    }
}
