using IniParser;
using IniParser.Model;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Pulsarc.Utils
{
    static class Config
    {
        private static FileIniDataParser parser;
        public static IniData iniData;

        public static Dictionary<string, Keys> bindings;

        private static Type keyType = Keys.A.GetType();

        public static void Initialize()
        {
            parser = new FileIniDataParser();
            bindings = new Dictionary<string, Keys>();

            iniData = parser.ReadFile("config.ini");

            AddBinding("Left");
            AddBinding("Up");
            AddBinding("Down");
            AddBinding("Right");

            AddBinding("Pause");
            AddBinding("Continue");
        }

        public static void Reload()
        {
            bindings.Clear();
            iniData = parser.ReadFile("config.ini");

            AddBinding("Left");
            AddBinding("Up");
            AddBinding("Down");
            AddBinding("Right");

            AddBinding("Pause");
            AddBinding("Continue");
        }


        private static void AddBinding(string key)
        {
            bindings.Add(key, (Keys)Enum.Parse(keyType, iniData["Bindings"][key]));
        }

        public static int GetInt(string category, string key)
        {
            return int.Parse(iniData[category][key]);
        }

        public static void SetInt(string category, string key, int value)
        {
            iniData[category][key] = value.ToString();
            parser.WriteFile("config.ini", iniData);
        }
    }
}
