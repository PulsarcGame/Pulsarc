using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Reflection;
using Pulsarc.UI.Screens.Gameplay;

namespace Pulsarc.Utils.SQLite
{
    public abstract class SQLiteStore
    {
        static public bool logging = true;
        SQLiteConnection db;
        string filename;
        public List<SQLiteData> tables;

        public SQLiteStore(string name_)
        {
            filename = name_ + ".db";

            tables = new List<SQLiteData>();

            initTables(); 

            if (!File.Exists(filename)) {
                SQLiteConnection.CreateFile(filename);
                connect();
                initDB();
            } else
            {
                connect();
            }
        }

        public abstract void initTables();

        public void connect()
        {
            db = new SQLiteConnection("Data Source="+ filename +";Version=3;");
            db.Open();
        }

        public void exec(string sql)
        {
            if (logging) Console.WriteLine(sql);
            new SQLiteCommand(sql, db).ExecuteNonQuery();
        }

        public SQLiteDataReader query(string sql)
        {
            if (logging) Console.WriteLine(sql);
            return new SQLiteCommand(sql, db).ExecuteReader();
        }

        public List<object> queryFirst(string sql)
        {
            List<object> res = new List<object>();
            SQLiteDataReader reader = query(sql);
            reader.Read();
            res.Add(reader);
            return res;
        }

        public void close()
        {
            db.Close();
            tables.Clear();
        }

        public void initDB()
        {
            foreach(SQLiteData data in tables)
            {
                string r = "CREATE TABLE " + data.GetType().Name.ToLower() + " (";

                bool first = true;
                foreach (FieldInfo prop in data.GetType().GetFields())
                {
                    if(first)
                    {
                        first = false;
                    } else
                    {
                        r += ",";
                    }
                    r += prop.Name.ToLower() + " " + prop.FieldType.Name.ToString().ToLower();
                }

                r += ")";

                exec(r);
            }
        }
    }
}
