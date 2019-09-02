using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pulsarc.Utils.SQLite
{
    public abstract class SQLiteData
    {

        public void SaveData(SQLiteStore db)
        {
            string r = "INSERT INTO " + GetType().Name.ToLower() + " (";
            string vals = "";
            bool f = true;
            foreach (FieldInfo prop in GetType().GetFields())
            {
                r += f ? "" : ",";
                vals += f ? "" : ",";
                f = false;

                r += prop.Name;
                if (prop.FieldType.Name.ToLower() == "string" || prop.FieldType.Name.ToLower() == "char")
                {
                    vals += "'"+prop.GetValue(this)+"'";
                } else
                {
                    vals += prop.GetValue(this);
                }
            }

            r += ") VALUES (" + vals + ")";
            db.exec(r);
        }
    }
}
