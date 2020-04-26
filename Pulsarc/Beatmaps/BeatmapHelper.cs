using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Pulsarc.Beatmaps.Events;
using Pulsarc.Utils.SQLite;
using Wobble.Logging;
using Pulsarc.Utils;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace Pulsarc.Beatmaps
{
    static class BeatmapHelper
    {
        /// <summary>
        /// Load a single Beatmap.
        /// </summary>
        /// <param name="path">The path to the beatmap folder.</param>
        /// <param name="fileName">The fileName of the .psc file.</param>
        static public Beatmap Load(string path, string fileName)
        {
            Beatmap parsed = new Beatmap();
            parsed.Path = path;
            parsed.FileName = fileName;

            var state = "";

            var lines = File.ReadLines($"{path}/{fileName}");

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
                            case "MapOffset":
                                try
                                {
                                    parsed.GetType().GetProperty(type).SetValue(parsed, int.Parse(rightPart));
                                }
                                catch
                                {
                                    PulsarcLogger.Error($"Unknown beatmap field : {type}", LogType.Runtime);
                                }
                                break;
                            case "Audio":
                                try
                                {
                                    parsed.GetType().GetProperty(type).SetValue(parsed, rightPart);
                                }
                                catch
                                {
                                    PulsarcLogger.Error($"Unknown beatmap field : {type}", LogType.Runtime);
                                }
                                break;
                            case "Background":
                                {
                                    try
                                    {
                                        parsed.GetType().GetProperty(type).SetValue(parsed, rightPart);
                                    }
                                    catch
                                {
                                    PulsarcLogger.Error($"Unknown beatmap field : {type}", LogType.Runtime);
                                }
                                    break;
                                }
                            case "PreviewTime":
                            case "KeyCount":
                                try
                                {
                                    parsed.GetType().GetProperty(type).SetValue(parsed, int.Parse(rightPart));
                                }
                                catch
                                {
                                    PulsarcLogger.Error($"Unknown beatmap field : {type}", LogType.Runtime);
                                }
                                break;
                            case "Difficulty":
                                try
                                {
                                    parsed.GetType().GetProperty(type).SetValue(parsed, Double.Parse(rightPart, CultureInfo.InvariantCulture));
                                }
                                catch
                                {
                                    PulsarcLogger.Error($"Unknown beatmap field : {type}", LogType.Runtime);
                                }
                                break;
                            case "Events":
                            case "TimingPoints":
                            case "SpeedVariations":
                            case "Arcs":
                                state = type;
                                break;
                            default:
                                PulsarcLogger.Error($"Unknown beatmap field : {type}", LogType.Runtime);
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
                                    int type = int.Parse(eventParts[(int)EventIndex.Type]);

                                    Event evnt = MakeEvent(line, (EventType)type);
                                    if (evnt != null) parsed.Events.Add(evnt);
                                }
                                catch
                                {
                                    PulsarcLogger.Error($"Invalid Event : {line}", LogType.Runtime);
                                }
                                break;
                            case "TimingPoints":
                                try
                                {
                                    parsed.TimingPoints.Add(new TimingPoint(Convert.ToInt32(eventParts[0]), Convert.ToInt32(eventParts[1])));
                                }
                                catch
                                {
                                    PulsarcLogger.Error($"Invalid TimingPoint : {line}", LogType.Runtime);
                                }
                                break;
                            case "Arcs":
                                try
                                {
                                    parsed.Arcs.Add(new Arc(Convert.ToInt32(eventParts[0]), Convert.ToInt32(eventParts[1], 2)));
                                }
                                catch
                                {
                                    PulsarcLogger.Error($"Invalid Arc : {line}", LogType.Runtime);
                                }
                                break;
                            default:
                                PulsarcLogger.Error($"Invalid state : {line}", LogType.Runtime);
                                break;

                        }
                    }
                }
            }

            // If no difficulty has been provided in the game file, process it now.
            if(parsed.Difficulty == 0)
                parsed.Difficulty = DifficultyCalculation.GetDifficulty(parsed);

            parsed.FullyLoaded = true;

            return parsed;
        }

        /// <summary>
        /// Load a Beatmap from a BeatmapData that should have all the data needed
        /// to make up a Beatmap.
        /// </summary>
        /// <param name="data">The BeatmapData object to load from.</param>
        /// <returns>The beatmap created from the BeatmapData</returns>
        public static Beatmap LoadLight(BeatmapData data)
        {
            Beatmap parsed = new Beatmap();
            parsed.FullyLoaded = false;

            // Path names
            parsed.Path = data.path;
            parsed.FileName = data.fileName;

            // Background
            parsed.Background = data.backgroundPath;

            // Audio
            parsed.Audio = data.audioPath;
            parsed.PreviewTime = data.audioPreviewTime;

            // Metadata
            parsed.Artist = data.artist;
            parsed.Title = data.title;
            parsed.Version = data.version;
            parsed.Mapper = data.mapper;

            // Game Data
            parsed.KeyCount = data.keyCount;
            parsed.Difficulty = data.difficulty;

            return parsed;
        }

        /// <summary>
        /// Save a single Beatmap as a .psc.
        /// </summary>
        /// <param name="beatmap">The Beatmap to be saved.</param>
        /// <param name="file_path">The filepath this Beatmap Should be saved to.</param>
        static public void Save(Beatmap beatmap, string file_path)
        {
            using (StreamWriter file = new StreamWriter(file_path))
            {
                // Write Game data/Metadata
                WriteProperty(file, beatmap, "FormatVersion");
                WriteProperty(file, beatmap, "Title");
                WriteProperty(file, beatmap, "Artist");
                WriteProperty(file, beatmap, "Mapper");
                WriteProperty(file, beatmap, "Version");
                WriteProperty(file, beatmap, "Audio");
                WriteProperty(file, beatmap, "PreviewTime");
                WriteProperty(file, beatmap, "Background");

                file.WriteLine("");
                WriteProperty(file, beatmap, "KeyCount");
                WriteProperty(file, beatmap, "Difficulty");

                // Write Events
                file.WriteLine("");
                file.WriteLine("Events:");

                foreach (Event evt in beatmap.Events)
                {
                    file.WriteLine(evt.ToString());
                }

                // Write Timing Points
                file.WriteLine("");
                file.WriteLine("TimingPoints:");

                foreach (TimingPoint timingPoint in beatmap.TimingPoints)
                {
                    file.WriteLine(timingPoint.ToString());
                }

                // Write Speed Variations
                file.WriteLine("");
                file.WriteLine("SpeedVariations:");

                // Write Arcs
                file.WriteLine("");
                file.WriteLine("Arcs:");

                foreach (Arc arc in beatmap.Arcs)
                {
                    file.WriteLine(arc.ToString());
                }
            }
        }

        public static void SaveAsZip(Beatmap beatmap)
        {
            using (FileStream output = File.Create($"Songs/{beatmap}.psm"))
            using (ZipOutputStream zipStream = new ZipOutputStream(output))
            {
                zipStream.SetLevel(3);

                int folderOffset = beatmap.Path.Length
                    + ((beatmap.Path.EndsWith("/") || beatmap.Path.EndsWith("\\")) ? 0 : 1);

                // Compress the folder
                CompressFolder(beatmap.Path, zipStream, folderOffset);
            }

            void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
            {
                string[] fileNames = Directory.GetFiles(beatmap.Path);

                foreach (string fileName in fileNames)
                {
                    FileInfo info = new FileInfo(fileName);

                    string entryName = fileName.Substring(folderOffset);
                    entryName = ZipEntry.CleanName(entryName);

                    ZipEntry entry = new ZipEntry(entryName)
                    {
                        DateTime = info.LastWriteTime,
                        Size = info.Length,
                    };

                    zipStream.PutNextEntry(entry);

                    byte[] buffer = new byte[4096];
                    using (FileStream input = File.OpenRead(fileName))
                    {
                        StreamUtils.Copy(input, zipStream, buffer);
                    }

                    zipStream.CloseEntry();
                }

                // Compress subfolders
                string[] folders = Directory.GetDirectories(beatmap.Path);
                foreach (string folder in folders)
                {
                    CompressFolder(folder, zipStream, folderOffset);
                }
            }
        }

        /// <summary>
        /// Write a property to file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="beatmap"></param>
        /// <param name="property"></param>
        static private void WriteProperty(StreamWriter file, Beatmap beatmap, string property)
        {
            try
            {
                // Use reflection to write any property. The property needs to implement ToString()
                file.WriteLine(property + ": " + beatmap.GetType().GetProperty(property).GetValue(beatmap, null));
            }
            catch
            {
                PulsarcLogger.Error($"Trying to write invalid property {property}", LogType.Runtime);
            }
        }

        /// <summary>
        /// Use bitwise enumeration to determine if an arc is on the specified column
        /// </summary>
        /// <param name="arc">The arc to check</param>
        /// <param name="column">The column to check in (in binary format)</param>
        /// <returns></returns>
        static public bool IsColumn(Arc arc, int column) => ((arc.Type >> column) & 1) != 0;

        /// <summary>
        /// Makes a new Event based on the data found in line
        /// </summary>
        /// <param name="line">A line from the .psc to read from.</param>
        /// <returns>A new event corresponding to the EventType index of line, using the
        /// data in "line" to make the event. Or null if none was found.</returns>
        static private Event MakeEvent(string line, EventType type)
        {
            switch (type)
            {
                case EventType.Zoom:
                    return new ZoomEvent(line);
                default:
                    return null;
            }
        }
    }
}
