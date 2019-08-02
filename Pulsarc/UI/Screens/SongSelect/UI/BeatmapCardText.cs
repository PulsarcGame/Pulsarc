using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapTitle : TextDisplayElement
    {
        public BeatmapTitle(Vector2 position, Color color, int fontSize = 18, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }
    }
    class BeatmapArtist : TextDisplayElement
    {
        public BeatmapArtist(Vector2 position, Color color, int fontSize = 14, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }
    }
    class BeatmapVersion : TextDisplayElement
    {
        public BeatmapVersion(Vector2 position, Color color, int fontSize = 16, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }
    }
    class BeatmapMapper : TextDisplayElement
    {
        public BeatmapMapper(Vector2 position, Color color, int fontSize = 16, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }
    }
    class BeatmapDifficulty : TextDisplayElement
    {
        public BeatmapDifficulty(Vector2 position, Color color, int fontSize = 16, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }

        public void Update(double diff)
        {
            Update(string.Format("{0:0.00}", diff));
        }
    }
}
