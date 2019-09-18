using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    abstract class Card : Drawable
    {
        public string 

        public static readonly Texture2D StaticTexture;

        protected bool isClicked = false;

        public Card(Vector2 position, Vector2 size, Anchor anchor) : base(Texture(), position, anchor: anchor)
        {

        }

        public static abstract Texture();
    }
}
