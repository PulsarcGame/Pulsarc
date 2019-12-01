using System.Data.SQLite;
using System.Reflection;
using Wobble.Logging;

namespace Pulsarc.Utils.SQLite
{
    public abstract class SQLiteData
    {
        public SQLiteData() { }

        public SQLiteData(SQLiteDataReader data)
        {
            foreach (FieldInfo prop in GetType().GetFields())
            {
                try
                { 
                    switch(prop.Name.ToLower())
                    {
                        case "datet":
                            prop.SetValue(this, data[prop.Name.ToLower()].ToString());
                            break;
                        default:
                            prop.SetValue(this, data[prop.Name.ToLower()]);
                            break;
                    }
                }
                catch
                {
                    try
                    {
                        PulsarcLogger.Warning($"Data field in SQLite request could not be set as a {info[i].FieldType.Name} : {data[info[i].Name.ToLower()]}", LogType.Runtime);
                    }
                    catch
                    {
                        PulsarcLogger.Warning($"Unexpected data field in SQLite request : {info[i].Name}", LogType.Runtime);
                    }
                }
            }
        }

        public void SaveData(SQLiteStore db)
        {
            string r = $"INSERT INTO {GetType().Name.ToLower()} (";
            string vals = "";
            bool f = true;

            foreach (FieldInfo prop in GetType().GetFields())
            {
                r += f ? "" : ",";
                vals += f ? "" : ",";
                f = false;

                r += prop.Name;
                if (prop.FieldType.Name.ToLower() == "string" || prop.FieldType.Name.ToLower() == "char")
                    vals += "'" + prop.GetValue(this).ToString().Replace("'" , "\'" + "'") + "'";
                else
                    vals += prop.GetValue(this);
            }

            r += $") VALUES ({vals})";
            db.Exec(r);
        }
    }
}
