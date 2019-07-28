using Microsoft.Xna.Framework;
using System;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Accuracy : TextDisplayElementFixedSize
    {
        public Accuracy(Vector2 position, bool centered = false) : base("", position, "%", 18, centered: centered)
        {
        }

        public new void Update(double value)
        {
            base.Update(Math.Round(value * 100,2));
        }
    }
}
