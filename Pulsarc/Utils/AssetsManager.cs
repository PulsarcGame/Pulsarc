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
        static public double TextureExpireTimeMs = 600000;

        static public ContentManager Content;
        static public Dictionary<String, SpriteFont> Fonts { get; set; }

        static public Dictionary<String, KeyValuePair<Texture2D, double>> StoredTexture { get; set; }

        static public void Initialize(ContentManager Content)
        {
            content = Content;

            Fonts = new Dictionary<String, SpriteFont>();

            Fonts.Add("DefaultFont", Content.Load<SpriteFont>("Fonts/rawline-600"));
            
            Skin.LoadSkin("DefaultSkin");
            Skin.defaultTexture = Content.Load<Texture2D>("default");

            StoredTexture = new Dictionary<String, KeyValuePair<Texture2D, double>>();
        }

        static public Texture2D Load(string path)
        {
            if (StoredTexture.ContainsKey(path) && StoredTexture[path].Value + TextureExpireTimeMs > PulsarcTime.CurrentElapsedTime)
            {
                return StoredTexture[path].Key;
            }

            Texture2D newTexture = null;

            if (File.Exists(path)) {
                newTexture = Texture2D.FromStream(Pulsarc.graphics.GraphicsDevice, File.OpenRead(path));
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
