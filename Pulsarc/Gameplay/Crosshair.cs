using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;

namespace Pulsarc.Gameplay
{
    class Crosshair : Drawable
    {
        public Crosshair() : base(Skin.crosshair)
        {
            Vector2 screen = Pulsarc.getDimensions();
            float radius = 200f / 1920f * screen.X;

            Resize(radius);
            position.X = screen.X / 2 - radius/2;
            position.Y = screen.Y / 2 - radius/2;
        }
    }
}
