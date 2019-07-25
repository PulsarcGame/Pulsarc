using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pulsarc.Skinning
{
    static class Skin
    {
        static private bool loaded = false;

        static public Dictionary<String, Texture2D> assets { get; set; }

        static public Texture2D defaultTexture;

        static public Dictionary<int, Texture2D> judges;

        static public void LoadSkin(string name)
        {
            assets = new Dictionary<string, Texture2D>();
            loaded = false;

            string skinFolder = "Skins/" + name + "/";

            if (Directory.Exists(skinFolder))
            {
                LoadSkinTexture(skinFolder + "Gameplay/", "arcs");
                LoadSkinTexture(skinFolder + "Gameplay/", "crosshair");

                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_button");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_replay");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_return");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_scorecard");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "result_background");
                LoadSkinTexture(skinFolder + "UI/ResultScreen/", "score_grade_container");

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
            Rectangle bounds = texture.Bounds;
            bounds.X += (int)cropHorizontal.X;
            bounds.Width -= (int)(cropHorizontal.X + cropHorizontal.Y);

            bounds.Y += (int)cropVertical.X;
            bounds.Height -= (int)(cropVertical.X + cropVertical.Y);

            Texture2D cropped = new Texture2D(Pulsarc.graphics.GraphicsDevice, bounds.Width, bounds.Height);
            Color[] data = new Color[bounds.Width * bounds.Height];

            texture.GetData(0, bounds, data, 0, bounds.Width * bounds.Height);
            cropped.SetData(data);

            return cropped;
        }

        static public bool isLoaded()
        {
            return loaded;
        }
    }
}
