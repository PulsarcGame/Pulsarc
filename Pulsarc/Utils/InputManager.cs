using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Wobble.Input;

namespace Pulsarc.Utils
{
    static class InputManager
    {
        private static Thread _inputThread;
        public static Queue<KeyValuePair<double, Keys>> KeyboardPresses { get; private set; }
        private static Queue<KeyValuePair<double, Keys>> KeyboardReleases { get; set; }
        private static List<Keys> PressedKeys { get; set; }
        private static KeyboardState KeyboardState { get; set; }
        private static MouseState MouseState { get; set; }
        public static KeyVal<MouseState, int> LastMouseClick { get; private set; }

        private static bool CapsLock { get; set; }
        public static bool Caps { get; private set; }

        public static void StartThread()
        {
            PressedKeys = new List<Keys>();
            KeyboardPresses = new Queue<KeyValuePair<double, Keys>>();
            KeyboardReleases = new Queue<KeyValuePair<double, Keys>>();
            LastMouseClick = new KeyVal<MouseState, int>(Mouse.GetState(),0);

            _inputThread = new Thread(InputUpdater);
            _inputThread.Start();
        }

        private static void InputUpdater()
        {
            const bool running = true;
            var threadLimiterWatch = new Stopwatch();

            threadLimiterWatch.Start();

            while (running)
            {
                Thread.Yield();
                if (threadLimiterWatch.ElapsedMilliseconds < 1) continue;
                threadLimiterWatch.Restart();

                try
                {
                    MouseManager.Update();
                    KeyboardManager.Update();

                    KeyboardState = Keyboard.GetState();
                    MouseState = Mouse.GetState();

                    if (KeyboardState.GetPressedKeys().Any())
                        foreach (Keys key in KeyboardState.GetPressedKeys())
                        {
                            if (PressedKeys.Contains(key)) continue;
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

                    for (int i = 0; i < PressedKeys.Count; i++)
                    {
                        Keys key = PressedKeys[i];

                        if (KeyboardState.IsKeyDown(key)) continue;
                        // Used if LN handling
                        //keyboardReleases.Enqueue(new KeyValuePair<double, Keys>(time, key));
                        PressedKeys.RemoveAt(i);
                        i--;


                        if (key == Keys.LeftShift || key == Keys.RightShift)
                            Caps = CapsLock;
                    }

                    switch (LastMouseClick.Value)
                    {
                        case 0 when MouseState.LeftButton == ButtonState.Pressed:
                            LastMouseClick.Key = MouseState;
                            LastMouseClick.Value = 1;
                            break;
                        case 1 when MouseState.LeftButton == ButtonState.Released:
                            LastMouseClick.Key = MouseState;
                            LastMouseClick.Value = 2;
                            break;
                    }
                }
                catch
                {
                    // not added yet
                }
            }
        }

        public static bool IsLeftClick()
        {
            if (LastMouseClick.Value != 2) return false;
            // Consome click if we send it
            LastMouseClick.Value = 0;
            return true;

        }

        public static void Reset()
        {
            KeyboardPresses.Clear();
        }
    }
}
