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
        int msOffset = -80;

        public List<Beatmap> Convert(string folder_path)
        {
            List<Beatmap> results = new List<Beatmap>();
            Beatmap result = new Beatmap();

            if(Directory.Exists(folder_path))
            {
                string configPath = folder_path + "/config.txt";
                if (File.Exists(configPath))
                {
                    IntralismBeatmap beatmap = JsonConvert.DeserializeObject<IntralismBeatmap>(File.ReadAllText(configPath, Encoding.UTF8));

                    result.FormatVersion = "1";
                    result.Mapper = "Intralism";
                    result.Artist = "Unknown";
                    result.Title = beatmap.name;
                    result.Version = "Converted";
                    result.Audio = beatmap.musicFile;
                    result.timingPoints.Add(new TimingPoint(0, 120));

                    foreach(Event evt in beatmap.events)
                    {
                        if(evt.data[0] == "SpawnObj")
                        {
                            int arc = 0;
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
                            int time = (int) Math.Floor(evt.time * 1000) + msOffset;
                            result.arcs.Add(new Arc(time, arc));
                        }
                    }
                }
            }

            results.Add(result);
            return results;
        }

        public void Save(string folder_path)
        {
            Beatmap map = Convert(folder_path).First();

            if(map.Audio != null)
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
