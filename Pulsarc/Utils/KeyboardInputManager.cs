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
        public static Queue<KeyValuePair<double, Keys>> keyboardPresses;
        public static Queue<KeyValuePair<double, Keys>> keyboardReleases;
        public static List<Keys> pressedKeys;
        public static KeyboardState state;

        static public void StartThread()
        {
            pressedKeys = new List<Keys>();
            keyboardPresses = new Queue<KeyValuePair<double, Keys>>();
            keyboardReleases = new Queue<KeyValuePair<double, Keys>>();

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

                    try
                    {
                        state = Keyboard.GetState();

                        if (state.GetPressedKeys().Count() > 0)
                        {
                            foreach (Keys key in state.GetPressedKeys())
                            {
                                if (!pressedKeys.Contains(key))
                                {
                                    keyboardPresses.Enqueue(new KeyValuePair<double, Keys>(AudioManager.getTime(), key));
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
                    } catch
                    {
                        Console.WriteLine("monogame missed a keystroke, sorry not sorry");
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
