using IniFileParser.Model;
using ManagedBass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Wobble.Audio;
using Wobble.Audio.Samples;
using Wobble.Logging;

namespace Pulsarc.Skinning
{
    static class Skin
    {
        // Whether or not the Skin is currently loaded.
        public static bool Loaded { get; private set; } = false;

        // The default texture for Pulsarc.
        public static Texture2D DefaultTexture => AssetsManager.Content.Load<Texture2D>("default");

        // A collection of all assets and their textures.
        public static Dictionary<string, Texture2D> Assets { get; private set; }

        // A colleciton of the judges and their textures.
        public static Dictionary<int, Texture2D> Judges { get; private set; }

        // A collection of multiple adjustable screens and the config files for those screens.
        public static Dictionary<string, IniData> Configs { get; private set; }

        // A collection of all the custom sounds in the skin folder and their ids
        public static Dictionary<string, AudioSample> Sounds { get; private set; }

        /// <summary>
        /// Load a skin from the folder name provided.
        /// </summary>
        /// <param name="name">The folder name of the skin to be loaded.</param>
        public static void LoadSkin(string name)
        {
            Assets = new Dictionary<string, Texture2D>();
            Configs = new Dictionary<String, IniData>();
            Judges = new Dictionary<int, Texture2D>();
            Sounds = new Dictionary<string, AudioSample>();
            Loaded = false;

            string skinFolder = $"Skins/{name}/";

            if (Directory.Exists(skinFolder))
            {
                // Load configs
                LoadConfigs(skinFolder);

                // Load gameplay assets
                LoadSkinTexture($"{skinFolder}Gameplay/", "arcs");
                LoadSkinTexture($"{skinFolder}Gameplay/", "crosshair");
                LoadSkinTexture($"{skinFolder}Gameplay/", "map_timer");

                // Load cursor asset
                LoadSkinTexture($"{skinFolder}UI/", "cursor");

                // Load Main Menu assets
                LoadMainMenu(skinFolder);

                // Load Result Screen assets (not including Grades)
                LoadResultScreen(skinFolder);

                // Load Song Select assets
                LoadSongSelect(skinFolder);

                // Load settings assets
                LoadSettings(skinFolder);

                // Load Grade assets
                LoadGrades(skinFolder);

                // Load judge assets
                LoadJudges(skinFolder);

                // Load sounds
                LoadSounds(skinFolder);

                Loaded = true;
            }
            else
                PulsarcLogger.Error($"Could not find the skin {name}", LogType.Network);
        }

        #region Loading Methods
        private static void LoadConfigs(string skinFolder)
        {
            IniFileParser.IniFileParser parser = new IniFileParser.IniFileParser();

            Configs.Add("skin",             parser.ReadFile($"{skinFolder}skin.ini"));
            Configs.Add("gameplay",         parser.ReadFile($"{skinFolder}Gameplay/gameplay.ini"));
            Configs.Add("main_menu",        parser.ReadFile($"{skinFolder}UI/MainMenu/main_menu.ini"));
            Configs.Add("judgements",       parser.ReadFile($"{skinFolder}Judgements/judgements.ini"));
            Configs.Add("result_screen",    parser.ReadFile($"{skinFolder}UI/ResultScreen/result_screen.ini"));
            Configs.Add("song_select",      parser.ReadFile($"{skinFolder}UI/SongSelect/song_select.ini"));
        }

