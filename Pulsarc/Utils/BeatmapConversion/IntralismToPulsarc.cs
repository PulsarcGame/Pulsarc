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
    class IntralismToPulsarc : IBeatmapConverter
    {
        // A value needed to properly convert Intralism's PlayerDistance to Pulsarc's ZLocation
        // It is the "x" in the equation: "IntralismPlayerDistance * x = PulsarcZLocation"
        // NOTE: Doesn't work well with negative IntralismPlayerDistance values
        private const float playerDistanceToZLocationFactor = 211.862069f;

        // Estimated offset difference between Intralism and Pulsarc
        private const int msOffset = -80;

        /// <summary>
        /// Convert an Intralism beatmap to a Pulsarc beatmap
        /// </summary>
        /// <param name="folder_path">The path to the Intralism map folder.</param>
        /// <returns>A list containing a single converted Beatmap.</returns>
        public List<Beatmap> Convert(string folder_path)
        {
            List<Beatmap> results = new List<Beatmap>();
            Beatmap result = new Beatmap();

            string backgroundImage = Config.Get["Converting"]["BGImage"];

            // See if the provided path exists
            if (Directory.Exists(folder_path))
            {
                string configPath = $"{folder_path}/config.txt";

                // See if the a "config.txt" file exists
                if (File.Exists(configPath))
                {
                    // Convert the config file to an IntrlaismBeatmap
                    IntralismBeatmap beatmap = JsonConvert.DeserializeObject<IntralismBeatmap>(File.ReadAllText(configPath, Encoding.UTF8));

                    string name = "";

                    // If the user specified an image path to use, use that path.
                    if (backgroundImage != null && !backgroundImage.Equals(""))
                        name = backgroundImage;

                    // If there's an average of 1 image per 10 seconds of map time or less
                    // and the user-defined path doesn't exist, grab the first image path in 
                    // the beatmap.
                    if (!File.Exists($"{folder_path}/{name}") && beatmap.LevelResources.Count > 0 && beatmap.LevelResources.Count < Math.Ceiling(beatmap.MusicTime / 10))
                        beatmap.LevelResources[0].TryGetValue("path", out name);

                    result.Background = name;

                    // Fill in the missing metadata
                    result.FormatVersion = "1";
                    result.Mapper = "Intralism";
                    result.Artist = "Unknown";
                    result.Title = beatmap.Name;
                    result.Version = "Converted";
                    result.Audio = beatmap.MusicFile;
                    result.TimingPoints.Add(new TimingPoint(0, 120));
                    result.PreviewTime = 0;

                    // Go through each Intralism Event
                    foreach (Event evt in beatmap.Events)
                        // If the current event is an Arc, convert it to a Pulsarc Arc.
                        switch (evt.Data[0])
                        {
                            case "SpawnObj":
                                // Add the converted arc to the Beatmap
                                result.Arcs.Add(HandleSpawnObj(evt));
                                break;
                            case "SetPlayerDistance":
                                // Add the converted zoom to the Beatmap
                                result.Events.Add(HandleSetPlayerDistance(evt));
                                break;
                            default:
                                break;
                        }
                }
            }

            results.Add(result);
            return results;
        }

        /// <summary>
        /// Handles the conversion of an Intralism SpawnObj Event into an arc.
        /// </summary>
        /// <param name="evt">The event to be converted</param>
        /// <returns>The Event converted to an arc</returns>
        private Arc HandleSpawnObj(Event evt)
        {
            int arc = 0;

            // Find each direction listed in the current event, and assign the appropriate bit to it.
            foreach (string direction in evt.Data[1].Split(',')[0].Split('[')[1].Split(']')[0].Split('-'))
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
            int time = (int)Math.Floor(evt.Time * 1000) + msOffset;

            return new Arc(time, arc);
        }

        /// <summary>
        /// Handles the conversion of an Intralism SetPlayerDistance event into a ZoomEvent.
        /// </summary>
        /// <param name="evt">The event to be converted</param>
        /// <returns>The Event converted to a ZoomEvent</returns>
        private ZoomEvent HandleSetPlayerDistance(Event evt)
        {
            // Convert the Intralism Event time to the format Pulsarc understands
            int time = (int)Math.Floor(evt.Time * 1000) + (msOffset * 2);

            float convertedZLocation = float.Parse(evt.Data[1]) * playerDistanceToZLocationFactor;
            
            double convertedZoomLevel = Math.Round((Pulsarc.BASE_WIDTH / 2) * (Skin.Assets["crosshair"].Width / 2) / convertedZLocation,2);

            return new ZoomEvent($"{time},1,-1,{convertedZoomLevel},0");
        }

        /// <summary>
        /// Converts an Intralism beatmap folder to a Pulsarc-compatible beatmap, and then saves the converted Beatmap to storage.
        /// </summary>
        /// <param name="folder_path">The path to the map-to-be-converted folder</param>
        public void Save(string folder_path)
        {
            Beatmap map = Convert(folder_path).First();

            if (map.Audio != null)
            {
                string audioPath = $"{folder_path}/{map.Audio}";

                if (File.Exists(audioPath))
                {
                    int id = 0;
                    // The folder name will look like "0 - Unknown - MapTitle - (Mapper)"
                    string folderName = string.Join("_", ($"{id} - {map.Artist} - {map.Title} ({map.Mapper})").Split(Path.GetInvalidFileNameChars()));
                    string dirName = $"Songs/{folderName}";

                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);

                    // Copy Audio File
                    File.Copy(audioPath, $"{dirName}/{map.Audio}", true);

                    // Copy Background Image
                    string backgroundPath = $"{folder_path}/{map.Background}";

                    if (File.Exists(backgroundPath))
                        File.Copy(backgroundPath, $"{dirName}/{map.Background}", true);
                    else
                        map.Background = "";


                    // The file name will look like "Unknown - MapTitle [Converted] (Mapper).psc"
                    string difficultyFileName = string.Join("_", ($"{map.Artist} - {map.Title} [{map.Version}] ({map.Mapper})").Split(Path.GetInvalidFileNameChars()));

                    BeatmapHelper.Save(map, $"{dirName}/{difficultyFileName}.psc");
                }
            }
        }
    }
}
