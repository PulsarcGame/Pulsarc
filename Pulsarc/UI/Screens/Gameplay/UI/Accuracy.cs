using Microsoft.Xna.Framework;
using System;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Accuracy : TextDisplayElementFixedSize
    {
        public Accuracy(Vector2 position, bool centered = false) : base("", position, 7, 18, centered: centered)
        {
        }

        public void Update(double value)
        {
            Update(Math.Round(value * 100,2).ToString(), "%");
        }
    }
}
