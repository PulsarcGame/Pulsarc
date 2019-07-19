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
        static double time;
        static Queue<KeyValuePair<Double,KeyboardState>> keyboardStates;

        static public void StartThread()
        {
            keyboardStates = new Queue<KeyValuePair<Double, KeyboardState>>();

            inputThread = new Thread(new ThreadStart(InputUpdater));
            inputThread.Start();
        }

        static public void InputUpdater()
        {
            var running = true;
            var threadLimiterWatch = new Stopwatch();
            var inputThreadExecTime = new Stopwatch();

            threadLimiterWatch.Start();
            inputThreadExecTime.Start();

            while (running)
            {
                if (threadLimiterWatch.ElapsedMilliseconds >= 1)
                {
                    threadLimiterWatch.Restart();

                    time = inputThreadExecTime.ElapsedMilliseconds;
                    KeyboardState state = Keyboard.GetState();

                    if (state.GetPressedKeys().Count() > 0) {
                        keyboardStates.Enqueue(new KeyValuePair<Double, KeyboardState>(time, state));
                    }
                }
            }
        }
    }
}
