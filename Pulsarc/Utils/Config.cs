using IniFileParser.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wobble.Bindables;
using Wobble.Logging;

namespace Pulsarc.Utils
{
    static class Config
    {
        public static IniFileParser.IniFileParser Parser { get; private set; }
        public static IniData ConfigData { get; private set; }

        // Available options

        // [Graphics]

        /// <summary>
        /// The Width of the game window
        /// </summary>
        internal static Bindable<int> ResolutionWidth { get; private set; }

        /// <summary>
        /// The Height of the game window
        /// </summary>
        internal static Bindable<int> ResolutionHeight { get; private set; }

        /// <summary>
        /// Screen mode. 0 = Windowed, 1 = FullScreen, 2 = Borderless
        /// </summary>
        internal static Bindable<int> FullScreen { get; private set; }

        /// <summary>
        /// Whether VSync is enabled.
        /// </summary>
        internal static Bindable<bool> VSync { get; private set; }

        /// <summary>
        /// The maximum amount of FPS
        /// </summary>
        internal static Bindable<int> FPSLimit { get; private set; }

        // [Bindings]

        /// <summary>
        /// Gameplay keybinds
        /// </summary>
        internal static Bindable<Keys> Left { get; private set; }
        internal static Bindable<Keys> Up { get; private set; }
        internal static Bindable<Keys> Down { get; private set; }
        internal static Bindable<Keys> Right { get; private set; }

        /// <summary>
        /// Other keybinds
        /// </summary>
        internal static Bindable<Keys> Pause { get; private set; }
        internal static Bindable<Keys> Continue { get; private set; }
        internal static Bindable<Keys> Retry { get; private set; }
        internal static Bindable<Keys> Convert { get; private set; }
        internal static Bindable<Keys> Screenshot { get; private set; }

        // [Audio]

        /// <summary>
        /// Audio Main volume
        /// </summary>
        internal static Bindable<int> MusicVolume { get; private set; }

        /// <summary>
        /// Effect volume (Hitsounds)
        /// </summary>
        internal static Bindable<int> EffectVolume { get; private set; }

        /// <summary>
        /// Global audio offset 
        /// </summary>
        internal static Bindable<int> GlobalOffset { get; private set; }

        /// <summary>
        /// Whether to pitch the audio with rates
        /// </summary>
        internal static Bindable<bool> RatePitch { get; private set; }

        /// <summary>
        /// Multiplier for map speed
        /// </summary>
        internal static Bindable<float> SongRate { get; private set; }

        /// <summary>
        /// Notes speed
        /// </summary>
        internal static Bindable<double> ApproachSpeed { get; private set; }

        /// <summary>
        /// How many notes that must have passed before hearing another Miss Sound
        /// </summary>
        internal static Bindable<int> MissSoundThreshold { get; private set; }

        // [Gameplay]

        /// <summary>
        /// Dimming of the map's background
        /// </summary>
        internal static Bindable<int> BackgroundDim { get; private set; }

        /// <summary>
        /// Fading time with hidden
        /// </summary>
        internal static Bindable<int> FadeTime { get; private set; }

        /// <summary>
        /// Whether Hidden is active or not
        /// </summary>
        internal static Bindable<bool> Hidden { get; private set; }

        /// <summary>
        /// Whether autoplay is active
        /// </summary>
        internal static Bindable<bool> Autoplay { get; private set; }

        // [Profile]

        /// <summary>
        /// Player name
        /// </summary>
        internal static Bindable<string> Username { get; private set; }

        // [Converting]

        /// <summary>
        /// The game from which the convertion takes place
        /// </summary>
        internal static Bindable<string> Game { get; private set; }

        /// <summary>
        /// The path leading to the map to convert
        /// </summary>
        internal static Bindable<string> Path { get; private set; }

        /// <summary>
        /// Whatever this is
        /// </summary>
        internal static Bindable<string> BGImage { get; private set; }

        /// <summary>
        ///     Dictates whether or not this is the first write of the file for the current game session.
        ///     (Not saved in Config)
        /// </summary>
        private static bool FirstWrite { get; set; }

        public static void Initialize()
        {
            Parser = new IniFileParser.IniFileParser();

            Reload();
        }

