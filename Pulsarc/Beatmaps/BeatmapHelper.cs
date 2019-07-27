using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace Pulsarc.Beatmaps
{
    static class BeatmapHelper
    {
        static public Beatmap Load(string file_path)
        {

            Beatmap parsed = new Beatmap();
            parsed.path = file_path;

            var state = "";

            var lines = File.ReadLines(file_path);
            foreach (var line in lines)
            {
                if (line.Length > 0)
                {
                    Queue<string> lineParts = new Queue<string>(line.Split(':'));

                    if (lineParts.Count > 1)
                    {
                        var type = lineParts.Dequeue();
                        var rightPart = String.Concat(lineParts.ToArray()).Trim();

                        switch (type)
                        {
                            case "FormatVersion":
                            case "Title":
                            case "Artist":
                            case "Mapper":
                            case "Version":
                            case "Audio":
                                try
                                {
                                    parsed.GetType().GetProperty(type).SetValue(parsed, rightPart);
                                }
                                catch
                                {
                                    Console.WriteLine("Unknown beatmap field : " + type);
                                }
                                break;
                            case "KeyCount":
                                try
                                {
                                    parsed.GetType().GetProperty(type).SetValue(parsed, Int32.Parse(rightPart));
                                }
                                catch
                                {
                                    Console.WriteLine("Unknown beatmap field : " + type);
                                }
                                break;
                            case "Difficulty":
                                try
                                {
                                    parsed.GetType().GetProperty(type).SetValue(parsed, Double.Parse(rightPart, CultureInfo.InvariantCulture));
                                }
                                catch
                                {
                                    Console.WriteLine("Unknown beatmap field : " + type);
                                }
                                break;
                            case "TimingPoints":
                            case "SpeedVariations":
                            case "Arcs":
                                state = type;
                                break;
                            default:
                                Console.WriteLine("Unknown beatmap field : " + type);
                                break;
                        }
                    }
                    else
                    {
                        var eventParts = line.Split(',');

                        switch (state)
                        {
                            case "TimingPoints":
                                try
                                {
                                    parsed.timingPoints.Add(new TimingPoint(Convert.ToInt32(eventParts[0]), Convert.ToInt32(eventParts[1])));
                                }
                                catch
                                {
                                    Console.WriteLine("Invalid TimingPoint : " + line);
                                }
                                break;
                            case "SpeedVariations":
                                try
                                {
                                    parsed.speedVariations.Add(new SpeedVariation(Convert.ToInt32(eventParts[0]), Convert.ToInt32(eventParts[1]), Double.Parse(eventParts[2], CultureInfo.InvariantCulture)));
                                }
                                catch
                                {
                                    Console.WriteLine("Invalid SpeedVariation : " + line);
                                }
                                break;
                            case "Arcs":
                                try
                                {
                                    parsed.arcs.Add(new Arc(Convert.ToInt32(eventParts[0]), Convert.ToInt32(eventParts[1], 2)));
                                }
                                catch
                                {
                                    Console.WriteLine("Invalid Arc : " + line);
                                }
                                break;
                            default:
                                Console.WriteLine("Invalid state : " + state);
                                break;

                        }
                    }
                }
            }

            if(parsed.Difficulty == 0)
            {
                parsed.Difficulty = DifficultyCalculation.GetDifficulty(parsed);
            }

            return parsed;
        }

        static public void Save(Beatmap beatmap, string file_path)
        {
            using (StreamWriter file =
                new StreamWriter(file_path))
            {
                writeProperty(file, beatmap, "FormatVersion");
                writeProperty(file, beatmap, "Title");
                writeProperty(file, beatmap, "Artist");
                writeProperty(file, beatmap, "Mapper");
                writeProperty(file, beatmap, "Version");
                writeProperty(file, beatmap, "Audio");

                file.WriteLine("");
                writeProperty(file, beatmap, "KeyCount");
                writeProperty(file, beatmap, "Difficulty");

                file.WriteLine("");
                file.WriteLine("TimingPoints:");

                foreach (TimingPoint timingPoint in beatmap.timingPoints)
                {
                    file.WriteLine(timingPoint.toString());
                }

                file.WriteLine("");
                file.WriteLine("SpeedVariations:");

                foreach (SpeedVariation speedVariation in beatmap.speedVariations)
                {
                    file.WriteLine(speedVariation.toString());
                }

                file.WriteLine("");
                file.WriteLine("Arcs:");

                foreach (Arc arc in beatmap.arcs)
                {
                    file.WriteLine(arc.toString());
                }
            }
        }

        static private void writeProperty(StreamWriter file, Beatmap beatmap, string property)
        {
            try
            {
                file.WriteLine(property + ": " + beatmap.GetType().GetProperty(property).GetValue(beatmap, null));
            }
            catch
            {
                Console.WriteLine("Trying to write invalid property " + property);
            }
        }

        static public bool isColumn(Arc arc, int k)
        {
            return ((arc.type >> k) & 1) != 0;
        }
    }
}
