using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pulsarc.Beatmaps;
using Newtonsoft.Json;
using System.IO;
using Pulsarc.Beatmaps.Events;
using Pulsarc.Skinning;

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
                        switch (evt.data[0])
                        {
                            case "SpawnObj":
                                // Add the converted arc to the Beatmap
                                result.arcs.Add(handleSpawnObj(evt));
                                break;
                            case "SetPlayerDistance":
                                // Add the converted zoom to the Beatmap
                                result.events.Add(handleSetPlayerDistance(evt));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            results.Add(result);
            return results;
        }

        /// <summary>
        /// Handles the conversion of an Intralism event into an arc.
        /// </summary>
        /// <param name="evt">The event to be converted</param>
        /// <returns></returns>
        private Arc handleSpawnObj(Event evt)
        {
            int arc = 0;

            // Find each direction listed in the current event, and assign the appropriate bit to it.
            foreach (string direction in evt.data[1].Split(',')[0].Split('[')[1].Split(']')[0].Split('-'))
            {
                switch (direction)
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
            int time = (int)Math.Floor(evt.time * 1000) + msOffset;

            return new Arc(time, arc);
        }

        private ZoomEvent handleSetPlayerDistance(Event evt)
        {
            // Convert the Intralism Event time to the format Pulsarc understands
            int time = (int)Math.Floor(evt.time * 1000) + (msOffset * 2);

            float convertedZLocation = float.Parse(evt.data[1]) * 221f; // 221 is the estimated x in the equation "IntralismPlayerDistance * x = PulsarcZoomLevel" Likely to be somewhere between 220 and 222.5
            
            float convertedZoomLevel = (Pulsarc.xBaseRes / 2) * (Skin.assets["crosshair"].Width / 2) / convertedZLocation;

            return new ZoomEvent(time + ",1,-1," + convertedZoomLevel + "," + 0);
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