        public static void Reload()
        {
            ConfigData = Parser.ReadFile("config.ini");

            foreach (KeyData element in ConfigData["Config"])
            {
                PulsarcLogger.Debug("CONFIG - " + element.KeyName + ":" + element.Value);
            }

            // [Graphics]

            ResolutionWidth = ReadValue(@"ResolutionWidth", GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
            ResolutionHeight = ReadValue(@"ResolutionHeight", GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            FullScreen = ReadValue(@"FullScreen", 0);
            VSync = ReadValue(@"VSync", false);
            FPSLimit = ReadValue(@"FPSLimit", 1000);

            // [Bindings]

            Left = ReadValue(@"Left", Keys.D);
            Up = ReadValue(@"Up", Keys.F);
            Down = ReadValue(@"Down", Keys.J);
            Right = ReadValue(@"Right", Keys.K);

            Pause = ReadValue(@"Pause", Keys.P);
            Continue = ReadValue(@"Continue", Keys.O);
            Retry = ReadValue(@"Retry", Keys.OemTilde);
            Convert = ReadValue(@"Convert", Keys.F1);
            Screenshot = ReadValue(@"Screenshot", Keys.F11);

            // [Audio]

            MusicVolume = ReadValue(@"MusicVolume", 50);
            EffectVolume = ReadValue(@"EffectVolume", 50);
            GlobalOffset = ReadValue(@"GlobalOffset", 0);
            RatePitch = ReadValue(@"RatePitch", true);

            MissSoundThreshold = ReadValue(@"MissSoundThreshold", 10);

            // [Gameplay]

            SongRate = ReadValue(@"SongRate", 1f);
            ApproachSpeed = ReadValue(@"ApproachSpeed", 25d);
            BackgroundDim = ReadValue(@"BackgroundDim", 70);
            FadeTime = ReadValue(@"FadeTime", 200);
            Hidden = ReadValue(@"Hidden", false);
            Autoplay = ReadValue(@"Autoplay", false);

            // [Profile]

            Username = ReadValue(@"Username", "Player");

            // [Converting]

            Game = ReadValue(@"Game", "Intralism");
            Path = ReadValue(@"Path", "D:\\SteamLibrary\\steamapps\\common\\Intralism\\Editor\\TristamOnceAgain");
            BGImage = ReadValue(@"BGImage", "");
        }

        // Copied and pasted from https://github.com/Quaver/Quaver/blob/282e27cc081dc3d4839c316f958d3821535362fd/Quaver.Shared/Config/ConfigManager.cs#L827
        /// <summary>
        ///     Reads a Bindable<T>. Works on all types.
        /// </summary>
        /// <returns></returns>
        private static Bindable<T> ReadValue<T>(string name, T defaultVal)
        {
            Bindable<T> binded = new Bindable<T>(name, defaultVal);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

            // Attempt to parse the value and default it if it can't.
            try
            {
                binded.Value = (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, ConfigData["Config"][name]);
                binded.ValueChanged += AutoSaveConfiguration;
            }
            catch (Exception e)
            {
                binded.Value = defaultVal;
                binded.ValueChanged += AutoSaveConfiguration;
                PulsarcLogger.Error(e.ToString());
            }

            return binded;
        }

        // Copied and pasted from https://github.com/Quaver/Quaver/blob/282e27cc081dc3d4839c316f958d3821535362fd/Quaver.Shared/Config/ConfigManager.cs#L927

        /// <summary>
        ///     Config Autosave functionality for Bindable<T>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="d"></param>
        private static void AutoSaveConfiguration<T>(object sender, BindableValueChangedEventArgs<T> d)
        {
            // ReSharper disable once ArrangeMethodOrOperatorBody
            CommonTaskScheduler.Add(CommonTask.WriteConfig);
        }


        // Copied and pasted from https://github.com/Quaver/Quaver/blob/c7a87e0a3ec5e5a04f6e8f9a42900ea5f9c783b6/Quaver.Shared/Config/ConfigManager.cs#L1016
        /// <summary>
        ///     Takes all of the current values from the ConfigManager class and creates a file with them.
        ///     This will automatically be called whenever a configuration value is changed in the code.
        /// </summary>
        internal static async Task SaveConfig()
        {
            // Tracks the number of attempts to write the file it has made.
            int attempts = 0;

            // Don't do anything if the file isn't ready.
            while (!IsFileReady("config.ini") && !FirstWrite)
            {
            }

            StringBuilder sb = new StringBuilder();

            // Top file information
            // sb.AppendLine("; Quaver Configuration File");
            sb.AppendLine("; Last Updated On: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
            sb.AppendLine("[Config]");
            sb.AppendLine("; Pulsarc Configuration Values");

            // For every line we want to append "PropName = PropValue" to the string
            foreach (PropertyInfo prop in typeof(Config).GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (prop.Name == "FirstWrite")
                {
                    continue;
                }

                switch(prop.Name)
                {
                    case "Game":
                        sb.AppendLine("; The Game to convert from. <Intralism | Mania>");
                        break;
                    case "Path":
                        sb.AppendLine("; The path to the map files. Example : D:\\SteamLibrary\\steamapps\\common\\Intralism\\Editor\\TristamOnceAgain");
                        break;
                    case "BGImage":
                        sb.AppendLine("; Pulsarc will automatically grab the first image of a non-animated Intralism map.");
                        sb.AppendLine("; To override this, type in the image name (with extension) below");
                        sb.AppendLine("; i.e. \"background2.png\"");
                        break;
                    case "SongRate":
                        sb.AppendLine("; How fast gameplay is. 1 = 100% speed, 1.5 = 150% speed, etc.");
                        break;
                    case "ApproachSpeed":
                        sb.AppendLine("; How fast the arcs approach the crosshair. (Does not influence judgement accuracy)");
                        break;
                    case "FadeTime":
                        sb.AppendLine("; The time (in ms) it takes for a hit arc to fade away. 0 means the arc will dissapear when hit.");
                        break;
                    case "GlobalOffset":
                        sb.AppendLine("; Offset (in ms). Positive values mean arcs appear later, negative mean arcs appear sooner.");
                        break;
                    case "RatePitch":
                        sb.AppendLine("; Set to true to let Song Rate influence the pitch (slow rates lower the pitch, fast rates raise the pitch");
                        break;
                    case "MissSoundThreshold":
                        sb.AppendLine("; How many notes should pass before you hear another \"miss\" sound. Set to 0 to hear a sound for every miss.");
                        break;
                    case "Pause":
                        sb.AppendLine("; The keys to pause and resume gameplay");
                        break;
                    case "Retry":
                        sb.AppendLine("; Instantly restarts the current map. OemTilde = `");
                        break;
                    case "Convert":
                        sb.AppendLine("; Converst the map specified in the \"Convert\" section at the top of this file");
                        break;
                    case "Screenshot":
                        sb.AppendLine("; Take a screenshot of the game and saves it to the Screenshots folder");
                        break;
                }

                try
                {
                    sb.AppendLine(prop.Name + " = " + prop.GetValue(null).ToString());
                }
                catch (Exception e)
                {
                    PulsarcLogger.Error(e.ToString());
                    sb.AppendLine(prop.Name + " = ");
                }
            }

            try
            {
                // Create a new stream
                using (StreamWriter sw = new StreamWriter("config.ini")
                {
                    AutoFlush = true
                })
                {

                    // Write to file and close it.;
                    await sw.WriteLineAsync(sb.ToString());
                    sw.Close();
                }

                FirstWrite = false;
            }
            catch (Exception)
            {
                // Try to write the file again 3 times.
                while (attempts != 2)
                {
                    attempts++;

                    // Create a new stream
                    StreamWriter sw = new StreamWriter("config.ini")
                    {
                        AutoFlush = true
                    };

                    // Write to file and close it.
                    await sw.WriteLineAsync(sb.ToString());
                    sw.Close();
                }

                // If too many attempts were made.
                if (attempts == 2)
                {
                    PulsarcLogger.Error("Too many write attempts to the config file have been made.", LogType.Runtime);
                }
            }
        }


        /// <summary>
        ///     Checks if the file is ready to be written to.
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public static bool IsFileReady(string sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return (inputStream.Length > 0);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
