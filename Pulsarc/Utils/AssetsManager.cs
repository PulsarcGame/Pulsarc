using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System.Collections.Generic;

namespace Pulsarc.Utils
{
    static class AssetsManager
    {
        public static ContentManager Content;
        public static Dictionary<string, SpriteFont> fonts { get; set; }

        public static void Initialize(ContentManager Content)
        {
            AssetsManager.Content = Content;

            fonts = new Dictionary<string, SpriteFont>
            {
                { "DefaultFont", Content.Load<SpriteFont>("Fonts/rawline-600") }
            };

            Skin.LoadSkin("DefaultSkin");
            Skin.defaultTexture = Content.Load<Texture2D>("default");
        }

        public static void Unload()
        {
            Skin.Unload();
        }
    }
}