using IniParser;
using IniParser.Model;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Utils
{
    static class Config
    {

        static public FileIniDataParser parser;
        static public IniData get;

        static public Dictionary<string, Keys> bindings;

        static private Type keyType = Keys.A.GetType();

        static public void Initialize()
        {
            parser = new FileIniDataParser();
            bindings = new Dictionary<string, Keys>();

            get = parser.ReadFile("config.ini");


            addBinding("Left");
            addBinding("Up");
            addBinding("Down");
            addBinding("Right");

            addBinding("Pause");
            addBinding("Continue");
            addBinding("Retry");
        }

        static public void Reload()
        {
            bindings.Clear();
            get = parser.ReadFile("config.ini");


            addBinding("Left");
            addBinding("Up");
            addBinding("Down");
            addBinding("Right");

            addBinding("Pause");
            addBinding("Continue");
            addBinding("Retry");
        }


        static private void addBinding(string key)
        {
            bindings.Add(key, (Keys)Enum.Parse(keyType, get["Bindings"][key]));
        }

        static public int getInt(string category, string key)
        {
            return int.Parse(get[category][key]);
        }

        static public void setInt(string category, string key, int value)
        {
            get[category][key] = value.ToString();
            parser.WriteFile("config.ini", get);
        }
    }
}
