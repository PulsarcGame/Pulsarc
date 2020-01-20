using System.Collections.Generic;
using Pulsarc.Beatmaps;

namespace Pulsarc.Utils.BeatmapConversion
{
    interface IBeatmapConverter
    {
        /// <summary>
        /// Convert a folder of beatmaps to Pulsarc-compatible beatmaps
        /// </summary>
        /// <param name="folderPath">The path to the maps-to-be-converted folder</param>
        /// <returns>A list containing converted beatmaps found from the folder.</returns>
        List<Beatmap> Convert(string folderPath);

        /// <summary>
        /// Convert a folder of beatmaps to Pulsarc-compatible beatmaps, and then save the converted Beatmaps to storage.
        /// </summary>
        /// <param name="folderPath">The path to the maps-to-be-converted folder</param>
        void Save(string folderPath);
    }
}
