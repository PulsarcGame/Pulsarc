using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay.UI
{
    class Combo : TextDisplayElement
    {
        public Combo(Vector2 position, int fontSize = 14, bool centered = false) : base("", position, fontSize, centered)
        {
        }

        public void Update(int value)
        {
            Update(value + "x");
        }
    }
}
