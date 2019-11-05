using System;
using System.Collections.Generic;
using System.IO;

namespace Pulsarc.Utils.BeatmapConversion
{
    /// <summary>
    /// A represenation of a osu!mania Beatmap.
    /// </summary>
    public class ManiaBeatmap
    {
        // The audio filename of the map
        public string AudioFilename { get; set; }

        // Audio Preview Time
        public int PreviewTime { get; set; }

        // The gamemode type
        public int Mode { get; set; }

        // Metadata
        // The song title
        public string Title { get; set; }
        // The artist of the song
        public string Artist { get; set; }
        // The creator of the map
        public string Creator { get; set; }
        // The version of the map
        public string Version { get; set; }

        // Events
        // All non-gameplay events
        public List<string> Events { get; set; }
        // A list of each TimingPoint (an event which changes BPM/SV/KiaiTime)
        public List<string> TimingPoints { get; set; }
        // A list of each osu HitObject
        public List<string> HitObjects { get; set; }

        /// <summary>
        /// Creates a new ManiaBeatmap using the filename provided.
        /// </summary>
        /// <param name="file">The filename of the mania beatmap</param>
        public ManiaBeatmap(string file)
        {
            Events = new List<string>();
            TimingPoints = new List<string>();
            HitObjects = new List<string>();

            string state = "";

            // Go through each line in the file
            foreach(string line in File.ReadAllLines(file))
            {
                if (line.Length > 0)
                {
                    Queue<string> parts = new Queue<string>(line.Split(':'));
                    
                    // If there's no colons, it's probably seperated by commas instead
                    if (parts.Count <= 1)
                        parts = new Queue<string>(line.Split(","));

                    if (parts.Count > 1)
                    {
                        string key = parts.Dequeue();
                        string value = String.Join(":", parts.ToArray()).Trim();

                        // Add Metadta
                        if (state == "")
                        {
                            switch (key)
                            {
                                case "AudioFilename":
                                case "Title":
                                case "Artist":
                                case "Creator":
                                case "Version":
                                    try
                                    {
                                        GetType().GetProperty(key).SetValue(this, value);
                                    }
                                    catch
                                    {
                                        Console.WriteLine($"Incorrect value in field : {key}");
                                    }
                                    break;
                                case "Mode":
                                case "PreviewTime":
                                    try
                                    {
                                        GetType().GetProperty(key).SetValue(this, int.Parse(value));
                                    }
                                    catch
                                    {
                                        Console.WriteLine($"Incorrect value in field : {key}");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            // If flagged in one of these event categories, add the line to the corresponding category.
                            switch(state)
                            {
                                case "[Events]":
                                    state = AddEvent(line, state);
                                    break;
                                case "[TimingPoints]":
                                    TimingPoints.Add(line);
                                    break;
                                case "[HitObjects]":
                                    HitObjects.Add(line);
                                    break;
                                default:
                                    state = "";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // If we enter one of these categories, change "state" to flag to the
                        // switch-case code above to add those events to the beatmap.
                        switch(line)
                        {
                            case "[Events]":
                                state = line;
                                break;
                            case "[TimingPoints]":
                                break;
                            case "[HitObjects]":
                                state = line;
                                break;
                            default:
                                if (!line.Contains("//")) //Ignore comments
                                    state = "";
                                break;
                        }
                    }
                }
            }
        }

        private string AddEvent(string line, string state)
        {
            Events.Add(line);
            
            // Temp: this is to change the map background, which is the first line
            return "";
        }
    }
}
