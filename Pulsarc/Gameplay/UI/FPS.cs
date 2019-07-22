using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay.UI
{
    class FPS : TextDisplayElement
    {
        public FPS(Vector2 position, int fontSize = 10, bool centered = false) : base("", position, fontSize, centered)
        {
        }

        public void Update(int value)
        {
            Update(value.ToString() + "fps");
        }
    }
}
