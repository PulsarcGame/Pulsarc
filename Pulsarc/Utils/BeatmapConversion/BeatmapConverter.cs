using Pulsarc.Beatmaps;
using System.Collections.Generic;

namespace Pulsarc.Utils.BeatmapConversion
{
    interface BeatmapConverter
    {
        List<Beatmap> Convert(string folderPath);
        void Save(string folderPath);
    }
}
