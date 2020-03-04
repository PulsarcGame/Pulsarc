using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    public class EditorSongSelectView : SongSelectionView
    {
        public EditorSongSelectView(Screen screen, List<Beatmap> beatmaps, string search = "")
            : base(screen, beatmaps, search) { }

        protected override BeatmapCard CreateCard(Beatmap beatmap, int index)
            => new EditCard(beatmap, index);
    }
}
