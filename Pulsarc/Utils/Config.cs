using IniFileParser.Model;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Wobble.Logging;

namespace Pulsarc.Utils
{
    static class Config
    {
        public static IniFileParser.IniFileParser Parser { get; private set; }
        public static IniData Get { get; private set; }

        public static Dictionary<string, Keys> Bindings { get; private set; }

        private static readonly Type keyType = Keys.A.GetType();

        public static void Initialize()
        {
            Parser = new IniFileParser.IniFileParser();
            Bindings = new Dictionary<string, Keys>();

            Get = Parser.ReadFile("config.ini");

            AddBindings();
        }

        public static void Reload()
        {
            Bindings.Clear();
            Get = Parser.ReadFile("config.ini");

            AddBindings();
        }

        private static void AddBindings()
        {
            AddBinding("Left");
            AddBinding("Up");
            AddBinding("Down");
            AddBinding("Right");

            AddBinding("Pause");
            AddBinding("Continue");
            AddBinding("Retry");
            AddBinding("Convert");
        }


        public static void AddBinding(string key)
        {
            if (Bindings.ContainsKey(key))
                Bindings[key] = (Keys)Enum.Parse(keyType, Get["Bindings"][key]);
            else
                Bindings.Add(key, (Keys)Enum.Parse(keyType, Get["Bindings"][key]));
        }

        public static int GetInt(string category, string key)
        {
            return int.Parse(Get[category][key]);
        }

        public static void SetInt(string category, string key, int value)
        {
            SetValue(category, key, value);
        }

        public static double GetDouble(string category, string key)
        {
            return double.Parse(Get[category][key]);
        }

        public static void SetDouble(string category, string key, double value)
        {
            SetValue(category, key, value);
        }

        public static float GetFloat(string category, string key)
        {
            return float.Parse(Get[category][key]);
        }

        public static void SetFloat(string category, string key, float value)
        {
            SetValue(category, key, value);
        }

        public static bool GetBool(string category, string key)
        {
            return bool.Parse(Get[category][key]);
        }

        public static void SetBool(string category, string key, bool value)
        {
            SetValue(category, key, value);
        }

        public static string GetString(string category, string key)
        {
            return Get[category][key];
        }

        public static void SetString(string category, string key, string value)
        {
            SetValue(category, key, value);
        }

        public static void SetValue(string category, string key, object value)
        {
            PulsarcLogger.Important($"Set {category} {key} to {value.ToString()}", LogType.Runtime);
            Get[category][key] = value.ToString();
        }

        public static void SaveConfig()
        {
            Parser.WriteFile("config.ini", Get);
            PulsarcLogger.Important("Saved config", LogType.Runtime);
        }
    }
}
