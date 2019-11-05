using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Reflection;

namespace Pulsarc.Utils.SQLite
{
    public abstract class SQLiteStore
    {
        public static bool Logging = true;

        private SQLiteConnection db;
        private string filename;

        public List<SQLiteData> Tables { get; protected set; }

        public SQLiteStore(string name_)
        {
            filename = name_ + ".db";

            Tables = new List<SQLiteData>();

            InitTables(); 

            if (!File.Exists(filename))
            {
                SQLiteConnection.CreateFile(filename);
                Connect();
                InitDB();
            }
            else
                Connect();
        }

        public abstract void InitTables();

        public void Connect()
        {
            db = new SQLiteConnection($"Data Source={filename};Version=3;");
            db.Open();
        }

        public void Exec(string sql)
        {
            if (Logging)
                Console.WriteLine(sql);

            new SQLiteCommand(sql, db).ExecuteNonQuery();
        }

        public SQLiteDataReader Query(string sql)
        {
            if (Logging)
                Console.WriteLine(sql);

            return new SQLiteCommand(sql, db).ExecuteReader();
        }

        public List<object> queryFirst(string sql)
        {
            List<object> res = new List<object>();
            SQLiteDataReader reader = Query(sql);

            reader.Read();
            res.Add(reader);
            
            return res;
        }

        public void Close()
        {
            db.Close();
            Tables.Clear();
        }

        public void InitDB()
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

                    r += prop.Name.ToLower() + " " + prop.FieldType.Name.ToString().ToLower();
                }

                r += ")";

                Exec(r);
            }
        }
    }
}
