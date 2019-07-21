using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pulsarc.Gameplay
{
    public class GameplayEngine
    {
        bool active = false;

        Beatmap currentBeatmap;
        Column[] columns;
        Crosshair crosshair;
        int keys;

        Stopwatch time;
        long timeOffset;
        int currentCrosshairRadius;

        double userSpeed;
        double currentSpeedMultiplier;
        double currentArcsSpeed;

        public void Init(string beatmapFolderName, string beatmapVersionName)
        {
            Reset();

            // Initialize default variables, parse beatmap
            keys = 4;
            userSpeed = 1;
            currentCrosshairRadius = 200;

            currentSpeedMultiplier = 1;
            currentArcsSpeed = userSpeed;

            columns = new Column[keys];
            crosshair = new Crosshair();

            currentBeatmap = BeatmapHelper.Load("Songs/" + beatmapFolderName + "/" + beatmapVersionName + ".psc");

            for(int i = 1; i <= keys; i++)
            {
                columns[i - 1] = new Column(i);
            }

            foreach(Arc arc in currentBeatmap.arcs)
            {
                for(int i = 0; i < keys; i++)
                {
                    // Use bitwise check to know if the column is concerned by this arc event
                    if (((arc.type >> i) & 1) != 0)
                    {
                        columns[i].hitObjects.Add(new HitObject(arc.time, (int) (i / (float) keys * 360), keys, currentArcsSpeed));
                    }
                }
            }

            active = true;
            time = new Stopwatch();
            time.Start();
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
                    }
                }
            }

            if(!atLeastOne)
            {
                Reset();
            }
        }

        public void handleInputs()
        {
            var max = 16;
            var c300 = 25;
            var c200 = 40;
            var c100 = 60;
            var c50 = 200;


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

                    var error = pressed.time - getElapsed();
                    
                    if (!(Math.Abs(error) > c50))
                    {
                        columns[column].hitObjects.RemoveAt(0);
                    }
                }
            }
        }

        public long getElapsed()
        {
            return time.ElapsedMilliseconds + timeOffset;
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
        }

        public void Reset()
        {
            active = false;

            currentBeatmap = null;
            columns = null;
            crosshair = null;
            if (time != null)
            {
                time.Stop();
            }
            time = null;
            timeOffset = 0;

            userSpeed = 1;
            currentSpeedMultiplier = 1;
            currentArcsSpeed = 1;
        }

        public bool isActive()
        {
            return active;
        }

        public void Pause()
        {
            time.Stop();
        }

        public void Continue()
        {
            time.Start();
        }

        public void deltaTime(long delta)
        {
            timeOffset += delta;
        }
    }
}
