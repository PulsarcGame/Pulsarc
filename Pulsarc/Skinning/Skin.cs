using IniParser;
using IniParser.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Pulsarc.Skinning
{
    static class Skin
    {
        static private bool loaded = false;

        static public Texture2D defaultTexture;

        static public Dictionary<String, Texture2D> assets { get; set; }
        static public Dictionary<int, Texture2D> judges;
        static public Dictionary<String, IniData> configs { get; set; }

        static public void LoadSkin(string name)
        {
            FileIniDataParser parser = new FileIniDataParser();
            assets = new Dictionary<string, Texture2D>();
            configs = new Dictionary<String, IniData>();
            loaded = false;

            string skinFolder = "Skins/" + name + "/";

            if (Directory.Exists(skinFolder))
            {

                configs.Add("skin", parser.ReadFile(skinFolder + "skin.ini"));
                configs.Add("gameplay", parser.ReadFile(skinFolder + "Gameplay/gameplay.ini"));
                configs.Add("main_menu", parser.ReadFile(skinFolder + "UI/MainMenu/main_menu.ini"));
                configs.Add("judgements", parser.ReadFile(skinFolder + "Judgements/judgements.ini"));
                configs.Add("result_screen", parser.ReadFile(skinFolder + "UI/ResultScreen/result_screen.ini"));

                LoadSkinTexture(skinFolder + "Gameplay/", "arcs");
                LoadSkinTexture(skinFolder + "Gameplay/", "crosshair");

                LoadSkinTexture(skinFolder + "UI/", "cursor");

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

                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_button_advanced");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_button_back");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_button_retry");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_scorecard");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_background");

                LoadSkinTexture(skinFolder + "UI/SongSelect/", "select_background");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "select_button_back");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "beatmap_card");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "card_diff_bar");
                LoadSkinTexture(skinFolder + "UI/SongSelect/", "card_diff_fill");

                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_background");
                LoadSkinTexture(skinFolder + "UI/Settings/", "settings_button_back");

                LoadSkinTexture(skinFolder + "Grades/", "grade_X");
                LoadSkinTexture(skinFolder + "Grades/", "grade_S");
                LoadSkinTexture(skinFolder + "Grades/", "grade_A");
                LoadSkinTexture(skinFolder + "Grades/", "grade_B");
                LoadSkinTexture(skinFolder + "Grades/", "grade_C");
                LoadSkinTexture(skinFolder + "Grades/", "grade_D");


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

        static private void LoadSkinTexture(string path, string asset)
        {
            assets.Add(asset, LoadTexture(path, asset));
        }

        static private void LoadCropSkinTexture(string asset, Texture2D texture, Vector2 cropHorizontal, Vector2 cropVertical)
        {
            assets.Add(asset, LoadCropFromTexture(texture, cropHorizontal, cropVertical));
        }

        static private Texture2D LoadTexture(string path, string asset)
        {
            try
            {
                return Texture2D.FromStream(Pulsarc.graphics.GraphicsDevice, File.Open(path + "/" + asset + ".png", FileMode.Open));
            }
            catch
            {
                Console.WriteLine("Failed to load " + asset + " in " + path);
                return defaultTexture;
            }
        }

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

        static public int getConfigInt(string config, string section, string key)
        {
            return int.Parse(configs[config][section][key]);
        }

        static public float getConfigFloat(string config, string section, string key)
        {
            return float.Parse(configs[config][section][key], CultureInfo.InvariantCulture);
        }

        static public Anchor getConfigAnchor(string config, string section, string key)
        {
            return (Anchor) Enum.Parse(Anchor.TopLeft.GetType(),configs[config][section][key]);
        }

        static public bool isLoaded()
        {
            return loaded;
        }

        static public void Unload()
        {
            // TODO: clear all skin textures
        }
    }
}
