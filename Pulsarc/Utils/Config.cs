using IniParser;
using IniParser.Model;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
            addBinding("Convert");
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
            addBinding("Convert");
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
            setValue(category, key, value);
        }

        static public double getDouble(string category, string key)
        {
            return double.Parse(get[category][key]);
        }

        static public void setDouble(string category, string key, double value)
        {
            setValue(category, key, value);
        }

        static public float getFloat(string category, string key)
        {
            return float.Parse(get[category][key]);
        }

        static public void setFloat(string category, string key, float value)
        {
            setValue(category, key, value);
        }

        static public bool getBool(string category, string key)
        {
            return bool.Parse(get[category][key]);
        }
        
        static public void setBool(string category, string key, bool value)
        {
            setValue(category, key, value);
        }

        static private void setValue(string category, string key, Object value)
        {
            Console.WriteLine("Set " + category + " " + key + " to " + value.ToString());
            get[category][key] = value.ToString();
        }

        static public void saveConfig()
        {
            parser.WriteFile("config.ini", get);
            Console.WriteLine("Saved config");
        }
    }
}
