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
        Beatmap Convert(string folder_path);
    }
}
