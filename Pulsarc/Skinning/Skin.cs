using IniParser;
using IniParser.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.UI;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Wobble.Logging;

namespace Pulsarc.Skinning
{
    static class Skin
    {
        static private bool loaded = false;

        static public Texture2D DefaultTexture { get; private set; } = AssetsManager.Content.Load<Texture2D>("default");

        // A collection of all assets and their textures.
        static public Dictionary<String, Texture2D> assets { get; set; }

        // A colleciton of the judges and their textures.
        static public Dictionary<int, Texture2D> judges;

        // A collection of multiple adjustable screens and the config files for those screens.
        static public Dictionary<String, IniData> configs { get; set; }

        /// <summary>
        /// Load a skin in the folder name provided.
        /// </summary>
        /// <param name="name">The folder name of the skin to be loaded.</param>
        static public void LoadSkin(string name)
        {
            FileIniDataParser parser = new FileIniDataParser();
            assets = new Dictionary<string, Texture2D>();
            configs = new Dictionary<String, IniData>();
            loaded = false;

            string skinFolder = "Skins/" + name + "/";

            if (Directory.Exists(skinFolder))
            {
                // Load configs
                configs.Add("skin", parser.ReadFile(skinFolder + "skin.ini"));
                configs.Add("gameplay", parser.ReadFile(skinFolder + "Gameplay/gameplay.ini"));
                configs.Add("main_menu", parser.ReadFile(skinFolder + "UI/MainMenu/main_menu.ini"));
                configs.Add("judgements", parser.ReadFile(skinFolder + "Judgements/judgements.ini"));
                configs.Add("result_screen", parser.ReadFile(skinFolder + "UI/ResultScreen/result_screen.ini"));
                configs.Add("song_select", parser.ReadFile(skinFolder + "UI/SongSelect/song_select.ini"));

                // Load gameplay assets
                LoadSkinTexture(skinFolder + "Gameplay/", "arcs");
                LoadSkinTexture(skinFolder + "Gameplay/", "crosshair");

                // Load cursor asset
                LoadSkinTexture(skinFolder + "UI/", "cursor");

                // Load Main Menu assets
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "menu_background");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "menu_game_icon");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_back_1");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_back_2");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_back_3");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_back_4");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_back_5");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_hover_1");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_hover_2");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_hover_3");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_hover_4");
                LoadSkinTexture(skinFolder + "UI/MainMenu/", "button_hover_5");

                // Load Result Screen assets (not including Grades)
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_button_advanced");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_button_back");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_button_retry");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_scorecard");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_background");

                // Load Song Select assets
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "select_background");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "select_button_back");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "beatmap_card");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "card_diff_bar");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "card_diff_fill");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "scorecard");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "searchbox");

                // Load settings assets
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_background");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_button_back");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_button_save");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_checkbox");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_checkbox_cross");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_binding");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_binding_focus");
                // Settings categories
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_icon_gameplay");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_icon_audio");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_icon_bindings");
                // Settings elements
                LoadSkinTexture(skinFolder + "UI/Settings/", "slider_select");
                LoadSkinTexture(skinFolder + "UI/Settings/", "slider");

                // Load Grade assets
                LoadSkinTexture(skinFolder + "Grades/", "grade_X");
                LoadSkinTexture(skinFolder + "Grades/", "grade_S");
                LoadSkinTexture(skinFolder + "Grades/", "grade_A");
                LoadSkinTexture(skinFolder + "Grades/", "grade_B");
                LoadSkinTexture(skinFolder + "Grades/", "grade_C");
                LoadSkinTexture(skinFolder + "Grades/", "grade_D");


                // Load judge assets
                judges = new Dictionary<int, Texture2D>();

                judges.Add(320, LoadTexture(skinFolder + "Judgements/", "max"));
                judges.Add(300, LoadTexture(skinFolder + "Judgements/", "perfect"));
                judges.Add(200, LoadTexture(skinFolder + "Judgements/", "great"));
                judges.Add(100, LoadTexture(skinFolder + "Judgements/", "good"));
                judges.Add(50, LoadTexture(skinFolder + "Judgements/", "bad"));
                judges.Add(0, LoadTexture(skinFolder + "Judgements/", "miss"));

                loaded = true;
            } else
            {
                Console.WriteLine("Could not find the skin " + name);
            }
        }

        /// <summary>
        /// Load a texture from the path and asset name provided. 
        /// </summary>
        /// <param name="path">The folder location of the texture</param>
        /// <param name="asset">The name of the asset, texture file must be the same name</param>
        static private void LoadSkinTexture(string path, string asset)
        {
            assets.Add(asset, LoadTexture(path, asset));
        }

        /// <summary>
        /// Add a texture that's cropped using the crop-Vectors provided.
        /// </summary>
        /// <param name="asset">The name of the asset.</param>
        /// <param name="texture">The texture to assign to the asset, after cropping.</param>
        /// <param name="cropHorizontal">The X coordinates of the rectangle used to crop from the texture.</param>
        /// <param name="cropVertical">The Y coordinates of the rectangle used to crop from the texture.</param>
        static private void LoadCropSkinTexture(string asset, Texture2D texture, Vector2 cropHorizontal, Vector2 cropVertical)
        {
            assets.Add(asset, LoadCropFromTexture(texture, cropHorizontal, cropVertical));
        }

        /// <summary>
        /// Attempts to return the texture found using the folder path and file asset name provided.
        /// </summary>
        /// <param name="path">The folder loaction of the texture.</param>
        /// <param name="asset">The name of the file to load.</param>
        /// <returns>A texture using the image file found, or an uninitialized "Default Texture" if the asset can't be loaded.</returns>
        static private Texture2D LoadTexture(string path, string asset)
        {
            try
            {
                return Texture2D.FromStream(Pulsarc.graphics.GraphicsDevice, File.Open(path + "/" + asset + ".png", FileMode.Open));
            }
            catch
            {
                try
                {
                    return Texture2D.FromStream(Pulsarc.graphics.GraphicsDevice, File.Open(path + "/" + asset + ".jpg", FileMode.Open));
                }
                catch
                {
                    Console.WriteLine("Failed to load " + asset + " in " + path);
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
        static private Texture2D LoadCropFromTexture(Texture2D texture, Vector2 cropHorizontal, Vector2 cropVertical) 
        {
            // Create a texture from a subrectangle in another texture

            // Define the subrectangle bounds
            Rectangle bounds = texture.Bounds;
            bounds.X += (int)cropHorizontal.X;
            bounds.Width -= (int)(cropHorizontal.X + cropHorizontal.Y);

            bounds.Y += (int)cropVertical.X;
            bounds.Height -= (int)(cropVertical.X + cropVertical.Y);

            // Create the new texture receptacle from the subrectangle dimensions
            Texture2D cropped = new Texture2D(Pulsarc.graphics.GraphicsDevice, bounds.Width, bounds.Height);
            Color[] data = new Color[bounds.Width * bounds.Height];

            // Fill the new texture with the contents of the primary texture's subrectangle
            texture.GetData(0, bounds, data, 0, bounds.Width * bounds.Height);
            cropped.SetData(data);

            return cropped;
        }

        /// <summary>
        /// Find the config provided, go to the config-section provided, and return the int value of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The int value found using the provided parameters.</returns>
        static public int getConfigInt(string config, string section, string key)
        {
            return int.Parse(getConfigString(config, section, key));
        }

        /// <summary>
        /// Find the config provided, go to the config-section provided, and return the float value of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The float value found using the provided parameters.</returns>
        static public float getConfigFloat(string config, string section, string key)
        {
            return float.Parse(getConfigString(config, section, key), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Find the config provided, go to the config-section provided, and return the string value of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The float value found using the provided parameters.</returns>
        static public string getConfigString(string config, string section, string key)
        {
            return configs[config][section][key].Replace("\"", string.Empty);
        }

        /// <summary>
        /// Find the config provided, go to the section provided, and return the Anchor of the key provided.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The Anchor found using the provided parameters.</returns>
        static public Anchor getConfigAnchor(string config, string section, string key)
        {
            return (Anchor)Enum.Parse(Anchor.TopLeft.GetType(), getConfigString(config, section, key));
        }

        /// <summary>
        /// Find the config provided, go to the section provided, and return the start position of the
        /// key provided. If parent is not null, the start position will be based on the parent.
        /// If parent is null, start position will be based on the screen.
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>
        /// The position found using the provided parameters.
        /// Currently only ScreenAnchor and pre-determined parent position finding is supported.
        /// </returns>
        static public Vector2 getConfigStartPosition(string config, string section, string key, Drawable parent = null)
        {
            string anchorString = getConfigString(config, section, key);

            // If there's a parent, return the start position relative to the parent,
            // otherwise find the start position relative to the screen.
            return parent == null
                ? AnchorUtil.FindScreenPosition((Anchor)Enum.Parse(Anchor.TopLeft.GetType(), anchorString))
                : AnchorUtil.FindDrawablePosition((Anchor)Enum.Parse(Anchor.TopLeft.GetType(), anchorString), parent);
        }

        /// <summary>
        /// Find the config provided, go to the section provided, and return the Color of the
        /// key provided. Format like this: "r, g, b, [a]"
        /// </summary>
        /// <param name="config">The config to look in.</param>
        /// <param name="section">The section of a config to look in.</param>
        /// <param name="key">The name of the variable.</param>
        /// <returns>The Color found using the provided parameters.</returns>
        static public Color getConfigColor(string config, string section, string key)
        {
            string valueToParse = getConfigString(config, section, key);

            var parts = valueToParse.Split(',');
            int r, g, b = 0;
            int a = 255;

            try
            {
                r = int.Parse(parts[0]);
                g = int.Parse(parts[1]);
                b = int.Parse(parts[2]);

                if (parts.Length > 3)
                {
                    a = int.Parse(parts[3]);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{key} was not formatted correctly." +
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
        /// Is a skin loaded?
        /// </summary>
        /// <returns>Whether a skin is loaded.</returns>
        static public bool isLoaded()
        {
            return loaded;
        }

        /// <summary>
        /// Clear all skin textures.
        /// </summary>
        static public void Unload()
        {
            // TODO: clear all skin textures
        }
    }
}
