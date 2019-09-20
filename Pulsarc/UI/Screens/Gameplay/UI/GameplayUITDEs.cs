using Microsoft.Xna.Framework;
using System;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Accuracy : TextDisplayElementFixedSize
    {
        public Accuracy(Vector2 position, int fontSize = 18, Anchor anchor = Anchor.Center, Color? color = null)
            : base("", position, "%", fontSize, anchor, color)
        {
            numberFormat = "#,##.00";
        }

        public new void Update(double value)
        {
            base.Update(Math.Round(value * 100, 2));
        }
    }

    class Score : TextDisplayElementFixedSize
    {
        public Score(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.Center, Color? color = null)
            : base("", position, "", fontSize, anchor, color) { }
    }

    class Combo : TextDisplayElementFixedSize
    {
        public Combo(Vector2 position, int fontSize = 14, Anchor anchor = Anchor.Center, Color? color = null)
            : base("", position, "x", fontSize, anchor, color) { }
    }
}
