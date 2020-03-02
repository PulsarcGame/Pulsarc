using Pulsarc.Beatmaps;
using System.Data.SQLite;

namespace Pulsarc.Utils.SQLite
{
    public class BeatmapData : SQLiteData
    {
        // Path to the beatmap folder and background image
        public string path;
        public string backgroundPath;

        // Path to the beatmap's audio
        public string audioPath;
        // Time (in ms) that the preview should start from in the song select
        public int audioPreviewTime;

        // Filename of the .psc
        public string fileName;

        // Metadata
        public string title;
        public string artist;
        public string mapper;
        public string version;

        // Game data
        public int keyCount;
        public double difficulty;

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
            path = beatmap.Path;
            backgroundPath = beatmap.Background;
            audioPath = beatmap.Audio;
            audioPreviewTime = beatmap.PreviewTime;
            fileName = beatmap.FileName;
            title = beatmap.Title;
            artist = beatmap.Artist;
            mapper = beatmap.Mapper;
            version = beatmap.Version;
            keyCount = beatmap.KeyCount;
            difficulty = beatmap.Difficulty;
        }

        /// <summary>
        /// The Beatmap as a string.
        /// Format is "Artist - Title [Version] (Mapper)"
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{artist} - {title} [{version}] ({mapper})";

        /// <summary>
        /// Find if this beatmap matches the search query.
        /// </summary>
        /// <param name="search">The query to search for.</param>
        /// <returns>True if search is found in this BeatmapData, false if not.</returns>
        public bool Match(string search)
        {
            bool matching =
                artist.ToLower().Contains(search) ||
                title.ToLower().Contains(search)  ||
                mapper.ToLower().Contains(search) ||
                version.ToLower().Contains(search);

            return matching;
        }
    }
}
