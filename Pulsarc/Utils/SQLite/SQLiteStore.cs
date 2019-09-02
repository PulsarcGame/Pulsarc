using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public abstract class SQLiteStore
    {
        SQLiteConnection db;
        string name;

        public SQLiteStore(string name_)
        {
            name = name_;

            if (!File.Exists(name)) {
                SQLiteConnection.CreateFile(name+".db");
                connect();
                initDB();
            } else
            {
                connect();
            }


        }

        public void connect()
        {
            db = new SQLiteConnection("Data Source="+name+".db;Version=3;");
            db.Open();
        }

        public void exec(string sql)
        {
            new SQLiteCommand(sql, db).ExecuteNonQuery();
        }

        public SQLiteDataReader query(string sql)
        {
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
        }

        public abstract void initDB();
    }
}
