using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapTitle : TextDisplayElement
    {
        public BeatmapTitle(Vector2 position, int fontSize = 18, bool centered = false) : base("", position, fontSize, centered)
        {
        }
    }
    class BeatmapArtist : TextDisplayElement
    {
        public BeatmapArtist(Vector2 position, int fontSize = 14, bool centered = false) : base("", position, fontSize, centered)
        {
        }
    }
    class BeatmapVersion : TextDisplayElement
    {
        public BeatmapVersion(Vector2 position, int fontSize = 16, bool centered = false) : base("", position, fontSize, centered)
        {
        }
    }
    class BeatmapMapper : TextDisplayElement
    {
        public BeatmapMapper(Vector2 position, int fontSize = 16, bool centered = false) : base("", position, fontSize, centered)
        {
        }
    }
}
