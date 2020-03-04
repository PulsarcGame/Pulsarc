using Pulsarc.Beatmaps;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    public class EditorSongSelect : SongSelection
    {
        protected override ScreenView CreateView(Screen screen, List<Beatmap> beatmaps, string keyword)
            => new EditorSongSelectView(screen, beatmaps, keyword);
    }
}
