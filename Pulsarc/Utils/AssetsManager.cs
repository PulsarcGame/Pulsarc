using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pulsarc.Utils
{
    static class AssetsManager
    {
        public static double TextureExpireTimeMs = 600000;

        public static ContentManager Content { get; private set; }
        public static Dictionary<string, SpriteFont> Fonts { get; set; }

        public static Dictionary<string, KeyValuePair<Texture2D, double>> StoredTexture { get; set; }

        public static void Initialize(ContentManager content)
        {
            Content = content;

            Fonts = new Dictionary<String, SpriteFont>();

            Fonts.Add("DefaultFont", Content.Load<SpriteFont>("Fonts/rawline-600"));
            
            Skin.LoadSkin("DefaultSkin");

            StoredTexture = new Dictionary<String, KeyValuePair<Texture2D, double>>();
        }

        public static Texture2D Load(string path)
        {
            if (StoredTexture.ContainsKey(path) && StoredTexture[path].Value + TextureExpireTimeMs > PulsarcTime.CurrentElapsedTime)
                return StoredTexture[path].Key;

            Texture2D newTexture = null;

            if (File.Exists(path))
            {
                newTexture = Texture2D.FromStream(Pulsarc.Graphics.GraphicsDevice, File.OpenRead(path));
                StoredTexture.Add(path, new KeyValuePair<Texture2D, double>(newTexture, PulsarcTime.CurrentElapsedTime));
            }

            return newTexture;
        }

        static public void Unload()
        {
            Skin.Unload();
        }
    }
}
