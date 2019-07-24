using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Pulsarc.Utils
{
    static class KeyboardInputManager
    {
        static Thread inputThread;
        public static Queue<KeyValuePair<long, Keys>> keyboardPresses;
        public static Queue<KeyValuePair<long, Keys>> keyboardReleases;
        public static List<Keys> pressedKeys;

        static public void StartThread()
        {
            pressedKeys = new List<Keys>();
            keyboardPresses = new Queue<KeyValuePair<long, Keys>>();
            keyboardReleases = new Queue<KeyValuePair<long, Keys>>();

            inputThread = new Thread(new ThreadStart(InputUpdater));
            inputThread.Start();
        }

        static public void InputUpdater()
        {
            var running = true;
            var threadLimiterWatch = new Stopwatch();

            threadLimiterWatch.Start();

            while (running)
            {
                if (threadLimiterWatch.ElapsedMilliseconds >= 1)
                {
                    threadLimiterWatch.Restart();
                    
                    KeyboardState state = Keyboard.GetState();

                    if (state.GetPressedKeys().Count() > 0) {
                        foreach(Keys key in state.GetPressedKeys())
                        {
                            if (!pressedKeys.Contains(key))
                            {
                                keyboardPresses.Enqueue(new KeyValuePair<long, Keys>(AudioManager.getTime(), key));
                                pressedKeys.Add(key);
                            }
                        }
                    }

                    for (int i = 0; i < pressedKeys.Count; i++)
                    {
                        Keys key = pressedKeys[i];

                        if (!state.IsKeyDown(key))
                        {
                            // Used if LN handling
                            //keyboardReleases.Enqueue(new KeyValuePair<double, Keys>(time, key));
                            pressedKeys.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        static public void Reset()
        {
            keyboardPresses.Clear();
        }
    }
}
