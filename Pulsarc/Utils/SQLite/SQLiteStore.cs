using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public abstract class SQLiteStore
    {
        static public bool logging = false;
        SQLiteConnection db;
        string filename;

        public SQLiteStore(string name_)
        {
            filename = name_ + ".db";

            if (!File.Exists(filename)) {
                SQLiteConnection.CreateFile(filename);
                connect();
                initDB();
            } else
            {
                connect();
            }


        }

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
        }

        public abstract void initDB();
    }
}
