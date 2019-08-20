using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pulsarc.Beatmaps;
using Newtonsoft.Json;
using System.IO;

namespace Pulsarc.Utils.BeatmapConversion
{
    class IntralismToPulsarc : BeatmapConverter
    {
        // Estimated offset difference between Intralism and Pulsarc
        int msOffset = -80;

        /// <summary>
        /// Convert an Intralism beatmap to a Pulsarc beatmap
        /// </summary>
        /// <param name="folder_path">The path to the Intralism map folder.</param>
        /// <returns>A list containing a single converted Beatmap.</returns>
        public List<Beatmap> Convert(string folder_path)
        {
            List<Beatmap> results = new List<Beatmap>();
            Beatmap result = new Beatmap();

            // See if the provided path exists
            if(Directory.Exists(folder_path))
            {
                string configPath = folder_path + "/config.txt";

                // See if the a "config.txt" file exists
                if (File.Exists(configPath))
                {
                    // Convert the config file to an IntrlaismBeatmap
                    IntralismBeatmap beatmap = JsonConvert.DeserializeObject<IntralismBeatmap>(File.ReadAllText(configPath, Encoding.UTF8));

                    // Fill in the missing metadata
                    result.FormatVersion = "1";
                    result.Mapper = "Intralism";
                    result.Artist = "Unknown";
                    result.Title = beatmap.name;
                    result.Version = "Converted";
                    result.Audio = beatmap.musicFile;
                    result.timingPoints.Add(new TimingPoint(0, 120));

                    // Go through each Intralism Event
                    foreach(Event evt in beatmap.events)
                    {
                        // If the current event is an Arc, convert it to a Pulsarc Arc.
                        if(evt.data[0] == "SpawnObj")
                        {
                            int arc = 0;
                            // Find each direction listed in the current event, and assign the appropriate bit to it.
                            foreach(string direction in evt.data[1].Split(',')[0].Split('[')[1].Split(']')[0].Split('-'))
                            {
                                switch(direction)
                                {
                                    case "Left":
                                        arc |= 1 << 2;
                                        break;
                                    case "Up":
                                        arc |= 1 << 3;
                                        break;
                                    case "Down":
                                        arc |= 1 << 1;
                                        break;
                                    case "Right":
                                        arc |= 1 << 0;
                                        break;
                                }
                            }

                            // Convert the Intralism Event time to the format Pulsarc understands
                            int time = (int) Math.Floor(evt.time * 1000) + msOffset;

                            // Add the converted arc to the Beatmap
                            result.arcs.Add(new Arc(time, arc));
                        }
                    }
                }
            }

            results.Add(result);
            return results;
        }

        /// <summary>
        /// Converts an Intralism beatmap folder to a Pulsarc-compatible beatmap, and then saves the converted Beatmap to storage.
        /// </summary>
        /// <param name="folder_path">The path to the map-to-be-converted folder</param>
        public void Save(string folder_path)
        {
            Beatmap map = Convert(folder_path).First();

            if(map.Audio != null)
            {
                string audioPath = folder_path + "/" + map.Audio;
                if (File.Exists(audioPath))
                {
                    int id = 0;
                    // The folder name will look like "0 - Unknown - MapTitle - (Mapper)"
                    string folderName = string.Join("_", (id + " - " + map.Artist + " - " + map.Title + " (" + map.Mapper + ")").Split(Path.GetInvalidFileNameChars()));
                    string dirName = "Songs/" + folderName;

                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }

                    File.Copy(audioPath, dirName + "/" + map.Audio, true);

                    // The file name will look like "Unknown - MapTitle [Converted] (Mapper).psc"
                    string difficultyFileName = string.Join("_", (map.Artist + " - " + map.Title + " [" + map.Version + "]" + " (" + map.Mapper + ")").Split(Path.GetInvalidFileNameChars()));

                    BeatmapHelper.Save(map, dirName + "/" + difficultyFileName + ".psc");
                }
            }
        }
    }
}
