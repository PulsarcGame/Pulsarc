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
            float radius = (200 / 1080) * screen.X;

            Resize(new Vector2(radius, radius));
            origin.X = screen.X / 2;
            origin.Y = screen.Y / 2;

            position.X = origin.X - radius;
            position.Y = origin.Y - radius;
        }
    }
}
