using Microsoft.Xna.Framework;
using System;

namespace Pulsarc.UI.Screens.Result.UI
{
    class JudgeCount : TextDisplayElement
    {
        public string name;

        public JudgeCount(string name, Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft)
            : base("", position, fontSize, anchor, color)
        {
            this.name = name;
        }

        public void Update(int value)
        {
            Update(value.ToString("#,#0"));
        }
    }

    class Accuracy : TextDisplayElement
    {
        public Accuracy(Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft)
            : base("", position, fontSize, anchor, color) { }

        public void Update(double value)
        {
            Update(Math.Round(value * 100, 2).ToString("#,##.00") + "%");
        }
    }

    class Combo : TextDisplayElement
    {
        public Combo(Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft)
            : base("", position, fontSize, anchor, color) { }

        public void Update(int value)
        {
            Update(value.ToString("#,#0") + "x");
        }
    }

    class Score : TextDisplayElement
    {
        public Score(Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft)
            : base("", position, fontSize, anchor, color) { }

        public void Update(long value)
        {
            Update(value.ToString("#,#0"));
        }
    }
}
