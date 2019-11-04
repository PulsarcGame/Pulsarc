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
        // Path to the beatmap folder and background image
        public string Path;
        public string BackgroundPath;

        // Path to the beatmap's audio
        public string AudioPath;
        // Time (in ms) that the preview should start from in the song select
        public int AudioPreviewTime;

        // Filename of the .psc
        public string FileName;

        // Metadata
        public string Title;
        public string Artist;
        public string Mapper;
        public string Version;

        // Game data
        public int KeyCount;
        public double Difficulty;

        /// <summary>
        /// Make a new BeatmapData
        /// </summary>
        public BeatmapData() : base() { }

        /// <summary>
        /// Make a new BeatmapData out of a DataReader
        /// </summary>
        /// <param name="data"></param>
        public BeatmapData(SQLiteDataReader data) : base(data) { }

        /// <summary>
        /// Make a new BeatmapData out of a Beatmap by loading
        /// the Beatmap from the provided path.
        /// </summary>
        /// <param name="path">Path to the Beatmap folder.</param>
        /// <param name="file">File name of the .psc Beatmap.</param>
        public BeatmapData(string path, string file) : this(BeatmapHelper.Load(path, file)) { }

        /// <summary>
        /// Make a new BeatmapData out of a Beatmap.
        /// </summary>
        /// <param name="beatmap">The Beatmap used to make this BeatmapData</param>
        public BeatmapData(Beatmap beatmap)
        {
            Path = beatmap.Path;
            BackgroundPath = beatmap.Background;
            AudioPath = beatmap.Audio;
            AudioPreviewTime = beatmap.PreviewTime;
            FileName = beatmap.FileName;
            Title = beatmap.Title;
            Artist = beatmap.Artist;
            Mapper = beatmap.Mapper;
            Version = beatmap.Version;
            KeyCount = beatmap.KeyCount;
            Difficulty = beatmap.Difficulty;
        }

        /// <summary>
        /// The Beatmap as a string.
        /// Format is "Artist - Title [Version] (Mapper)"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Artist} - {Title} [{Version}] ({Mapper})";
        }

        /// <summary>
        /// Find if this beatmap matches the search query.
        /// </summary>
        /// <param name="search">The query to search for.</param>
        /// <returns>True if search is found in this BeatmapData, false if not.</returns>
        public bool Match(string search)
        {
            bool matching =
                Artist.ToLower().Contains(search) ||
                Title.ToLower().Contains(search)  ||
                Mapper.ToLower().Contains(search) ||
                Version.ToLower().Contains(search);

            return matching;
        }
    }
}
