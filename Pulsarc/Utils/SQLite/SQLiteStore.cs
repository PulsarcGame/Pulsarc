using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Wobble.Logging;

namespace Pulsarc.Utils.SQLite
{
    public abstract class SqLiteStore
    {
        public static bool Logging = true;

        private SQLiteConnection _db;
        private readonly string _filename;

        protected List<SQLiteData> Tables { get; }

        protected SqLiteStore(string name)
        {
            _filename = name + ".db";

            Tables = new List<SQLiteData>();

            InitTables(); 

            if (!File.Exists(_filename))
            {
                SQLiteConnection.CreateFile(_filename);
                Connect();
                InitDb();
            }
            else
                Connect();
        }

        protected abstract void InitTables();

        private void Connect()
        {
            _db = new SQLiteConnection($"Data Source={_filename};Version=3;");
            _db.Open();
        }

        public void Exec(string sql)
        {
            PulsarcLogger.Debug(sql, LogType.Runtime);

            new SQLiteCommand(sql, _db).ExecuteNonQuery();
        }

        protected SQLiteDataReader Query(string sql)
        {
            PulsarcLogger.Debug(sql, LogType.Runtime);

            return new SQLiteCommand(sql, _db).ExecuteReader();
        }

        public List<object> QueryFirst(string sql)
        {
            List<object> res = new List<object>();
            SQLiteDataReader reader = Query(sql);

            reader.Read();
            res.Add(reader);
            
            return res;
        }

        public void Close()
        {
            _db.Close();
            Tables.Clear();
        }

        private void InitDb()
        {
            foreach(SQLiteData data in Tables)
            {
                string r = $"CREATE TABLE {data.GetType().Name.ToLower()} (";

                bool first = true;
                foreach (FieldInfo prop in data.GetType().GetFields())
                {
                    if(first)
                        first = false;
                    else
                        r += ",";

                    r += prop.Name.ToLower() + " " + prop.FieldType.Name.ToLower();
                }

                r += ")";

                Exec(r);
            }
        }
    }
}
