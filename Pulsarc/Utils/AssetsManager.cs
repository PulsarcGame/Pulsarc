using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;

namespace Pulsarc.Utils
{
    static class AssetsManager
    {
        private const double TextureExpireTimeMs = 600000;

        public static ContentManager Content { get; private set; }
        public static Dictionary<string, SpriteFont> Fonts { get; set; }

        private static Dictionary<string, KeyValuePair<Texture2D, double>> StoredTexture { get; set; }

        public static void Initialize(ContentManager content)
        {
            Content = content;

            Fonts = new Dictionary<String, SpriteFont> {{"DefaultFont", Content.Load<SpriteFont>("Fonts/rawline-600")}};

            Fonts["DefaultFont"].DefaultCharacter = '?'; // Prevents crashing with invalid characters
            
            Skin.LoadSkin("DefaultSkin");

            StoredTexture = new Dictionary<String, KeyValuePair<Texture2D, double>>();
        }

        public static Texture2D Load(string path)
        {
            if (StoredTexture.ContainsKey(path))
                if (StoredTexture[path].Value + TextureExpireTimeMs > PulsarcTime.CurrentElapsedTime)
                    return StoredTexture[path].Key;
                else
                    StoredTexture.Remove(path);

            if (!File.Exists(path)) return null;
            var newTexture = Texture2D.FromStream(Pulsarc.Graphics.GraphicsDevice, File.OpenRead(path));
            StoredTexture.Add(path, new KeyValuePair<Texture2D, double>(newTexture, PulsarcTime.CurrentElapsedTime));

            return newTexture;
        }

        public static void Unload()
        {
            Skin.Unload();
        }
    }
}
