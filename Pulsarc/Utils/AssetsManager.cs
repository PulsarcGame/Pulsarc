using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;

namespace Pulsarc.Utils
{
    static class AssetsManager
    {
        static private ContentManager content;
        static public ContentManager Content { get => content; }
        static public Dictionary<String, SpriteFont> fonts { get; set; }

        static public void Initialize(ContentManager Content)
        {
            content = Content;

            fonts = new Dictionary<String, SpriteFont>();

            fonts.Add("DefaultFont", Content.Load<SpriteFont>("Fonts/rawline-600"));
            
            Skin.LoadSkin("DefaultSkin");
        }

        static public void Unload()
        {
            Skin.Unload();
        }
    }
}
