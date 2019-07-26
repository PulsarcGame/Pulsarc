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

namespace Pulsarc.UI.Screens.Gameplay
{
    public class GameplayEngine : Screen
    {
        public override ScreenView View { get ; protected set; }
        private GameplayEngineView getGameplayView() { return (GameplayEngineView)View; }

        public static bool active = false;
        bool autoPlay = false;

        Stopwatch endWatch;

        // Beatmap Elements

        public Beatmap currentBeatmap;
        public Column[] columns;
        public int keys;

        // Gameplay Elements

        public long timeOffset;
        public int currentCrosshairRadius;
        public double userSpeed;
        public double currentSpeedMultiplier;
        public double currentArcsSpeed;
        public List<KeyValuePair<long, int>> errors;
        public List<JudgementValue> judgements;

        public long max_score;
        public long score;
        public int score_display;
        public int combo;
        public int combo_multiplier;
        public float rate;

        public int endDelay = 2000;

        // Performance
        public int msIgnore = 500;

        public GameplayEngine()
        {
            View = new GameplayEngineView(this);
        }

        public void Init(Beatmap beatmap)
        {

            // Initialize default variables, parse beatmap
            endWatch = new Stopwatch();
            rate = 1f;
            keys = 4;
            userSpeed = 1 / rate / rate;
            currentCrosshairRadius = 200;
            timeOffset = 0;

            AudioManager.audioRate = rate;

            currentSpeedMultiplier = userSpeed;
            currentArcsSpeed = 1;

            // Initialize Gameplay variables
            columns = new Column[keys];
            judgements = new List<JudgementValue>();
            errors = new List<KeyValuePair<long, int>>();

            combo = 0;
            combo_multiplier = Scoring.max_combo_multiplier;
            score = 0;

            currentBeatmap = beatmap;

            AudioManager.song_path = Directory.GetParent(currentBeatmap.path).FullName + "\\" + currentBeatmap.Audio;

            for (int i = 1; i <= keys; i++)
            {
                columns[i - 1] = new Column(i);
            }

            int objectCount = 0;
            foreach (Arc arc in currentBeatmap.arcs)
            {
                for (int i = 0; i < keys; i++)
                {
                    // Use bitwise check to know if the column is concerned by this arc event
                    if (((arc.type >> i) & 1) != 0)
                    {
                        columns[i].AddHitObject(new HitObject(arc.time, (int)(i / (float)keys * 360), keys, currentArcsSpeed), currentArcsSpeed * currentSpeedMultiplier);
                        objectCount++;
                    }
                }
            }
            max_score = Scoring.getMaxScore(objectCount);

            foreach (Column col in columns)
            {
                col.SortUpdateHitObjects();
            }

            if (autoPlay)
            {

                List<KeyValuePair<long, Keys>> inputs = new List<KeyValuePair<long, Keys>>();

                for (int i = 0; i < keys; i++)
                {
                    foreach (HitObject arc in columns[i].hitObjects)
                    {
                        Keys press = Keys.D;
                        switch (i)
                        {
                            case 2:
                                press = Keys.D;
                                break;
                            case 3:
                                press = Keys.F;
                                break;
                            case 1:
                                press = Keys.J;
                                break;
                            case 0:
                                press = Keys.K;
                                break;
                        }

                        inputs.Add(new KeyValuePair<long, Keys>(arc.time + (long) Math.Pow(new Random().Next(80)-40,3)/600, press));
                    }
                }

                inputs.Sort((x, y) => x.Key.CompareTo(y.Key));

                foreach (KeyValuePair<long, Keys> input in inputs)
                {
                    KeyboardInputManager.keyboardPresses.Enqueue(input);
                }
            }

            getGameplayView().Init();

            AudioManager.Start();
            GameplayEngine.active = true;
        }

        public void Init(string folder, string diff)
        {
            Init(BeatmapHelper.Load("Songs/" + folder + "/" + diff + ".psc"));
        }

