using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Title : TextDisplayElement
    {
        public Title(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor)
        {
        }
    }
    class Artist : TextDisplayElement
    {
        public Artist(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor)
        {
        }
    }
    class Version : TextDisplayElement
    {
        public Version(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor)
        {
        }
    }
    class Mapper : TextDisplayElement
    {
        public Mapper(Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }
    }
}
