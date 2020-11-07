using System;
using System.Collections.Generic;
using Pulsarc.Beatmaps;
using System.IO;

namespace Pulsarc.Utils.BeatmapConversion
{
    class ManiaToPulsarc : IBeatmapConverter
    {
        // Estimated offset difference between osu!mania and Pulsarc
        private const int msOffset = 0;

        /// <summary>
        /// Convert an osu!mania beatmap to a Pulsarc beatmap
        /// </summary>
        /// <param name="folder_path">The path to the osu!mania map folder.</param>
        /// <returns>A list containing all the difficulties as seperate Beatmaps.</returns>
        public List<Beatmap> Convert(string folder_path)
        {
            List<Beatmap> results = new List<Beatmap>();

            // See if the provided folder exists
            if (Directory.Exists(folder_path))
            {
                // Look for .osu files, there should be one for each difficulty
                foreach (string file in Directory.GetFiles(folder_path, "*.osu"))
                {
                    Beatmap result = new Beatmap();
                    ManiaBeatmap maniaBeatmap = new ManiaBeatmap(file);

                    string mapOffset = Config.Get["Converting"]["MapOffset"];

                    // If the MapOffset value is empty, null, or not an int, set map offset to 0.
                    // Otherwise set it to the provided value.
                    result.MapOffset =
                        string.IsNullOrEmpty(mapOffset) || !int.TryParse(mapOffset, out int a)
                        ? "0" : mapOffset;

                    // Fill in metadata
                    result.FormatVersion = "1";
                    result.Mapper = maniaBeatmap.Creator;
                    result.Artist = maniaBeatmap.Artist;
                    result.Title = maniaBeatmap.Title;
                    result.Version = maniaBeatmap.Version;
                    result.Audio = maniaBeatmap.AudioFilename;
                    result.PreviewTime = maniaBeatmap.PreviewTime;

                    if (maniaBeatmap.Events.Count > 0)
                    {
                        // Remove the "0,0,"" and "",0,0" on the background line.
                        string backgroundName = maniaBeatmap.Events[0];
                        string[] charsToRemove = new string[] { ",", "\"", "0" };

                        foreach (string c in charsToRemove)
                        {
                            backgroundName = backgroundName.Replace(c, "");
                        }

                        result.Background = backgroundName;
                    }

                    // Look at each HitObject, and assign the appropriate bit to it.
                    foreach (string str in maniaBeatmap.HitObjects)
                    {
                        string[] parts = str.Split(',');
                        int arc = 0;

                        switch (parts[0])
                        {
                            case "64":
                                arc |= 1 << 2;
                                break;
                            case "192":
                                arc |= 1 << 3;
                                break;
                            case "320":
                                arc |= 1 << 1;
                                break;
                            case "448":
                                arc |= 1 << 0;
                                break;
                        }

                        int time = Int32.Parse(parts[2]) + msOffset;
                        result.Arcs.Add(new Arc(time, arc));
                    }
                    results.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Convert a folder of osu!mania beatmaps to Pulsarc-compatible beatmaps, and then save the converted Beatmaps to storage.
        /// </summary>
        /// <param name="folder_path">The path to the maps-to-be-converted folder</param>
        public void Save(string folder_path)
        {
            foreach (Beatmap map in Convert(folder_path))
            {
                if (map.Audio != null)
                {
                    string audioPath = $"{folder_path}/{map.Audio}";

                    if (File.Exists(audioPath))
                    {
                        int id = 0;
                        // The folder name will look like "0 - Artist - SongTitle - (Mapper)"
                        string folderName = string.Join("_", ($"{id} - {map.Artist} - {map.Title} ({map.Mapper})").Split(Path.GetInvalidFileNameChars()));
                        string dirName = $"Songs/{folderName}";

                        if (!Directory.Exists(dirName))
                            Directory.CreateDirectory(dirName);

                        // Copy Audio File
                        File.Copy(audioPath, $"{dirName}/{map.Audio}", true);

                        // Copy Background Image
                        string backgroundPath = $"{folder_path}/{map.Background}";
                        
                        if (File.Exists(backgroundPath))
                        {
                            File.Copy(backgroundPath, $"{dirName}/{map.Background}", true);
                        }
                        else
                        {
                            map.Background = "";
                        }

                        // The file name will look like "Artist - SongTitle [Converted] (Mapper).psc"
                        string difficultyFileName = string.Join("_", ($"{map.Artist} - {map.Title} [{map.Version}] ({map.Mapper})").Split(Path.GetInvalidFileNameChars()));

                        BeatmapHelper.Save(map, $"{dirName}/{difficultyFileName}.psc");
                    }
                }
            }
        }
    }
}
