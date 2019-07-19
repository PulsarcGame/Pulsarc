using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Pulsarc.Skinning
{
    static class Skin
    {
        static private bool loaded = false;

        static public Texture2D arcs;
        static public Texture2D crosshair;

        static public Texture2D leftArc;
        static public Texture2D upArc;
        static public Texture2D downArc;
        static public Texture2D rightArc;

        static public void LoadSkin(string name)
        {
            loaded = false;

            string skinFolder = "Skins/" + name + "/";

            if (Directory.Exists(skinFolder))
            {
                arcs        = LoadSkinTexture(skinFolder + "Gameplay/", "arcs.png");
                crosshair   = LoadSkinTexture(skinFolder + "Gameplay/", "crosshair.png");

                float halfW = arcs.Width / 2;
                float halfH = arcs.Height / 2;
                leftArc     = LoadCropFromTexture(arcs, new Vector2(0, halfW), new Vector2(halfH, 0));
                upArc       = LoadCropFromTexture(arcs, new Vector2(0, halfW), new Vector2(0, halfH));
                downArc     = LoadCropFromTexture(arcs, new Vector2(halfW, 0), new Vector2(halfH, 0));
                rightArc    = LoadCropFromTexture(arcs, new Vector2(halfW, 0), new Vector2(0, halfH));

                loaded = true;
            } else
            {
                Console.Write("Could not find the skin " + name);
            }
        }

        static private Texture2D LoadSkinTexture(string path, string asset)
        {
            try
            {
                return Texture2D.FromStream(Pulsarc.graphics.GraphicsDevice, File.Open(path + "/" + asset, FileMode.Open));
            }
            catch
            {
                Console.Write("Failed to load " + asset + " in " + path);
                return new Texture2D(Pulsarc.graphics.GraphicsDevice, 1, 1);
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
