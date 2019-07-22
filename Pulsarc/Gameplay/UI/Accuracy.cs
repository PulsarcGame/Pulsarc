using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay.UI
{
    class Accuracy : TextDisplayElement
    {
        public Accuracy(Vector2 position, bool centered = false) : base("", position, centered: centered)
        {
        }

        public void Update(double value)
        {
            Update(Math.Round(value * 100,2) + "%");
        }
    }
}
