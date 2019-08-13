using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Beatmaps
{
    public class Beatmap
    {
        /// <summary>The path to the .psc file this Beatmap parses from.</summary>
        public string path;

        // Metadata
        /// <summary>The format version of this Beatmap.</summary>
        public string FormatVersion { get; set; } = "1";
        /// <summary>The title of the song used for this Beatmap.</summary>
        public string Title { get; set; }
        /// <summary>The artist of the song used for this Beatmap.</summary>
        public string Artist { get; set; }
        /// <summary>The mapper who created this beatmap.</summary>
        public string Mapper { get; set; }
        /// <summary>The version of this Beatmap (Convert, Marathon, etc.)</summary>
        public string Version { get; set; }
        /// <summary>The audio for this Beatmap.</summary>
        public string Audio { get; set; }

        // General
        /// <summary>How many keys this Beatmap uses.</summary>
        public int KeyCount { get; set; } = 4;
        /// <summary>The calculated difficulty of this Beatmap.</summary>
        public double Difficulty { get; set; } = 0;

        // Events
        /// <summary>All the events in this Beatmap (storyboard, zoom, etc.)</summary>
        public List<Event> events;

        // Gameplay
        public List<SpeedVariation> speedVariations;
        public List<TimingPoint> timingPoints;
        public List<Arc> arcs;

        /// <summary>
        /// A collection of arcs, timing points, speed variations, and events that make up the gameplay of Pulsarc.
        /// </summary>
        public Beatmap()
        {
            arcs = new List<Arc>();
            timingPoints = new List<TimingPoint>();
            speedVariations = new List<SpeedVariation>();
            events = new List<Event>();
        }
    }
}
