using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Pulsarc.Beatmaps
{
    public class Beatmap
    {
        // Beatmap folder path
        public string path;

        // Beatmap filename
        public string fileName;

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

        // Background filename
        public string Background { get; set; }

        // General

        // How many keys this Beatmap uses.
        public int KeyCount { get; set; } = 4;

        // The calculated difficulty of this Beatmap.
        public double Difficulty { get; set; } = 0;


        // Events
        
        // All the events in this Beatmap (storyboard, zoom, etc.)
        public List<Event> events;

        // Gameplay

        public List<SpeedVariation> speedVariations;
        public List<TimingPoint> timingPoints;
        public List<Arc> arcs;

        // Performance
        public bool fullyLoaded = false;

        /// <summary>
        /// A collection of arcs, timing points, speed variations,
        /// and events that make up the gameplay of Pulsarc.
        /// </summary>
        public Beatmap()
        {
            arcs = new List<Arc>();
            timingPoints = new List<TimingPoint>();
            speedVariations = new List<SpeedVariation>();
            events = new List<Event>();
        }

        public string getHash()
        {
            // The hash is modified for any metadata or arc/sv change
            int a = 0;
            foreach (Arc arc in arcs)
            {
                a += arc.time + arc.type;
            }
            int t = 0;
            foreach (SpeedVariation sv in speedVariations)
            {
                t += sv.time + sv.time + (int) sv.type;
            }
            string uniqueMapDescriptor = Artist + Title + Mapper + Version + a + ',' + t;
            return BitConverter.ToString(((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(new UTF8Encoding().GetBytes(uniqueMapDescriptor)))
                   // without dashes
                   .Replace("-", string.Empty)
                   // make lowercase
                   .ToLower();
        }

        public List<ScoreData> getLocalScores()
        {
            return DataManager.scoreDB.getScores(getHash());
        }

        public override string ToString()
        {
            return Artist + " - " + Title + " [" + Version + "] (" + Mapper + ")";
        }
    }
}
