using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Pulsarc.Utils.SQLite;
using System.Data.SQLite;

namespace Pulsarc.Beatmaps
{
    public class BeatmapData : SQLiteData
    {
        public string path;
        public string background_path;
        public string audio_path;
        public int audio_preview;
        public string fileName;
        public string title;
        public string artist;
        public string mapper;
        public string version;
        public int key_count;
        public double difficulty;

        public BeatmapData() : base() { }

        public BeatmapData(SQLiteDataReader data) : base(data) { }

        public BeatmapData(string path, string file) : this(BeatmapHelper.Load(path, file)) { }

        public BeatmapData(Beatmap beatmap)
        {
            path = beatmap.path;
            background_path = beatmap.Background;
            audio_path = beatmap.Audio;
            audio_preview = beatmap.PreviewTime;
            fileName = beatmap.fileName;
            title = beatmap.Title;
            artist = beatmap.Artist;
            mapper = beatmap.Mapper;
            version = beatmap.Version;
            key_count = beatmap.KeyCount;
            difficulty = beatmap.Difficulty;
        }

        public override string ToString()
        {
            return artist + " - " + title + " [" + version + "] (" + mapper + ")";
        }

        public bool match(string search)
        {
            if (artist.ToLower().Contains(search)) return true;
            if (title.ToLower().Contains(search)) return true;
            if (mapper.ToLower().Contains(search)) return true;
            if (version.ToLower().Contains(search)) return true;

            return false;
        }
    }
}