        private static void LoadMainMenu(string skinFolder)
        {
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "menu_background");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "menu_game_icon");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_back_1");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_back_2");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_back_3");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_back_4");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_back_5");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_hover_1");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_hover_2");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_hover_3");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_hover_4");
            LoadSkinTexture($"{skinFolder}UI/MainMenu/", "button_hover_5");
        }

        private static void LoadResultScreen(string skinFolder)
        {
            LoadSkinTexture($"{skinFolder}UI/ResultScreen/", "result_button_advanced");
            LoadSkinTexture($"{skinFolder}UI/ResultScreen/", "result_button_back");
            LoadSkinTexture($"{skinFolder}UI/ResultScreen/", "result_button_retry");
            LoadSkinTexture($"{skinFolder}UI/ResultScreen/", "result_scorecard");
            LoadSkinTexture($"{skinFolder}UI/ResultScreen/", "result_background");
        }

        private static void LoadSongSelect(string skinFolder)
        {
            LoadSkinTexture($"{skinFolder}UI/SongSelect/", "select_background");
            LoadSkinTexture($"{skinFolder}UI/SongSelect/", "select_button_back");
            LoadSkinTexture($"{skinFolder}UI/SongSelect/", "beatmap_card");
            LoadSkinTexture($"{skinFolder}UI/SongSelect/", "card_diff_bar");
            LoadSkinTexture($"{skinFolder}UI/SongSelect/", "card_diff_fill");
            LoadSkinTexture($"{skinFolder}UI/SongSelect/", "scorecard");
            LoadSkinTexture($"{skinFolder}UI/SongSelect/", "searchbox");
        }

        private static void LoadSettings(string skinFolder)
        {
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_background");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_button_back");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_button_save");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_checkbox");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_checkbox_cross");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_binding");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_binding_focus");

            // Settings categories
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_icon_gameplay");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_icon_audio");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "settings_icon_bindings");

            // Settings elements
            LoadSkinTexture($"{skinFolder}UI/Settings/", "slider_select");
            LoadSkinTexture($"{skinFolder}UI/Settings/", "slider");
        }

        private static void LoadGrades(string skinFolder)
        {
            char letter = 'A';

            for (int i = 1; i < 5; i++)
                LoadSkinTexture($"{skinFolder}Grades/", "grade_" + letter++);

            LoadSkinTexture($"{skinFolder}Grades/", "grade_S");
            LoadSkinTexture($"{skinFolder}Grades/", "grade_X");
        }

        private static void LoadJudges(string skinFolder)
        {
            foreach (JudgementValue judge in Judgement.Judgements)
                Judges.Add(judge.Score, LoadTexture($"{skinFolder}Judgements/", judge.Name));
        }

        private static void LoadSounds(string skinFolder)
        {
            LoadSample($"{skinFolder}Audio/", "hitsound");
        }
        #endregion

        /// <summary>
        /// Load a texture from the path and asset name provided. 
        /// </summary>
        /// <param name="path">The folder location of the texture</param>
        /// <param name="asset">The name of the asset, texture file must be the same name</param>
        private static void LoadSkinTexture(string path, string asset)
        {
            Assets.Add(asset, LoadTexture(path, asset));
        }

        /// <summary>
        /// Add a texture that's cropped using the crop-Vectors provided.
        /// </summary>
        /// <param name="asset">The name of the asset.</param>
        /// <param name="texture">The texture to assign to the asset, after cropping.</param>
        /// <param name="cropHorizontal">The X coordinates of the rectangle used to crop from the texture.</param>
        /// <param name="cropVertical">The Y coordinates of the rectangle used to crop from the texture.</param>
        private static void LoadCropSkinTexture(string asset, Texture2D texture, Vector2 cropHorizontal, Vector2 cropVertical)
        {
            Assets.Add(asset, LoadCropFromTexture(texture, cropHorizontal, cropVertical));
        }

        /// <summary>
        /// Attempts to return the texture found using the folder path and file asset name provided.
        /// </summary>
        /// <param name="path">The folder loaction of the texture.</param>
        /// <param name="asset">The name of the file to load.</param>
        /// <returns>A texture using the image file found, or an uninitialized "Default Texture" if the asset can't be loaded.</returns>
        private static Texture2D LoadTexture(string path, string asset)
        {
            // Try opening as a .png
            try
            {
                return Texture2D.FromStream(Pulsarc.Graphics.GraphicsDevice, File.Open($"{path}/{asset}.png", FileMode.Open));
            }
            catch
            {
                // Try opening as a .jpg
                try
                {
                    return Texture2D.FromStream(Pulsarc.Graphics.GraphicsDevice, File.Open($"{path}/{asset}.jpg", FileMode.Open));
                }
                // Don't load, throw a console error, and return default texture.
                catch
                {
                    PulsarcLogger.Warning($"Failed to load {asset} in {path}, make sure it is a .png or .jpg file!", LogType.Runtime);
                    return DefaultTexture;
                }
            }
        }

        /// <summary>
        /// Creates a cropped texture from a base texture using the coordinates provided.
        /// </summary>
        /// <param name="texture">The texture to be cropped.</param>
        /// <param name="cropHorizontal">The X coordinates of the rectangle used to crop from the texture.</param>
        /// <param name="cropVertical">The Y coordinates of the rectangle used to crop from the texture.</param>
        /// <returns>The cropped area as a new texture.</returns>
        private static Texture2D LoadCropFromTexture(Texture2D texture, Vector2 cropHorizontal, Vector2 cropVertical) 
        {
            // Define the subrectangle bounds
            Rectangle bounds = texture.Bounds;
            bounds.X += (int)cropHorizontal.X;
            bounds.Width -= (int)(cropHorizontal.X + cropHorizontal.Y);

            bounds.Y += (int)cropVertical.X;
            bounds.Height -= (int)(cropVertical.X + cropVertical.Y);

            // Create the new texture receptacle from the subrectangle dimensions
            Texture2D cropped = new Texture2D(Pulsarc.Graphics.GraphicsDevice, bounds.Width, bounds.Height);
            Color[] data = new Color[bounds.Width * bounds.Height];

            // Fill the new texture with the contents of the primary texture's subrectangle
            texture.GetData(0, bounds, data, 0, bounds.Width * bounds.Height);
            cropped.SetData(data);

            return cropped;
        }

        private static string[] acceptedAudioExtensions = new string[]
        {
            ".mp3",
            ".wav",
            ".ogg"
        };


        private static void LoadSample(string path, string name)
        {
            bool fileExists = false;
            string extension = "";

            for (int i = 0; i < acceptedAudioExtensions.Length; i++)
            {
                if (File.Exists(path + name + acceptedAudioExtensions[i]))
                {
                    extension = acceptedAudioExtensions[i];
                    fileExists = true;
                    break;
                }
            }

            if (!fileExists)
            {
                throw new FileNotFoundException("AudioSample not found", path, new AudioEngineException());
            }

            Sounds.Add(name, new AudioSample(path + name + extension));
        }

        /// <summary>
        /// Find the config provided, go to the config-section provided, and return the int value of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The int value found using the provided parameters.</returns>
        public static int GetConfigInt(string config, string section, string key)
        {
            return int.Parse(GetConfigString(config, section, key));
        }

        /// <summary>
        /// Find the config provided, go to the config-section provided, and return the float value of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The float value found using the provided parameters.</returns>
        public static float GetConfigFloat(string config, string section, string key)
        {
            return float.Parse(GetConfigString(config, section, key), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Find the config provided, go to the config-section provided, and return the string value of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The float value found using the provided parameters.</returns>
        public static string GetConfigString(string config, string section, string key)
        {
            return Configs[config][section][key].Replace("\"", string.Empty);
        }

        /// <summary>
        /// Find the config provided, go to the section provided, and return the Anchor of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The Anchor found using the provided parameters.</returns>
        public static Anchor GetConfigAnchor(string config, string section, string key)
        {
            return (Anchor)Enum.Parse(Anchor.TopLeft.GetType(), GetConfigString(config, section, key));
        }

        /// <summary>
        /// Find the config provided, go to the section provided, and return the start position of the
        /// key provided. If parent is not null, the start position will be based on the parent.
        /// If parent is null, start position will be based on the screen.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <param name="parent">The parant Drawable to base the start position off of.
        /// Default value is null for screen-based positioning.</param>
        /// <returns>
        /// The position found using the provided parameters.
        /// </returns>
        public static Vector2 GetConfigStartPosition(string config, string section, string key, in Drawable parent = null)
        {
            Anchor anchor = Anchor.TopLeft;

            return GetConfigStartPosition(config, section, key, out anchor, parent);
        }

        /// <summary>
        /// Find the config provided, go to the section provided, and return the start position of the
        /// key provided. If parent is not null, the start position will be based on the parent.
        /// If parent is null, start position will be based on the screen.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <param name="parent">The parant Drawable to base the start position off of.</param>
        /// <param name="anchor">Out reference anchor if whatever calling this needs it.</param>
        /// <returns>
        /// The position found using the provided parameters.
        /// </returns>
        public static Vector2 GetConfigStartPosition(string config, string section, string key, out Anchor anchor, in Drawable parent = null)
        {
            anchor = GetConfigAnchor(config, section, key);

            // If there's a parent, return the start position relative to the parent,
            // otherwise find the start position relative to the screen.
            return parent == null
                ? AnchorUtil.FindScreenPosition(anchor)
                : AnchorUtil.FindDrawablePosition(anchor, parent);
        }

        /// <summary>
        /// Find the config provided, go to the section provided, and return the Color of the
        /// key provided. Format like this: "r, g, b, [a]"
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The Color found using the provided parameters.</returns>
        public static Color GetConfigColor(string config, string section, string key)
        {
            string valueToParse = GetConfigString(config, section, key);

            var parts = valueToParse.Split(',');
            int r, g, b = 0;
            int a = 255;

            try
            {
                r = int.Parse(parts[0]);
                g = int.Parse(parts[1]);
                b = int.Parse(parts[2]);

                if (parts.Length > 3)
                    a = int.Parse(parts[3]);
            }
            // If the color wasn't formatted right, display a console error and return black instead
            catch (Exception e)
            {
                PulsarcLogger.Error($"{key} was not formatted correctly." +
                    $"\nPlease format {key} with \"{{red}},{{green}},{{blue}},[alpha]\", where alpha is optional:" +
                    $"\n Each value can be from 0 to 255. For example for Black color write {key} =0,0,0,255." +
                    $"\nBecause the format was incorrect, the Color for {key} will be Black instead.", LogType.Runtime);
                r = 0;
                g = 0;
                b = 0;
            }

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Clear all skin textures.
        /// </summary>
        public static void Unload()
        {
            // TODO: clear all skin textures
        }
    }
}
