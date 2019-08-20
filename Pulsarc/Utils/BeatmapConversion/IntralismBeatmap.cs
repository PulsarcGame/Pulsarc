using System.Collections.Generic;

namespace Pulsarc.Utils.BeatmapConversion
{
    /// <summary>
    /// A represenation of an Intralism Beatmap.
    /// </summary>
    class IntralismBeatmap
    {
        // "Metadata"
        public long configVersion; // Version 1 has less storyboard events, and may have different timing to config 2 maps.
        public string name; // Map Title
        public string info; // Map/Steam Workshop Description

        // Intralism map ID and the filename of the image corresponding to that ID.
        public List<Dictionary<string, string>> levelResources;

        // Map tags, used for Steam Workshop.
        public List<string> tags;

        // 1 or 2 crosshairs on screen, outdated feature
        public long handCount;

        // Used for Steam Workshop
        public string moreInfoURL;

        // Beatmap data
        public long speed; // Base arc speed
        public long lives; // Base life amount
        public long maxLives; // Maximum life possible
        public string musicFile; // The audio filename for the map
        public double musicTime; // How long the song is

        // The thumbnail for the map
        public string iconFile;

        // The base "Environmental Type", changes some base Storyboard events.
        public long environmentType;

        // ¯\_(ツ)_/¯
        public List<Dictionary<string, string>> unlockConditions; // Seemed to be used to "unlock" maps in an earlier version of Intralism
        public bool hidden; // Probably related to above

        // The timing of checkpoints in the map
        public List<double> checkpoints;

        // A list of all the Events in the map
        public List<Event> events;
    }

    /// <summary>
    /// Intralism event, not to be confused with Pulsarc.Beatmaps.Event
    /// </summary>
    class Event
    {
        // The time of the event
        public double time;

        // A list of each part of the event data
        public List<string> data;
    }
}
