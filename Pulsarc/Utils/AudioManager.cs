using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Wobble.Audio.Tracks;

namespace Pulsarc.Utils
{
    static class AudioManager
    {
        static public bool running = false;
        static public bool initialized = false;
        static public bool active = false;
        static public bool paused = false;
        static public string song_path = "";
        static public long offset = 50;
        static public int startDelayMs = 1000;
        static public float audioRate = 1;

        static Thread audioThread;
        static AudioTrack song;
        static Stopwatch threadLimiterWatch;

        static public void Start()
        {
            audioThread = new Thread(new ThreadStart(AudioPlayer));
            audioThread.Start();
        }

        static public void AudioPlayer()
        {
            threadLimiterWatch = new Stopwatch();
            if (song_path == "")
            {
                return;
            }
            if (!initialized)
            {
                Wobble.Audio.AudioManager.Initialize(null, null);
                initialized = true;
            }

            running = true;
            var threadTime = new Stopwatch();


            song = new AudioTrack(song_path, false)
            {
                Rate = audioRate,
            };

            threadLimiterWatch.Start();

            while(threadLimiterWatch.ElapsedMilliseconds < startDelayMs) { }

            if (GameplayEngine.active)
            {
                threadLimiterWatch.Restart();

                song.Play();
                threadTime.Start();

                TimeSpan ts;

                active = true;
                while (running)
                {
                    if (threadLimiterWatch.ElapsedMilliseconds >= 1)
                    {
                        threadLimiterWatch.Restart();

                        ts = new TimeSpan(threadTime.ElapsedMilliseconds);
                        Wobble.Audio.AudioManager.Update(new GameTime(ts, ts));
                    }
                }
            }
        }

        static public long getTime()
        {
            if(active && song.StreamLoaded) { 
                return (long) song.Position - offset;
            } else
            {
                return -startDelayMs + threadLimiterWatch.ElapsedMilliseconds;
            }
        }

        static public void deltaTime(long time)
        {
            if(active)
            {
                song.Seek(song.Position + time);
            }
        }

        static public void Pause()
        {
            if (active && !paused && song.IsPlaying)
            {
                song.Pause();
                paused = true;
            }
        }

        static public void Resume()
        {
            if (active && paused && !song.IsPlaying)
            {
                song.Play();
                paused = false;
            }
        }

        static public void Stop()
        {
            if (active)
            {
                Pause();
                if (song.IsPlaying)
                {
                    song.Stop();
                }
                song = null;
                Reset();
            }
        }

        static public void Reset()
        {
            active = false;
            paused = false;
            running = false;
            song_path = "";
        }

        static public bool FinishedPlaying()
        {
            return !paused && !song.IsPlaying && song.IsStopped;
        }
    }
}
