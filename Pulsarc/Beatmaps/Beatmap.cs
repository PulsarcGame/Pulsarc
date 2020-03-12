using Pulsarc.Beatmaps.Events;
using Pulsarc.Utils;
using Pulsarc.Utils.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pulsarc.Beatmaps
{
    public class Beatmap
    {
        // Beatmap folder path and file
        public string Path { get; set; }
        public string FileName { get; set; }

        // Metadata
        // The format version of this Beatmap.
        public string FormatVersion { get; set; } = "1";

        // The title of the song used for this Beatmap.
        public string Title { get; set; }

        // The artist of the song used for this Beatmap.
        public string Artist { get; set; }

        // The mapper who created this beatmap.
        public string Mapper { get; set; }

        // The version of this Beatmap (Convert, Marathon, etc.)
        public string Version { get; set; }

        // The audio for this Beatmap.
        public string Audio { get; set; }

        // The time (in ms) to start previewing the song at in song selection.
        public int PreviewTime { get; set; }

        // Background filename
        public string Background { get; set; }

        // General
        // How many keys this Beatmap uses.
        public int KeyCount { get; set; } = 4;

        // The calculated difficulty of this Beatmap.
        public double Difficulty { get; set; } = 0;

        // Events
        // All the events in this Beatmap (storyboard, zoom, etc.)
        public List<Event> Events { get; set; }

        // Gameplay
        public List<TimingPoint> TimingPoints { get; set; }
        public List<Arc> Arcs { get; set; }

        // Performance
        public bool FullyLoaded { get; set; } = false;

        /// <summary>
        /// A collection of arcs, timing points, speed variations,
        /// and events that make up the gameplay of Pulsarc.
        /// </summary>
        public Beatmap()
        {
            Arcs = new List<Arc>();
            TimingPoints = new List<TimingPoint>();
            Events = new List<Event>();
        }
        
        /// <summary>
        /// Get the hash value for this beatmap, hash generated from data in the beatmap.
        /// </summary>
        /// <returns>The Hash value for this beatmap.</returns>
        public string GetHash()
        {
            // The hash is modified for any metadata or arc/sv change
            int arcCount = 0;
            foreach (Arc arc in Arcs)
                arcCount += arc.Time + arc.Type;

            int eventCount = 0;
            foreach (Event evnt in Events)
                eventCount += evnt.Time + evnt.Time + (int)evnt.Type;

            string uniqueMapDescriptor = Artist + Title + Mapper + Version + arcCount + ',' + eventCount;
            return BitConverter.ToString(
                ((HashAlgorithm)CryptoConfig.CreateFromName("MD5"))
                    .ComputeHash(new UTF8Encoding()
                    .GetBytes(uniqueMapDescriptor)))
                    // without dashes
                    .Replace("-", string.Empty)
                    // make lowercase
                    .ToLower();
        }

        /// <summary>
        /// Get the locally saved scores for this beatmap.
        /// </summary>
        /// <returns></returns>
        public List<ScoreData> GetLocalScores()
        {
            return DataManager.ScoreDB.GetScores(GetHash());
        }

        /// <summary>
        /// Get the audio path to this beatmap's audio file.
        /// </summary>
        /// <returns></returns>
        public string GetFullAudioPath()
            =>  // Get the full path to "/Songs"
                (Directory.GetParent(Path)
                // Get rid of "\Songs", Path has an extra "/Songs" in it.
                .FullName.Replace("\\Songs", "") +
                // Add the beatmap path and audio filename.
                $"/{Path}/{Audio}")
                // Replace "\" with "/" for cross platform support
                .Replace("\\", "/");

        /// <summary>
        /// This Beatmap as a string.
        /// Format is "Artist - Title [Version] (Mapper)"
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Artist} - {Title} [{Version}] ({Mapper})";
    }
}
