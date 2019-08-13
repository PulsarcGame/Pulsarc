using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Beatmaps
{
    public class Beatmap
    {
        // The path to the .psc file this Beatmap parses from.
        public string path;

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
    }
}
