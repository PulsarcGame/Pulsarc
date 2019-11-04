﻿using Microsoft.Xna.Framework.Input;
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
        private static Thread inputThread;
        public static Queue<KeyValuePair<double, Keys>> KeyboardPresses;
        public static Queue<KeyValuePair<double, Keys>> KeyboardReleases;
        public static List<Keys> PressedKeys;
        public static KeyboardState KeyboardState;
        public static MouseState MouseState;
        public static KeyVal<MouseState, int> LastMouseClick;

        public static bool CapsLock = false;
        public static bool Caps = false;

        static public void StartThread()
        {
            PressedKeys = new List<Keys>();
            KeyboardPresses = new Queue<KeyValuePair<double, Keys>>();
            KeyboardReleases = new Queue<KeyValuePair<double, Keys>>();
            LastMouseClick = new KeyVal<MouseState, int>(Mouse.GetState(),0);

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

                        KeyboardState = Keyboard.GetState();
                        MouseState = Mouse.GetState();

                        if (KeyboardState.GetPressedKeys().Count() > 0)
                            foreach (Keys key in KeyboardState.GetPressedKeys())
                            {
                                if (!PressedKeys.Contains(key))
                                {
                                    KeyboardPresses.Enqueue(new KeyValuePair<double, Keys>(AudioManager.GetTime(), key));
                                    PressedKeys.Add(key);

                                    if (key == Keys.CapsLock)
                                    {
                                        CapsLock = !CapsLock;
                                        Caps = CapsLock;
                                    }

                                    if (key == Keys.LeftShift || key == Keys.RightShift)
                                        Caps = !CapsLock;
                                }
                            }

                        for (int i = 0; i < PressedKeys.Count; i++)
                        {
                            Keys key = PressedKeys[i];

                            if (!KeyboardState.IsKeyDown(key))
                            {
                                // Used if LN handling
                                //keyboardReleases.Enqueue(new KeyValuePair<double, Keys>(time, key));
                                PressedKeys.RemoveAt(i);
                                i--;


                                if (key == Keys.LeftShift || key == Keys.RightShift)
                                    Caps = CapsLock;
                            }
                        }

                        if (LastMouseClick.Value == 0 && MouseState.LeftButton == ButtonState.Pressed)
                        {
                            LastMouseClick.Key = MouseState;
                            LastMouseClick.Value = 1;
                        }
                        else if (LastMouseClick.Value == 1 && MouseState.LeftButton == ButtonState.Released)
                        {
                            LastMouseClick.Key = MouseState;
                            LastMouseClick.Value = 2;
                        }
                    }
                    catch { }
                }
            }
        }

        static public bool IsLeftClick()
        {
            if (LastMouseClick.Value == 2)
            {
                // Consome click if we send it
                LastMouseClick.Value = 0;
                return true;
            }

            return false;
        }

        static public void Reset()
        {
            KeyboardPresses.Clear();
        }
    }
}
