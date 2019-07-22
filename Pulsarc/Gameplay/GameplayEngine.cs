using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pulsarc.Beatmaps;
using Pulsarc.Gameplay.UI;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pulsarc.Gameplay
{
    public class GameplayEngine
    {
        bool active = false;

        // UI Elements

        Accuracy accuracyDisplay;
        Score scoreDisplay;
        Combo comboDisplay;
        JudgeBox judgeBox;

        // Beatmap Elements

        Beatmap currentBeatmap;
        Column[] columns;
        Crosshair crosshair;
        int keys;


        // Gameplay Elements

        long timeOffset;
        int currentCrosshairRadius;
        double userSpeed;
        double currentSpeedMultiplier;
        double currentArcsSpeed;
        List<Double> errors;

        Int64 score;
        int combo;

        public void Init(Beatmap playedBeatmap)
        {
            Reset();

            // Initialize UI

            scoreDisplay = new Score(new Vector2(Pulsarc.getDimensions().X / 2, 20), centered: true);
            accuracyDisplay = new Accuracy(new Vector2(Pulsarc.getDimensions().X / 2, 50), centered: true);
            comboDisplay = new Combo(new Vector2(Pulsarc.getDimensions().X / 2, 80), centered: true);

            judgeBox = new JudgeBox(new Vector2(Pulsarc.getDimensions().X / 2, Pulsarc.getDimensions().Y / 2));

            // Initialize default variables, parse beatmap
            keys = 4;
            userSpeed = 1;
            currentCrosshairRadius = 200;
            timeOffset = 0;

            currentSpeedMultiplier = 1;
            currentArcsSpeed = userSpeed;

            // Initialize Gameplay variables
            columns = new Column[keys];
            crosshair = new Crosshair(currentCrosshairRadius);
            errors = new List<double>();

            combo = 0;
            score = 0;

            currentBeatmap = playedBeatmap;
            AudioManager.song = Song.FromUri(currentBeatmap.Audio, new Uri(Directory.GetParent(currentBeatmap.path).FullName + "/" + currentBeatmap.Audio, UriKind.Absolute));

            for (int i = 1; i <= keys; i++)
            {
                columns[i - 1] = new Column(i);
            }

            foreach (Arc arc in currentBeatmap.arcs)
            {
                for (int i = 0; i < keys; i++)
                {
                    // Use bitwise check to know if the column is concerned by this arc event
                    if (((arc.type >> i) & 1) != 0)
                    {
                        columns[i].hitObjects.Add(new HitObject(arc.time, (int)(i / (float)keys * 360), keys, currentArcsSpeed));
                    }
                }
            }

            AudioManager.Start();
            active = true;
        }

        public void Init(string beatmapFolderName, string beatmapVersionName)
        {
            Init(BeatmapHelper.Load("Songs/" + beatmapFolderName + "/" + beatmapVersionName + ".psc"));
        }

        public void Update()
        {
            bool atLeastOne = false;
            // Update UI and objects positions
            for (int i = 0; i < keys; i++)
            {
                for(int k = 0; k < columns[i].hitObjects.Count; k++)
                {
                    columns[i].hitObjects[k].recalcPos((int) getElapsed(), currentSpeedMultiplier, currentCrosshairRadius);
                    atLeastOne = true;

                    if(columns[i].hitObjects[k].time + 100 < getElapsed())
                    {
                        columns[i].hitObjects.RemoveAt(k);
                        k--;
                        combo = 0;
                        score += Judgement.getMiss().score;
                        errors.Add(Judgement.getMiss().acc);
                    }
                }
            }

            double accuracyTotal = 0;
            foreach(double error in errors)
            {
                accuracyTotal += Math.Abs(error);
            }

            accuracyDisplay.Update(errors.Count > 0 ? accuracyTotal / errors.Count : 1);
            scoreDisplay.Update(score);
            comboDisplay.Update(combo);
            judgeBox.Update(getElapsed());

            if (!atLeastOne)
            {
                Reset();
            }
        }

        public void handleInputs()
        {
            while (KeyboardInputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = KeyboardInputManager.keyboardPresses.Dequeue();
                HitObject pressed = null;
                var column = -1;

                switch(press.Value)
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

                if (column >=0 && columns[column].hitObjects.Count > 0)
                {
                    pressed = columns[column].hitObjects[0];

                    int error = (int) (pressed.time - press.Key);

                    KeyValuePair<double, int> judge = Judgement.getErrorJudgement(Math.Abs(error));

                    if (judge.Value >= 0)
                    {
                        columns[column].hitObjects.RemoveAt(0);
                        errors.Add(judge.Key);
                        judgeBox.Add((long) press.Key, judge.Value);


                        if (judge.Value > 0)
                        {
                            combo++;
                            score += judge.Value * combo;
                        } else
                        {
                            combo = 0;
                            score += judge.Value;
                        }
                    }
                }
            }
        }

        public long getElapsed()
        {
            return AudioManager.getTime() + timeOffset;
        }

        public void Draw()
        {
            // Draw everything
            crosshair.Draw();

            for (int i = 0; i < keys; i++)
            {
                foreach (HitObject hitObject in columns[i].hitObjects)
                {
                    if (hitObject.IsSeen())
                    {
                        hitObject.Draw();
                    }
                }
            }

            accuracyDisplay.Draw();
            scoreDisplay.Draw();
            comboDisplay.Draw();
            judgeBox.Draw();
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
            AudioManager.Reset();
            active = false;

            currentBeatmap = null;
            columns = null;
            crosshair = null;

            userSpeed = 1;
            currentSpeedMultiplier = 1;
            currentArcsSpeed = 1;
        }

        public bool isActive()
        {
            return active;
        }

        public void deltaTime(long delta)
        {
            timeOffset += delta;
        }
    }
}
