using System;
using System.Collections.Generic;
using System.IO;

namespace Pulsarc.Utils.BeatmapConversion
{
    public class ManiaBeatmap
    {
        public string AudioFilename { get; set; }
        public int Mode { get; set; }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Creator { get; set; }
        public string Version { get; set; }

        public List<string> Events { get; set; }
        public List<string> TimingPoints { get; set; }
        public List<string> HitObjects { get; set; }

        public ManiaBeatmap(string file)
        {
            Events = new List<string>();
            TimingPoints = new List<string>();
            HitObjects = new List<string>();

            string state = "";
            foreach(string line in File.ReadAllLines(file))
            {
                if (line.Length > 0)
                {
                    Queue<string> parts = new Queue<string>(line.Split(':'));
                    if (parts.Count > 1)
                    {
                        string key = parts.Dequeue();
                        string value = String.Join(":", parts.ToArray()).Trim();

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
                                        Console.WriteLine("Incorrect value in field : " + key);
                                    }
                                    break;
                                case "Mode":
                                    try
                                    {
                                        Mode = Int16.Parse(value);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Incorrect value in field : " + key);
                                    }
                                    break;
                            }
                        } else
                        {
                            switch(state)
                            {
                                case "[Events]":
                                    Events.Add(line);
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
                    } else
                    {
                        switch(line)
                        {
                            case "[Events]":
                            case "[TimingPoints]":
                            case "[HitObjects]":
                                state = line;
                                break;
                            default:
                                state = "";
                                break;
                        }
                    }
                }
            }
        }
    }
}
