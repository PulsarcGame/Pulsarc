using Microsoft.Xna.Framework;
using System;

namespace Pulsarc.UI.Screens.Gameplay.UI
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
