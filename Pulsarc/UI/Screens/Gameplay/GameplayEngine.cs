using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.Utils;
using Pulsarc.UI.Screens.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Wobble.Screens;
using Pulsarc.UI.Common;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class GameplayEngine : Screen
    {
        public override ScreenView View { get ; protected set; }
        private GameplayEngineView getGameplayView() { return (GameplayEngineView)View; }

        // Whether or not the gameplay engine is currently running
        public static bool active = false;

        // temp: Whether or not the gameplay is automatically run
        bool autoPlay = false;

        // Whether or not autoplay should use randomness.
        bool autoPlayRandom = false;

        // Used for delaying the gameplay's end
        Stopwatch endWatch;
        public int endDelay = 2000;


        // Beatmap Elements
        
        // The current beatmap being played.
        public Beatmap currentBeatmap;

        // All the "tracks" or "directions" HitObjects can come from.
        public Column[] columns;

        // The time for arcs to fade after being hit, defined by the user
        public int timeToFade;

        // Used to store the key-style of the current map (4k, 7k, etc.)
        public int keys;

        // Background
        public Background background;

        // Events indexes
        public int speedVariationIndex;
        public int eventIndex;


        // Gameplay Elements
        public double timeOffset;

        public Crosshair crosshair;

        // User-defined base speed
        public double userSpeed;

        // Current speed modifier defined by the Beatmap
        public double currentSpeedMultiplier;
        public double currentArcsSpeed;
        public List<KeyValuePair<double, int>> errors;
        public List<JudgementValue> judgements;
        public Dictionary<Keys, int> bindings;

        public long max_score;
        public long score;
        public int score_display;

        // The current combo during gameplay.
        public int combo;

        // The highest combo obtained during gameplay thus far. 
        public int max_combo;

        // Hidden value to determine score.
        public int combo_multiplier;

        // How fast the audio (and relevant gameplay) will play at.
        public float rate;

        public double time => AudioManager.getTime() + timeOffset;


        // Performance

        // Time distance from which hitobjects are neither updated not drawn
        public int msIgnore = 500;

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
            // Reset in case it wasn't properly handled outside
            Reset();

            // Set the offset for each play before starting audio
            // TODO: add local beatmap offset
            AudioManager.offset = Config.getInt("Audio", "GlobalOffset");

            // temp: These values should be obtained from mods/config/beatmap parsing
            rate = 1f; 
            keys = 4;
            userSpeed = Config.getInt("Gameplay", "ApproachSpeed") / 5f / rate; // "5f" is used to give more choice in config for speed

            timeToFade = Config.getInt("Gameplay", "FadeTime");

            crosshair = new Crosshair(300); // 300 = base crosshair diameter in intralism
            timeOffset = 0;

            // Initialize default variables, parse beatmap
            endWatch = new Stopwatch();
            AudioManager.audioRate = rate;

            currentSpeedMultiplier = userSpeed;
            currentArcsSpeed = 1;

            speedVariationIndex = 0;
            eventIndex = 0;

            // Initialize Gameplay variables
            columns = new Column[keys];
            judgements = new List<JudgementValue>();
            errors = new List<KeyValuePair<double, int>>();
            bindings = new Dictionary<Keys, int>();

            combo = 0;
            max_combo = 0;
            combo_multiplier = Scoring.max_combo_multiplier;
            score = 0;

            background = new Background(Config.getInt("Gameplay", "BackgroundDim") / 100f);
            background.changeBackground(beatmap.path, beatmap.Background);

            currentBeatmap = beatmap;

            // Set the path of the song to be played later on
            AudioManager.song_path = Directory  .GetParent(currentBeatmap.path) // Get the path to "\Songs"
                                                .FullName.Replace("\\Songs", "") + // Get rid of the extra "\Songs"
                                                "\\" + beatmap.path + // Add the beatmap path.
                                                "\\" + currentBeatmap.Audio; // Add the audio name.
            
            // Create columns and their hitobjects
            for (int i = 1; i <= keys; i++)
            {
                columns[i - 1] = new Column(i);
            }

            int objectCount = 0;
            int speedVarInitIndex = 0;
            
            foreach (Arc arc in currentBeatmap.arcs)
            {
                // Go through events to update current arcs speed
                while(currentBeatmap.speedVariations.Count > speedVarInitIndex && currentBeatmap.speedVariations[speedVarInitIndex].time <= arc.time)
                {
                    switch (currentBeatmap.speedVariations[speedVarInitIndex].type)
                    {
                        case 1:
                            // Arcs spawn speed change
                            //currentArcsSpeed = currentBeatmap.speedVariations[speedVarInitIndex].intensity;
                            break;
                    }
                    speedVarInitIndex++;
                }

                // Add arcs to the columns
                for (int i = 0; i < keys; i++)
                {
                    if (BeatmapHelper.isColumn(arc, i))
                    {
                        columns[i].AddHitObject(new HitObject(arc.time, (int)(i / (float)keys * 360), keys, currentArcsSpeed), currentArcsSpeed * currentSpeedMultiplier, crosshair.getZLocation());
                        objectCount++;
                    }
                }
            }

            // Compute the beatmap's highest possible score, for displaying the current display_score later on
            max_score = Scoring.getMaxScore(objectCount);

            // Sort the hitobjects according to their first appearance for optimizing update/draw
            foreach (Column col in columns)
            {
                col.SortUpdateHitObjects();
            }

            // Load user bindings
            bindings.Add(Config.bindings["Left"], 2);
            bindings.Add(Config.bindings["Up"], 3);
            bindings.Add(Config.bindings["Down"], 1);
            bindings.Add(Config.bindings["Right"], 0);

            // Load autoplay by filling the input queue with desired inputs
            if (autoPlay)
            {
                Keys[] presses =
                {
                    Config.bindings["Right"],
                    Config.bindings["Down"],
                    Config.bindings["Left"],
                    Config.bindings["Up"],
                };

                List<KeyValuePair<double, Keys>> inputs = new List<KeyValuePair<double, Keys>>();

                for (int i = 0; i < keys; i++)
                {
                    foreach (HitObject arc in columns[i].hitObjects)
                    {
                        if (autoPlayRandom)
                        {
                            inputs.Add(new KeyValuePair<double, Keys>(arc.time + Math.Pow(new Random().Next(80) - 40, 3) / 1300, presses[i]));
                        }
                        else
                        {
                            inputs.Add(new KeyValuePair<double, Keys>(arc.time, presses[i]));
                        }
                    }
                }

                inputs.Sort((x, y) => x.Key.CompareTo(y.Key));

                foreach (KeyValuePair<double, Keys> input in inputs)
                {
                    InputManager.keyboardPresses.Enqueue(input);
                }
            }

            // Once everything is loaded, initialize the view
            getGameplayView().Init();

            // Start audio and gameplay
            AudioManager.Start();
            GameplayEngine.active = true;
            Pulsarc.display_cursor = false;

            // Collect any excess memory to prevent GC from starting soon, avoiding freezes.
            // TODO: disable GC while in gameplay
            GC.Collect();
        }

        /// <summary>
        /// Initialize this gameplay view by using the folder location and
        /// difficulty name to find the beatmap. Legacy.
        /// </summary>
        /// <param name="folder">Beatmap folder name.</param>
        /// <param name="diff">Difficulty name for the beatmap.</param>
        public void Init(string folder, string diff)
        {
            // Legacy
            Init(BeatmapHelper.Load("Songs/" + folder, diff + ".psc"));
        }

        public override void Update(GameTime gameTime)
        {
            // If not active, don't update.
            if (!active) return;

            // Quit gameplay when nothing is left to play in terms of Audio.
            // Could be improved to respect an EndDelay timer.
            if (AudioManager.active && AudioManager.FinishedPlaying())
            {
                EndGameplay();
                return;
            }

            // Handle user input in priority
            handleInputs(gameTime);

            // Gameplay commands
            // End the gameplay with the "escape" key TODO: make this key bindable.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                EndGameplay();
                return;
            }

            // Pause Gameplay with bindable "Pause" key.
            if (Keyboard.GetState().IsKeyDown(Config.bindings["Pause"]))
            {
                Pause();
            }
            
            // Resume Gameplay with bindable "Continue" key.
            if (Keyboard.GetState().IsKeyDown(Config.bindings["Continue"]))
            {
                Resume();
            }
            
            // Restart gameplay using bindable "Retry" key.
            if (Keyboard.GetState().IsKeyDown(Config.bindings["Retry"]))
            {
                Retry();
                return;
            }

            // Keep track of whether or not any object is left to play
            bool atLeastOne = false;

            // Handle all events
            // Speed variations
            if (currentBeatmap.speedVariations.Count > speedVariationIndex && currentBeatmap.speedVariations[speedVariationIndex].time <= time)
            {
                switch (currentBeatmap.speedVariations[speedVariationIndex].type)
                {
                    case 0:
                        // Global speed change
                        //currentSpeedMultiplier = userSpeed * (1/currentBeatmap.speedVariations[speedVariationIndex].intensity);
                        break;
                }
                speedVariationIndex++;
            }

            // Events
            if (currentBeatmap.events.Count > eventIndex + 1 && currentBeatmap.events[eventIndex].time <= time)
            {
                currentBeatmap.events[eventIndex].Handle(this);
                eventIndex++;
            }

            // Update UI and objects positions
            for (int i = 0; i < keys; i++)
            {
                bool updatedAll = false;
                for (int k = 0; k < columns[i].updateHitObjects.Count && !updatedAll; k++)
                {
                    // Remove the hitobject if it is marked for removal before updating it
                    if (columns[i].updateHitObjects[k].Value.erase)
                    {
                        columns[i].updateHitObjects.RemoveAt(k);
                        continue;
                    }

                    // Process the new position of this object
                    columns[i].updateHitObjects[k].Value.recalcPos((int)time, currentSpeedMultiplier, crosshair.getZLocation());
                    atLeastOne = true;

                    // Ignore the following objects if we have reached the ignored distance
                    if (columns[i].updateHitObjects[k].Key - msIgnore > time)
                    {
                        updatedAll = true;
                    }

                    // Determine whether or not this note has been missed by the user, and take action if so
                    if (columns[i].updateHitObjects[k].Value.time + Judgement.getMiss().judge * rate < time && columns[i].updateHitObjects[k].Value.hittable)
                    {
                        // Remove the hitobject and reset the combo
                        columns[i].hitObjects.Remove(columns[i].updateHitObjects[k].Value);
                        columns[i].updateHitObjects.RemoveAt(k);
                        k--;
                        combo = 0;

                        // Add a miss to the score and obtained judgements, then display it
                        JudgementValue miss = Judgement.getMiss();

                        KeyValuePair<long, int> hitResult = Scoring.processHitResults(miss, score, combo_multiplier);
                        score = hitResult.Key;
                        combo_multiplier = hitResult.Value;
                        getGameplayView().addJudge(time, miss.score);
                        judgements.Add(miss);
                    }
                }
            }

            // Reprocess the displayed score
            updateScoreDisplay();

            // Update other display elements
            View.Update(gameTime);

            // End gameplay with a delay if needed
            if (!atLeastOne)
            {
                if (!endWatch.IsRunning)
                {
                    endWatch.Start();
                }
                else
                {
                    if (endWatch.ElapsedMilliseconds >= endDelay)
                    {
                        EndGameplay();
                    }
                }

            }
        }

        /// <summary>
        /// Update score_display according to the maximum displayed score.
        /// </summary>
        private void updateScoreDisplay()
        {
            score_display = (int) (score / (float) max_score * Scoring.max_score);
        }

        /// <summary>
        /// Move the gameplay backwards or forward in time.
        /// </summary>
        /// <param name="delta">How much time to move.</param>
        public void deltaTime(long delta)
        {
            AudioManager.deltaTime(delta);
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
            Beatmap retry = currentBeatmap;

            Reset();

            Init(retry);
        }

        /// <summary>
        /// Reset this GameplayEngine, use before retrying or changing to a new map.
        /// </summary>
        public void Reset()
        {
            active = false;

            // Clear Input and Audio
            InputManager.keyboardPresses.Clear();
            AudioManager.Stop();

            // Unset attributes to avoid potential conflict with next gameplays
            currentBeatmap = null;
            columns = null;

            // Reset Attributes
            userSpeed = 1;
            currentSpeedMultiplier = 1;
            currentArcsSpeed = 1;
        }

        /// <summary>
        /// Handle the currently queued Inputs that may affect the gameplay
        /// </summary>
        public void handleInputs(GameTime gameTime)
        {
            while (InputManager.keyboardPresses.Count > 0 
                && InputManager.keyboardPresses.Peek().Key <= AudioManager.getTime()) // Prevents future input from being handled. Useful for auto. Remove for quick auto result testing
            {
                KeyValuePair<double, Keys> press = InputManager.keyboardPresses.Dequeue();

                // Process a hit if the pressed key corresponds to a bound key
                if(bindings.ContainsKey(press.Value)) { 
                    HitObject pressed = null;
                    var column = bindings[press.Value];

                    // Check the first hitobject of the corresponding column if there is >= one
                    if (columns[column].hitObjects.Count > 0 && columns[column].hitObjects.Exists(x => x.hittable))
                    {
                        pressed = columns[column].hitObjects.Find(x => x.hittable);

                        int error = (int)((pressed.time - press.Key) / rate);

                        // Get the judge for the timing error
                        JudgementValue judge = Judgement.getErrorJudgementValue(Math.Abs(error));

                        // If no judge is obtained, it is a ghost hit and is ignored
                        if (judge != null)
                        {
                            // Otherwise, handle the hit according to the judge

                            getGameplayView().addHit(press.Key, error, judge.score);

                            // Add a Fading HitObject, and mark the pressed HitObject for removal.
                            columns[column].AddHitObject(new HitObjectFade(pressed, timeToFade, gameTime), pressed.baseSpeed, crosshair.getZLocation());
                            pressed.erase = true;

                            columns[column].hitObjects.Remove(pressed);

                            // Take care of judgement of the hit.
                            errors.Add(new KeyValuePair<double, int>(press.Key, error));
                            judgements.Add(judge);

                            KeyValuePair<long, int> hitResult = Scoring.processHitResults(judge, score, combo_multiplier);
                            score = hitResult.Key;
                            combo_multiplier = hitResult.Value;

                            if (judge.score > 0)
                            {
                                combo++;
                                if(combo > max_combo)
                                {
                                    max_combo = combo;
                                }
                            }
                            else
                            {
                                combo = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stop gameplay and remove this engine from displaying.
        /// </summary>
        public void EndGameplay()
        {
            // Create the result screen before exiting gameplay
            ResultScreen next = new ResultScreen(judgements, errors, score_display, max_combo, currentBeatmap, background);
            Pulsarc.display_cursor = true;
            Reset();
            // Switch to results screen
            ScreenManager.RemoveScreen(true);
            ScreenManager.AddScreen(next);

            // TODO: restart GC when out of gameplay
            GC.Collect();
        }
    }
}
