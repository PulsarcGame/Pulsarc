using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Wobble.Input;

namespace Pulsarc.Utils
{
    static class InputManager
    {
        static Thread inputThread;
        public static Queue<KeyValuePair<double, Keys>> keyboardPresses;
        public static Queue<KeyValuePair<double, Keys>> keyboardReleases;
        public static List<Keys> pressedKeys;
        public static KeyboardState keyboardState;
        public static MouseState mouseState;
        public static KeyVal<MouseState, int> lastMouseClick;

        static public void StartThread()
        {
            pressedKeys = new List<Keys>();
            keyboardPresses = new Queue<KeyValuePair<double, Keys>>();
            keyboardReleases = new Queue<KeyValuePair<double, Keys>>();
            lastMouseClick = new KeyVal<MouseState, int>(Mouse.GetState(),0);

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
                        MouseManager.Update();
                        KeyboardManager.Update();

                        keyboardState = Keyboard.GetState();
                        mouseState = Mouse.GetState();

                        if (keyboardState.GetPressedKeys().Count() > 0)
                        {
                            foreach (Keys key in keyboardState.GetPressedKeys())
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

                            if (!keyboardState.IsKeyDown(key))
                            {
                                // Used if LN handling
                                //keyboardReleases.Enqueue(new KeyValuePair<double, Keys>(time, key));
                                pressedKeys.RemoveAt(i);
                                i--;
                            }
                        }

                        if(lastMouseClick.Value == 0 && mouseState.LeftButton == ButtonState.Pressed)
                        {
                            lastMouseClick.Key = mouseState;
                            lastMouseClick.Value = 1;
                        } else if (lastMouseClick.Value == 1 && mouseState.LeftButton == ButtonState.Released)
                        {
                            lastMouseClick.Key = mouseState;
                            lastMouseClick.Value = 2;
                        }
                    } catch
                    {
                    }
                }
            }
        }

        static public bool isLeftClick()
        {
            if(lastMouseClick.Value == 2)
            {
                // Consome click if we send it
                lastMouseClick.Value = 0;
                return true;
            }
            return false;
        }

        static public void Reset()
        {
            keyboardPresses.Clear();
        }
    }
}
