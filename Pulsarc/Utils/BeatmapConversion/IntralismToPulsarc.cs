using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulsarc.Beatmaps;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace Pulsarc.Utils.BeatmapConversion
{
    class IntralismToPulsarc : BeatmapConverter
    {
        public Beatmap Convert(string folder_path)
        {
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
                            int time = (int) Math.Floor(evt.time * 1000);
                            result.arcs.Add(new Arc(time, arc));
                            Console.WriteLine(time + " : " + arc);
                        }
                    }
                }
            }

            return result;
        }
    }
}
