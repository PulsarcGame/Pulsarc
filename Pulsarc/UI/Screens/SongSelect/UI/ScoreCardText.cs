using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class ScoreCardScore : TextDisplayElement
    {
        public ScoreCardScore(Vector2 position, Color color, int fontSize = 14, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }

        public void Update(int value)
        {
            Update(value.ToString("#,##"));
        }
    }
    class ScoreCardRank : TextDisplayElement
    {
        public ScoreCardRank(Vector2 position, Color color, int fontSize = 14, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }

        public void Update(int value)
        {
            Update("#" + value);
        }
    }
    class ScoreCardAccuracy : TextDisplayElement
    {
        public ScoreCardAccuracy(Vector2 position, Color color, int fontSize = 14, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }

        public void Update(double value)
        {
            Update(Math.Round(value * 100, 2).ToString("#,##.00") + "%");
        }
    }
    class ScoreCardCombo : TextDisplayElement
    {
        public ScoreCardCombo(Vector2 position, Color color, int fontSize = 14, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }

        public void Update(int value)
        {
            Update("x" + value);
        }
    }
}
