using Pulsarc.Beatmaps;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Audio;
using Pulsarc.Utils.Graphics;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.BaseEngine
{
    public abstract class ArcCrosshairEngine : PulsarcScreen, IEventHandleable
    {
        public override ScreenView View { get; protected set; }
        private ArcCrosshairEngineView GetView() => (ArcCrosshairEngineView)View;

        public virtual bool Hidden { get; protected set; }

        // Beatmap Elements
        // The current beatmap being played.
        public Beatmap CurrentBeatmap { get; protected set; }

        // All the "tracks" or "directions" HitObjects can come from.
        public Column[] Columns { get; protected set; }

        // Used to store the key-style of the current map (4k, 7k, etc.)
        public int KeyCount { get; private set; }

        // Background
        public Background Background { get; protected set; }

        protected Crosshair Crosshair { get; set; }

        // User-defined base speed
        // "5f" is used to give more choice in config for speed;
        public double UserSpeed => Config.GetInt("Gameplay", "ApproachSpeed") / 5f / Rate;

        // Current speed modifier defined by the Beatmap
        public double CurrentSpeedMultiplier { get; set; }
        public double CurrentArcsSpeed { get; set; }

        // How fast the audio (and relevant gameplay) will play at.
        public float Rate { get; protected set; }

        // The current time of the song, which the engine
        // uses to determine arc positioning and event handling.
        protected double Time
        {
            get => AudioManager.GetTime();
            set => AudioManager.Seek(value);
        }

        //Performance
        // Time distance (in ms) from which hitobjects are neither updated not drawn
        public int IgnoreTime { get; private set; } = 500;

        public ArcCrosshairEngine() => View = SetView();

        /// <summary>
        /// Returns the view of this ArcCrosshairEngine
        /// </summary>
        /// <returns></returns>
        protected abstract ScreenView SetView();

        /// <summary>
        /// Initializes the ArcCrosshairEngine
        /// </summary>
        /// <param name="beatmap">The beatmap to be loaded into this engine.</param>
        public virtual void Init(Beatmap beatmap)
        {
            // If the map isn't fully loaded yet, do so now.
            if (!beatmap.FullyLoaded)
            {
                beatmap = BeatmapHelper.Load(beatmap.Path, beatmap.FileName);
            }

            // Load values gained from config/user settings
            LoadConfig();

            // Initialize default variables, parse beatmap
            InitializeVariables(beatmap);

            // Initialize Gameplay variables
            InitializeEngine(beatmap);

            // Create columns and their hitobjects
            CreateColumns();

            // Once everything is loaded, initialize the view
            GetView().Init();
        }

        #region Initializiation Methods
        /// <summary>
        /// Load all the stats found in the config
        /// </summary>
        protected virtual void LoadConfig()
        {
            // Set the offset for each play before starting audio
            AudioManager.Offset = Config.GetInt("Audio", "GlobalOffset");

            KeyCount = 4;

            // TODO: Allow maps to start at different crosshair radii
            Crosshair = new Crosshair(300, Hidden);

            //CurrentSpeedMultiplier = UserSpeed;
            //CurrentArcsSpeed = 1;
        }

        /// <summary>
        /// Initialize default variables
        /// </summary>
        /// <param name="beatmap">The beatmap to load.</param>
        protected abstract void InitializeVariables(in Beatmap beatmap);

        /// <summary>
        /// Initializes base variables needed by any ArcCrosshairEngine
        /// </summary>
        /// <param name="beatmap"></param>
        protected void InitializeEngine(in Beatmap beatmap)
        {
            Columns = new Column[KeyCount];

            Background = new Background(Config.GetInt("Gameplay", "BackgroundDim") / 100f);
            Background.ChangeBackground(
                GraphicsUtils.LoadFileTexture(beatmap.Path + "/" + beatmap.Background));

            AudioManager.SongPath = beatmap.GetFullAudioPath();

            CurrentBeatmap = beatmap;
        }

        /// <summary>
        /// Create columns and fill them with the needed arcs from the Current Beatmap.
        /// </summary>
        private void CreateColumns()
        {
            // Create one column for each key being used
            for (int i = 1; i <= KeyCount; i++)
            {
                Columns[i - 1] = new Column(i);
            }

            // Add arcs to the columns
            foreach (Arc arc in CurrentBeatmap.Arcs)
            {
                // Check each column to see where this arc should go
                for (int i = 0; i < KeyCount; i++)
                {
                    if (BeatmapHelper.IsColumn(arc, i))
                    {
                        AddHitObjectToColumn(arc, i);
                    }
                }
            }

            // Sort hit objects based on time so they draw correctly.
            foreach (Column col in Columns)
            {
                col.SortUpdateHitObjects();
            }
        }

        protected abstract void AddHitObjectToColumn(Arc arc, int colIndex);
        #endregion

        #region IEventHandleable Methods
        /// <summary>
        /// Returns the current beatmap.
        /// Used for event handling.
        /// </summary>
        /// <returns>The current beatmap of the engine.</returns>
        public Beatmap GetCurrentBeatmap() => CurrentBeatmap;

        /// <summary>
        /// Whether or not this engine uses a crosshair.
        /// Used for event handling.
        /// </summary>
        /// <returns><see langword="true"/>If the crosshair isn't null.</returns>
        public bool HasCrosshair() => Crosshair != null;

        /// <summary>
        /// Returns the crosshair of this engine, if it exists.
        /// Used for event handling.
        /// </summary>
        /// <returns>The crosshair of this engine, or null if it doesn't exist yet.</returns>
        public Crosshair GetCrosshair() => Crosshair;

        /// <summary>
        /// Returns the current time of this engine.
        /// Used for event handling.
        /// </summary>
        /// <returns>The current time in the engine. Note: this is audio time, not real time.</returns>
        public double GetCurrentTime() => Time;
        #endregion
    }
}
