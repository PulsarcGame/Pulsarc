using System.Collections.Generic;

namespace Pulsarc.Utils.BeatmapConversion
{
    /// <summary>
    /// A represenation of an Intralism Beatmap.
    /// </summary>
    class IntralismBeatmap
    {
        // "Metadata"
        // Version 1 has less storyboard events, and may have different timing to config 2 maps.
        public long ConfigVersion { get; set; }
        // Map Title
        public string Name { get; set; }
        // Map Description
        public string Info { get; set; }

        // Intralism map ID and the filename of the image corresponding to that ID.
        public List<Dictionary<string, string>> LevelResources { get; set; }

        // Map tags, used for Steam Workshop.
        public List<string> Tags { get; set; }

        // 1 or 2 crosshairs on screen, outdated feature
        public long HandCount { get; set; }

        // Used by the Steam Workshop
        public string MoreInfoUrl { get; set; }

        // Beatmap data
        // Base arc speed
        public long Speed { get; set; }
        // Base life amount
        public long Lives { get; set; }
        // Maximum life possible
        public long MaxLives { get; set; }
        // The audio filename for the map
        public string MusicFile { get; set; }
        // How long the song is
        public double MusicTime { get; set; }

        // The thumbnail for the map
        public string IconFile { get; set; }

        // The base "Environmental Type", changes some base Storyboard events.
        public long EnvironmentType { get; set; }

        // ¯\_(ツ)_/¯
        // Seemed to be used to "unlock" maps in an earlier version of Intralism
        public List<Dictionary<string, string>> UnlockConditions { get; set; }
        // Probably related to above
        public bool Hidden { get; set; }

        // The timing of checkpoints in the map
        public List<double> Checkpoints { get; set; }

        // A list of all the Events in the map
        public List<Event> Events { get; set; }
    }

    /// <summary>
    /// Intralism event, not to be confused with Pulsarc.Beatmaps.Event
    /// </summary>
    class Event
    {
        // The time of the event
        public double Time { get; set; }

        // A list of each part of the event data
        public List<string> Data { get; set; }
    }
}
