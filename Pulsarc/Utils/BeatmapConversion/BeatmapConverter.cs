using Pulsarc.Beatmaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Utils.BeatmapConversion
{
    interface BeatmapConverter
    {
        /// <summary>
        /// Convert a folder of beatmaps to Pulsarc-compatible beatmaps
        /// </summary>
        /// <param name="folder_path">The path to the maps-to-be-converted folder</param>
        /// <returns>A list containing converted beatmaps found from the folder.</returns>
        List<Beatmap> Convert(string folder_path);

        /// <summary>
        /// Convert a folder of beatmaps to Pulsarc-compatible beatmaps, and then save the converted Beatmaps to storage.
        /// </summary>
        /// <param name="folder_path">The path to the maps-to-be-converted folder</param>
        void Save(string folder_path);
    }
}