        public override void Update(GameTime gameTime)
        {
            if (!active) return;
            if (AudioManager.active && AudioManager.FinishedPlaying())
            {
                EndGameplay();
                return;
            }

            handleInputs();

            // Gameplay commands

            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                EndGameplay();
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                Pause();

            if (Keyboard.GetState().IsKeyDown(Keys.O))
                Resume();

            bool atLeastOne = false;
            // Update UI and objects positions
            for (int i = 0; i < keys; i++)
            {
                bool updatedAll = false;
                for (int k = 0; k < columns[i].updateHitObjects.Count && !updatedAll; k++)
                {
                    if (columns[i].updateHitObjects[k].Value.erase)
                    {
                        columns[i].updateHitObjects.RemoveAt(k);
                        continue;
                    }

                    columns[i].updateHitObjects[k].Value.recalcPos((int) getElapsed(), currentSpeedMultiplier, currentCrosshairRadius);
                    atLeastOne = true;

                    if (columns[i].updateHitObjects[k].Key - msIgnore > getElapsed())
                    {
                        updatedAll = true;
                    }

                    if (columns[i].updateHitObjects[k].Value.time + Judgement.getMiss().judge * rate < getElapsed())
                    {
                        columns[i].hitObjects.Remove(columns[i].updateHitObjects[k].Value);
                        columns[i].updateHitObjects.RemoveAt(k);
                        k--;
                        combo = 0;
                        JudgementValue miss = Judgement.getMiss();

                        KeyValuePair<long, int> hitResult = Scoring.processHitResults(miss, score, combo_multiplier);
                        score = hitResult.Key;
                        combo_multiplier = hitResult.Value;
                        getGameplayView().addJudge(getElapsed(), miss.score);
                        judgements.Add(miss);
                    }
                }
            }

            updateScoreDisplay();
            View.Update(gameTime);

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

        private void updateScoreDisplay()
        {
            score_display = (int) (score / (float) max_score * Scoring.max_score);
        }

        public void deltaTime(long delta)
        {
            AudioManager.deltaTime(delta);
        }

        public void Pause()
        {
            AudioManager.Pause();
        }

        public void Resume()
        {
            AudioManager.Resume();
        }

        public void Reset()
        {
            active = false;
            AudioManager.Stop();
            KeyboardInputManager.Reset();

            currentBeatmap = null;
            columns = null;

            userSpeed = 1;
            currentSpeedMultiplier = 1;
            currentArcsSpeed = 1;
        }

        public void handleInputs()
        {
            while (KeyboardInputManager.keyboardPresses.Count > 0 && KeyboardInputManager.keyboardPresses.Peek().Key <= AudioManager.getTime())
            {
                KeyValuePair<long, Keys> press = KeyboardInputManager.keyboardPresses.Dequeue();
                HitObject pressed = null;
                var column = -1;

                switch (press.Value)
                {
                    case Keys.D:
                        column = 2;
                        break;
                    case Keys.F:
                        column = 3;
                        break;
                    case Keys.J:
                        column = 1;
                        break;
                    case Keys.K:
                        column = 0;
                        break;
                    default:
                        break;
                }

                if (column >= 0 && columns[column].hitObjects.Count > 0)
                {
                    pressed = columns[column].hitObjects[0];

                    int error = (int)((pressed.time - press.Key) / rate);

                   JudgementValue judge = Judgement.getErrorJudgementValue(Math.Abs(error));

                    if (judge != null)
                    {
                        getGameplayView().addHit(press.Key, error, judge.score);

                        columns[column].hitObjects[0].erase = true;
                        columns[column].hitObjects.RemoveAt(0);
                        errors.Add(new KeyValuePair<long, int>(press.Key, error));
                        judgements.Add(judge);

                        KeyValuePair<long, int> hitResult = Scoring.processHitResults(judge, score, combo_multiplier);
                        score = hitResult.Key;
                        combo_multiplier = hitResult.Value;

                        if (judge.score > 0)
                        {
                            combo++;
                        }
                        else
                        {
                            combo = 0;
                        }
                    }
                }
            }
        }

        public void EndGameplay()
        {
            ResultScreen next = new ResultScreen(judgements, errors, score_display);
            Reset();
            ScreenManager.RemoveScreen(true);
            ScreenManager.AddScreen(next);
        }

        public long getElapsed()
        {
            return AudioManager.getTime() + timeOffset;
        }
    }
}
