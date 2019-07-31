using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Beatmaps
{
    public class Beatmap
    {
        public string path;

        // Metadata
        public string FormatVersion { get; set; } = "1";
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Mapper { get; set; }
        public string Version { get; set; }
        public string Audio { get; set; }

        // General
        public int KeyCount { get; set; } = 4;
        public double Difficulty { get; set; } = 0;

        // Events
        public List<Event> events;

        // Gameplay
        public List<SpeedVariation> speedVariations;
        public List<TimingPoint> timingPoints;
        public List<Arc> arcs;

        public Beatmap()
        {
            arcs = new List<Arc>();
            timingPoints = new List<TimingPoint>();
            speedVariations = new List<SpeedVariation>();
            events = new List<Event>();
        }
    }
}
