using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace Pulsarc.Beatmaps
{
    static class BeatmapHelper
    {
        /// <summary>
        /// Load a single Beatmap.
        /// </summary>
        /// <param name="file_path">The path to a .psc beatmap file.</param>
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
                    // Information like Metadata are in the form 'Type: Data'
                    Queue<string> lineParts = new Queue<string>(line.Split(':'));

                    // If we recognize this form, it's an attribute.
                    // Otherwise, it is a more complex data form, like an event
                    if (lineParts.Count > 1)
                    {
                        var type = lineParts.Dequeue();
                        var rightPart = string.Concat(lineParts.ToArray()).Trim();

                        // Use reflection to set the Beatmap's attributes
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
                            case "Events":
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
                        // Each event is comma separated and can have any amount of values.
                        var eventParts = line.Split(',');

                        // Handling depends on the data type (or the current reading state)
                        switch (state)
                        {
                            case "Events":
                                try
                                {
                                    string[] parameters = new string[] { line };
                                    parsed.events.Add((Event) Activator.CreateInstance(Type.GetType(eventParts[1]), parameters));
                                }
                                catch
                                {
                                    Console.WriteLine("Invalid Event : " + line);
                                }
                                break;
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

            // If no difficulty has been provided in the game file, process it.
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
                file.WriteLine("Events:");

                foreach (Event evt in beatmap.events)
                {
                    file.WriteLine(evt.ToString());
                }

                file.WriteLine("");
                file.WriteLine("TimingPoints:");

                foreach (TimingPoint timingPoint in beatmap.timingPoints)
                {
                    file.WriteLine(timingPoint.ToString());
                }

                file.WriteLine("");
                file.WriteLine("SpeedVariations:");

                foreach (SpeedVariation speedVariation in beatmap.speedVariations)
                {
                    file.WriteLine(speedVariation.ToString());
                }

                file.WriteLine("");
                file.WriteLine("Arcs:");

                foreach (Arc arc in beatmap.arcs)
                {
                    file.WriteLine(arc.ToString());
                }
            }
        }

        static private void writeProperty(StreamWriter file, Beatmap beatmap, string property)
        {
            try
            {
                // Use reflection to write any property. The property need to implement ToString()
                file.WriteLine(property + ": " + beatmap.GetType().GetProperty(property).GetValue(beatmap, null));
            }
            catch
            {
                Console.WriteLine("Trying to write invalid property " + property);
            }
        }

        static public bool isColumn(Arc arc, int k)
        {
            // Use bitwise enumeration to determine if an arc is on the specified column
            return ((arc.type >> k) & 1) != 0;
        }
    }
}
