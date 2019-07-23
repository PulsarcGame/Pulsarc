using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Pulsarc.Utils
{
    static class AudioManager
    {
        static public bool active = false;
        static public Song song = null;
        static public long time;
        static public long offset = 140;

        static Thread audioThread;

        static Stopwatch audioThreadExecTime;

        static public void Start()
        {
            audioThread = new Thread(new ThreadStart(AudioPlayer));
            audioThread.Start();
        }

        static public void AudioPlayer()
        {
            if(song == null)
            {
                return;
            }
            var running = true;
            var threadLimiterWatch = new Stopwatch();
            audioThreadExecTime = new Stopwatch();

            MediaPlayer.Play(song);
            threadLimiterWatch.Start();
            audioThreadExecTime.Start();

            active = true;
            while (running)
            {
                if (threadLimiterWatch.ElapsedMilliseconds >= 1)
                {
                    threadLimiterWatch.Restart();

                    time = audioThreadExecTime.ElapsedMilliseconds;
                }
            }
        }

        static public long getTime()
        {
            return active ? audioThreadExecTime.ElapsedMilliseconds - offset : 0;
        }

        static public void Pause()
        {
            MediaPlayer.Pause();
            audioThreadExecTime.Stop();
        }

        static public void Resume()
        {
            MediaPlayer.Resume();
            audioThreadExecTime.Start();
        }

        static public void Stop()
        {
            Pause();
            MediaPlayer.Stop();
            Reset();
        }

        static public void Reset()
        {
            active = false;
            song = null;
            try
            {
                audioThread.Abort();
            } catch { }
        }
    }
}
