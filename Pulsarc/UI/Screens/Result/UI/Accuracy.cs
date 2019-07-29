using Microsoft.Xna.Framework;
using System;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Accuracy : TextDisplayElement
    {
        public Accuracy(Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }

        public void Update(double value)
        {
            Update(Math.Round(value * 100,2).ToString("#,##.00") + "%");
        }
    }
}
