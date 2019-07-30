using System;
using System.Collections.Generic;
using Pulsarc.Beatmaps;
using System.IO;

namespace Pulsarc.Utils.BeatmapConversion
{
    class ManiaToPulsarc : BeatmapConverter
    {
        int msOffset = 0;

        public List<Beatmap> Convert(string folder_path)
        {
            List<Beatmap> results = new List<Beatmap>();

            if (Directory.Exists(folder_path))
            {
                foreach (string file in Directory.GetFiles(folder_path, "*.osu"))
                {
                    Beatmap result = new Beatmap();
                    ManiaBeatmap maniaBeatmap = new ManiaBeatmap(file);

                    result.FormatVersion = "1";
                    result.Mapper = maniaBeatmap.Creator;
                    result.Artist = maniaBeatmap.Artist;
                    result.Title = maniaBeatmap.Title;
                    result.Version = maniaBeatmap.Version;
                    result.Audio = maniaBeatmap.AudioFilename;

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
                        result.arcs.Add(new Arc(time, arc));
                    }
                    results.Add(result);
                }
            }

            return results;
        }

        public void Save(string folder_path)
        {
            foreach (Beatmap map in Convert(folder_path))
            {
                if (map.Audio != null)
                {
                    string audioPath = folder_path + "/" + map.Audio;
                    if (File.Exists(audioPath))
                    {
                        int id = 0;
                        string folderName = string.Join("_", (id + " - " + map.Artist + " - " + map.Title + " (" + map.Mapper + ")").Split(Path.GetInvalidFileNameChars()));
                        string dirName = "Songs/" + folderName;

                        if (!Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }

                        File.Copy(audioPath, dirName + "/" + map.Audio, true);
                        string difficultyFileName = string.Join("_", (map.Artist + " - " + map.Title + " [" + map.Version + "]" + " (" + map.Mapper + ")").Split(Path.GetInvalidFileNameChars()));
                        BeatmapHelper.Save(map, dirName + "/" + difficultyFileName + ".psc");
                    }
                }
            }
        }
    }
}
