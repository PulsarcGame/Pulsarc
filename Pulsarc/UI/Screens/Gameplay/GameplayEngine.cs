using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Utils;
using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Result;
using System;
using System.Collections.Generic;
using Wobble.Screens;
using Pulsarc.Utils.Graphics;
using System.Linq;
using Pulsarc.Utils.SQLite;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class GameplayEngine : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }
        private GameplayEngineView GetGameplayView() { return (GameplayEngineView)View; }

        // Whether or not the gameplay engine is currently running
        public static bool Active { get; private set; } = false;

        // Whether or not the gameplay is automatically run
        public bool AutoPlay => Config.GetBool("Gameplay", "Autoplay");
        // Whether or not autoplay should use randomness.
        private bool autoPlayAddRandomness = false;

        public bool Hidden => Config.GetBool("Gameplay", "Hidden");

        // Keep track of whether or not any object is left to play
        public bool AtLeastOneLeft { get; private set; } = false;

        // Used for delaying the gameplay's end
        private bool ending => endTime != -1;
        private double endTime = -1;
        private const int endDelay = 2000;

        // Beatmap Elements
        // The current beatmap being played.
        public Beatmap CurrentBeatmap { get; private set; }

        // All the "tracks" or "directions" HitObjects can come from.
        public Column[] Columns { get; private set; }

        // The time for arcs to fade after being hit, defined by the user
        private int arcFadeTime => Config.GetInt("Gameplay", "FadeTime");

        // Used to store the key-style of the current map (4k, 7k, etc.)
        public int KeyCount { get; private set; }

        // Background
        public Background Background { get; private set; }

        // Events
        // Indexes
        private int eventIndex;

        // Next event to be activated
        public Event NextEvent { get; private set; }

        // Active Events that are currently being Handled
        public List<Event> ActiveEvents { get; private set; } = new List<Event>();

        // Gameplay Elements
        // The offset for this map determined by the player
        // TODO: add local beatmap offset that can be set by the player
        public double MapOffset => 0;

        public Crosshair Crosshair { get; private set; }

        // User-defined base speed
        // "5d" is used to give more choice in config for speed;
        public double UserSpeed => Config.GetDouble("Gameplay", "ApproachSpeed") / 5d / Rate;

        // Current speed modifier defined by the Beatmap
        public double CurrentSpeedMultiplier { get; set; }
        public double CurrentArcsSpeed { get; set; }

        // Judgement variables
        private List<KeyValuePair<double, int>> errors;
        private List<KeyValuePair<double, int>> rawInputs;
        public List<JudgementValue> Judgements { get; private set; }

        // Key bindings
        private Dictionary<Keys, int> bindings;

        private long maxScore;
        private long score;
        public int scoreDisplay { get; private set; }

        // The current combo during gameplay.
        public int Combo { get; private set; }

        // The highest combo obtained during gameplay thus far. 
        private int maxCombo;

        // Hidden value to determine score.
        private int comboMultiplier;

        // How fast the audio (and relevant gameplay) will play at.
        public float Rate => Config.GetFloat("Gameplay", "SongRate");

        // The current time of the song, which the gameplay engine
        // uses to determine arc positioning and event handling.
        public double Time => AudioManager.GetTime() + MapOffset;

        // Performance
        // Time distance (in ms) from which hitobjects are neither updated not drawn
        public int IgnoreTime { get; private set; } = 500;

        /// <summary>
        /// The engine that handles the gameplay of Pulsarc.
        /// </summary>
        public GameplayEngine()
        {
            View = new GameplayEngineView(this);
        }

        /// <summary>
        /// Initializes the current GameplayView with the provided beatmap.
        /// </summary>
        /// <param name="beatmap">The beatmap to play through</param>
        public void Init(Beatmap beatmap)
        {
            if (!beatmap.FullyLoaded)
                beatmap = BeatmapHelper.Load(beatmap.Path, beatmap.FileName);

            // Reset in case it wasn't properly handled outside
            Reset();

            // Load values gained from config/user settings
            LoadConfig();

            // Initialize default variables, parse beatmap
            InitializeVariables(beatmap);

            // Initialize Gameplay variables
            InitializeGameplay(beatmap);

            // Create columns and their hitobjects
            CreateColumns(beatmap);

            // Sort the hitobjects according to their first appearance for optimizing update/draw
            SortHitObjects();

            // Once everything is loaded, initialize the view
            GetGameplayView().Init();

            // Start audio and gameplay
            StartGameplay();

            // Collect any excess memory to prevent GC from starting soon, avoiding freezes.
            // TODO: disable GC while in gameplay
            GC.Collect();

            Init();
        }

        /// <summary>
        /// Legacy.
        /// Initialize this gameplay view by using the folder location and
        /// difficulty name to find the beatmap.
        /// </summary>
        /// <param name="folder">Beatmap folder name.</param>
        /// <param name="diff">Difficulty name for the beatmap.</param>
        public void Init(string folder, string diff)
        {
            Init(BeatmapHelper.Load("Songs/" + folder, diff + ".psc"));
        }

        #region Initializiation Methods
        /// <summary>
        /// Load all the stats found in the config
        /// </summary>
        private void LoadConfig()
        {
            // Set the offset for each play before starting audio
            AudioManager.offset = Config.GetInt("Audio", "GlobalOffset");

            KeyCount = 4;

            // TODO: Allow maps to start at different crosshair radii
            Crosshair = new Crosshair(300, Hidden);
        }

        /// <summary>
        /// Initialize default variables
        /// </summary>
        /// <param name="beatmap"></param>
        private void InitializeVariables(Beatmap beatmap)
        {
            AudioManager.audioRate = Rate;

            CurrentSpeedMultiplier = UserSpeed;
            CurrentArcsSpeed = 1;

            eventIndex = 0;

            // If there are events, make nextEvent the first event, otherwise make it null
            NextEvent = beatmap.Events.Count > 0 ? beatmap.Events[eventIndex] : null;
        }

        /// <summary>
        /// Initialize gameplay variables
        /// </summary>
        /// <param name="beatmap"></param>
        private void InitializeGameplay(Beatmap beatmap)
        {
            Columns = new Column[KeyCount];
            Judgements = new List<JudgementValue>();
            errors = new List<KeyValuePair<double, int>>();
            rawInputs = new List<KeyValuePair<double, int>>();
            bindings = new Dictionary<Keys, int>();

            Combo = 0;
            maxCombo = 0;
            comboMultiplier = Scoring.MaxComboMultiplier;
            score = 0;

            Background = new Background(Config.GetInt("Gameplay", "BackgroundDim") / 100f);
            Background.ChangeBackground(GraphicsUtils.LoadFileTexture(beatmap.Path + "/" + beatmap.Background));

            // Set the path of the song to be played later on
            AudioManager.songPath = beatmap.GetFullAudioPath();

            CurrentBeatmap = beatmap;
        }

        /// <summary>
        /// Create columns from the beatmap
        /// </summary>
        /// <param name="beatmap"></param>
        private void CreateColumns(Beatmap beatmap)
        {
            for (int i = 1; i <= KeyCount; i++)
                Columns[i - 1] = new Column(i);

            int objectCount = 0;

            // Add arcs to the columns
            foreach (Arc arc in CurrentBeatmap.Arcs)
                for (int i = 0; i < KeyCount; i++)
                    if (BeatmapHelper.IsColumn(arc, i))
                    {
                        Columns[i].AddHitObject(new HitObject(arc.Time, (int)(i / (float)KeyCount * 360), KeyCount, CurrentArcsSpeed, Hidden),
                                                CurrentArcsSpeed * CurrentSpeedMultiplier,
                                                Crosshair.GetZLocation());

                        objectCount++;
                    }

            // Compute the beatmap's highest possible score,
            // for displaying the current display_score later on
            maxScore = Scoring.GetMaxScore(objectCount);
        }

        /// <summary>
        /// Sort hit objects based on time so they draw correctly
        /// </summary>
        private void SortHitObjects()
        {
            foreach (Column col in Columns)
                col.SortUpdateHitObjects();

            // Load user bindings
            bindings.Add(Config.Bindings["Left"], 2);
            bindings.Add(Config.Bindings["Up"], 3);
            bindings.Add(Config.Bindings["Down"], 1);
            bindings.Add(Config.Bindings["Right"], 0);

            // Load autoplay by filling the input queue with desired inputs
            if (AutoPlay)
            {
                Keys[] presses =
                {
                    Config.Bindings["Right"],
                    Config.Bindings["Down"],
                    Config.Bindings["Left"],
                    Config.Bindings["Up"],
                };

                List<KeyValuePair<double, Keys>> inputs = new List<KeyValuePair<double, Keys>>();

                for (int i = 0; i < KeyCount; i++)
                    foreach (HitObject arc in Columns[i].HitObjects)
                        if (autoPlayAddRandomness)
                            // If enabled, add some randomness to the autoplay inputs. Legacy.
                            inputs.Add(new KeyValuePair<double, Keys>(arc.Time + (Math.Pow(new Random().Next(80) - 40, 3) / 1300), presses[i]));
                        else
                            // Otherwise autoplay will hit each arc perfectly.
                            inputs.Add(new KeyValuePair<double, Keys>(arc.Time, presses[i]));

                inputs.Sort((x, y) => x.Key.CompareTo(y.Key));

                foreach (KeyValuePair<double, Keys> input in inputs)
                    InputManager.KeyboardPresses.Enqueue(input);
            }
        }
        #endregion

        /// <summary>
        /// Start gameplay, activate audio, etc.
        /// </summary>
        private void StartGameplay()
        {
            AudioManager.StartGamePlayer();


            GameplayEngine.Active = true;
            Pulsarc.DisplayCursor = false;
        }

        /// <summary>
        /// Updates everything during gameplay
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public override void Update(GameTime gameTime)
        {
            // If not active, don't update.
            if (!Active)
                return;

            // Engine stuff (pause, continue, end, restart)
            HandleEngineInputs();

            // If paused, don't handle anything else
            if (AudioManager.paused)
                return;

            // Quit gameplay when nothing is left to play, or if the audio finished playing
            if ((AudioManager.active && AudioManager.FinishedPlaying()) || (ending && Time >= endTime + endDelay))
            {                
                EndGameplay(true);
                return;
            }

            // Handle user input in priority
            HandleInputs(gameTime);
            
            // Event Handling
            HandleEvents();

            // Update objects positions and UI like Judges
            UpdateGameplay();

            // Reprocess the displayed score
            UpdateScoreDisplay();

            // Update other display elements
            View.Update(gameTime);

            // End gameplay with a delay if needed
            DelayEndGameplay();
        }

        #region Update Methods
        /// <summary>
        /// Handle inputs that pause, continue, leave, or restart gameplay
        /// </summary>
        private void HandleEngineInputs()
        {
            // End the gameplay with the "escape" key TODO: make this key bindable.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                EndGameplay();
                return;
            }

            // Pause Gameplay with bindable "Pause" key.
            if (Keyboard.GetState().IsKeyDown(Config.Bindings["Pause"]))
                Pause();

            // Resume Gameplay with bindable "Continue" key.
            if (Keyboard.GetState().IsKeyDown(Config.Bindings["Continue"]))
                Resume();

            // Restart gameplay using bindable "Retry" key.
            if (Keyboard.GetState().IsKeyDown(Config.Bindings["Retry"]))
                Retry();
        }

        /// <summary>
        /// Handle the currently queued Inputs that may affect the gameplay
        /// </summary>
        private void HandleInputs(GameTime gameTime)
        {
            // Prevents future input from being handled. Useful for auto. Remove for quick auto result testing
            while (InputManager.KeyboardPresses.Count > 0
                && InputManager.KeyboardPresses.Peek().Key <= AudioManager.GetTime())
            {
                KeyValuePair<double, Keys> press = InputManager.KeyboardPresses.Dequeue();

                // Process a hit if the pressed key corresponds to a bound key
                if(bindings.ContainsKey(press.Value))
                {
                    HitObject pressed = null;
                    int column = bindings[press.Value];
                    rawInputs.Add(new KeyValuePair<double, int>(press.Key, column));

                    // Check the first hitobject of the corresponding column if there is >= one
                    if (Columns[column].HitObjects.Count > 0 && Columns[column].HitObjects.Exists(x => x.Hittable))
                    {
                        pressed = Columns[column].HitObjects.Find(hO => hO.Hittable);

                        int error = (int)((pressed.Time - press.Key) / Rate);

                        // Get the judge for the timing error
                        JudgementValue judge = Judgement.GetJudgementValueByError(Math.Abs(error));

                        // If no judge is obtained, it is a ghost hit and is ignored
                        if (judge != null)
                        {
                            // Otherwise, handle the hit according to the judge
                            GetGameplayView().AddHit(press.Key, error, judge.Score);

                            // Add a Fading HitObject, and mark the pressed HitObject for removal.
                            if (!pressed.Hidden)
                                Columns[column].AddHitObject(   new HitObjectFade(pressed, arcFadeTime, KeyCount),
                                                                CurrentArcsSpeed * CurrentSpeedMultiplier,
                                                                Crosshair.GetZLocation());

                            pressed.ToErase = true;

                            Columns[column].HitObjects.Remove(pressed);

                            // Take care of judgement of the hit.
                            errors.Add(new KeyValuePair<double, int>(press.Key, error));
                            Judgements.Add(judge);

                            KeyValuePair<long, int> hitResult = Scoring.ProcessHitResults(judge, score, comboMultiplier);
                            score = hitResult.Key;
                            comboMultiplier = hitResult.Value;

                            if (judge.Score > 0)
                            {
                                Combo++;

                                if(Combo > maxCombo)
                                    maxCombo = Combo;
                            }
                            else
                                Combo = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the Arcs and gameplay-related UI
        /// </summary>
        private void UpdateGameplay()
        {
            if (CurrentBeatmap == null)
                return;

            AtLeastOneLeft = false;

            for (int i = 0; i < KeyCount; i++)
            {
                bool updatedAll = false;

                // For readability
                ref Column currentColumn = ref Columns[i];

                for (int k = 0; k < currentColumn.UpdateHitObjects.Count && !updatedAll; k++)
                {
                    // For readability
                    // can't ref properties, but this doesn't hurt performance
                    // when compared to the old: "Columns[i].UpdateHitObjects[k].Value"
                    HitObject currentHitObject = currentColumn.UpdateHitObjects[k].Value;

                    // Remove the hitobject if it is marked for removal
                    if (currentHitObject.ToErase)
                    {
                        currentColumn.UpdateHitObjects.RemoveAt(k);
                        continue;
                    }

                    // Process the new position of this object
                    currentHitObject.RecalcPos((int)Time, CurrentSpeedMultiplier, Crosshair.GetZLocation());
                    AtLeastOneLeft = true;

                    // Ignore the following objects if we have reached the ignored distance
                    if (currentColumn.UpdateHitObjects[k].Key - IgnoreTime > Time)
                        updatedAll = true;

                    // Determine whether or not this note has been missed by the user, and take action if so
                    if (currentHitObject.Time + (Judgement.GetMiss().Judge * Rate) < Time
                        && currentHitObject.Hittable)
                    {
                        // Remove the hitobject and reset the combo
                        currentColumn.HitObjects.Remove(currentHitObject);
                        currentColumn.UpdateHitObjects.RemoveAt(k);
                        k--;
                        Combo = 0;

                        // Add a miss to the score and obtained judgements, then display it
                        JudgementValue miss = Judgement.GetMiss();

                        KeyValuePair<long, int> hitResult = Scoring.ProcessHitResults(miss, score, comboMultiplier);
                        score = hitResult.Key;
                        comboMultiplier = hitResult.Value;
                        GetGameplayView().AddJudge(Time, miss.Score);
                        Judgements.Add(miss);
                    }
                }
            }
        }

        /// <summary>
        /// Update score_display according to the maximum displayed score.
        /// </summary>
        private void UpdateScoreDisplay()
        {
            scoreDisplay = (int)(score / (float)maxScore * Scoring.MaxScore);
        }

        /// <summary>
        /// Handles all Beatmap events.
        /// </summary>
        private void HandleEvents()
        {
            if (CurrentBeatmap == null)
                return;

            // Events
            ActivateNextEvent();

            // Handle all active events, and remove inactive Events
            HandleActiveEvents();
        }

        private void ActivateNextEvent()
        {
            // If the current event Index is within range, and the next event time is less than or equal to the current time
            if (CurrentBeatmap.Events.Count > eventIndex && NextEvent.Time <= Time)
            {
                // Start handling the current event, and increase the event index
                NextEvent.Active = true;
                ActiveEvents.Add(NextEvent);
                eventIndex++;

                // Get ready for the next event, if it exists
                if (CurrentBeatmap.Events.Count > eventIndex)
                    NextEvent = CurrentBeatmap.Events[eventIndex];
            }
        }

        private void HandleActiveEvents()
        {
            for(int i = 0; i < ActiveEvents.Count; i++)
                // If the event is active, handle it
                if (ActiveEvents[i].Active)
                    ActiveEvents[i].Handle(this);
                // Otherwise, add remove it
                else
                    ActiveEvents.RemoveAt(i--);
        }

        /// <summary>
        /// Start the end-delay timer if there are no more arcs to hit
        /// </summary>
        private void DelayEndGameplay()
        {
            if (endTime == -1 && !AtLeastOneLeft)
                endTime = Time;
        }
        #endregion

        /// <summary>
        /// Move the gameplay backwards or forward in time.
        /// </summary>
        /// <param name="delta">How much time to move.</param>
        public void MoveTime(long delta)
        {
            AudioManager.DeltaTime(delta);
        }

        /// <summary>
        /// Pause the gameplay.
        /// </summary>
        public void Pause()
        {
            AudioManager.Pause();
        }

        /// <summary>
        /// Resume gameplay.
        /// </summary>
        public void Resume()
        {
            AudioManager.Resume();
        }

        /// <summary>
        /// Restart gameplay
        /// </summary>
        public void Retry()
        {
            Beatmap retry = CurrentBeatmap;

            Reset();

            Init(retry);
        }

        /// <summary>
        /// Reset this GameplayEngine, use before retrying or changing to a new map.
        /// </summary>
        public void Reset()
        {
            Active = false;

            // Clear Input and Audio
            InputManager.KeyboardPresses.Clear();
            AudioManager.Stop();

            // Unset attributes to avoid potential conflict with next gameplays
            CurrentBeatmap = null;
            Columns = null;

            ActiveEvents = new List<Event>();
            NextEvent = null;

            // Reset Attributes
            CurrentSpeedMultiplier = 1;
            CurrentArcsSpeed = 1;
        }

        /// <summary>
        /// Stop gameplay and remove this engine from view.
        /// </summary>
        public void EndGameplay(bool save = false)
        {
            Background.Dimmed = false;

            // Stop audio
            if (AudioManager.running)
                AudioManager.Stop();

            // Save rplay data if this is a valid play
            if(!AutoPlay && save)
                DataManager.ScoreDB.AddReplay(new ReplayData(CurrentBeatmap.GetHash(), string.Join(",", rawInputs)));

            // Create the result screen before exiting gameplay
            ResultScreen next = new ResultScreen(Judgements, errors, scoreDisplay, maxCombo, Rate, 0, CurrentBeatmap, Background, !AutoPlay && save);
            Pulsarc.DisplayCursor = true;
            Reset();

            // Switch to results screen
            ScreenManager.RemoveScreen(true);
            ScreenManager.AddScreen(next);

            // TODO: restart GC when out of gameplay
            GC.Collect();
        }

        public override void UpdateDiscord()
        {
            PulsarcDiscord.SetStatus("Playing Singleplayer", CurrentBeatmap.Title);
        }
    }
}
