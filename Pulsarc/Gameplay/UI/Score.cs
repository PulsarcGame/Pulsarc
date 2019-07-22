using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay.UI
{
    class Score : TextDisplayElement
    {
        public Score(Vector2 position, int fontSize = 20, bool centered = false) : base("", position, fontSize, centered)
        {
        }

        public void Update(long value)
        {
            Update(value.ToString());
        }
    }
}
