using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Diagnostics;
using System.Threading;
using Wobble.Audio.Tracks;

namespace Pulsarc.Utils
{
    static class AudioManager
    {
        public static bool running = false;
        public static bool initialized = false;
        public static bool active = false;
        public static bool activeThreadLimiterWatch = false;
        public static bool paused = false;
        public static string songPath = "";
        public static double offset = 35;
        public static double startDelayMs = 1000;
        public static float audioRate = 1;

        static Thread audioThread;
        static AudioTrack song;
        static Stopwatch threadLimiterWatch;

        public static void Start()
        {
            audioThread = new Thread(new ThreadStart(AudioPlayer));
            audioThread.Start();
        }

        public static void AudioPlayer()
        {
            threadLimiterWatch = new Stopwatch();
            threadLimiterWatch.Start();
            activeThreadLimiterWatch = true;

            if (songPath == "")
            {
                return;
            }
            if (!initialized)
            {
                while (!initialized)
                {
                    try
                    {
                        Wobble.Audio.AudioManager.Initialize(null, null);
                        initialized = true;
                    }
                    catch
                    {
                        Console.WriteLine("BASS failed to initialize, retrying...");
                    }
                }
            }

            running = true;
            var threadTime = new Stopwatch();

            song = new AudioTrack(songPath, false)
            {
                Rate = audioRate,
            };
            song.Volume = Config.GetInt("Audio", "MusicVolume");

            threadLimiterWatch.Start();

            while (threadLimiterWatch.ElapsedMilliseconds < startDelayMs - offset) { }

            if (GameplayEngine.active)
            {
                threadLimiterWatch.Restart();

                song.Play();
                threadTime.Start();

                active = true;
                while (running)
                {
                    if (threadLimiterWatch.ElapsedMilliseconds >= 1)
                    {
                        threadLimiterWatch.Restart();
                        Wobble.Audio.AudioManager.Update(threadTime.ElapsedMilliseconds);
                    }
                }
            }
        }

        public static double getTime()
        {
            if(active && song.StreamLoaded) { 
                return song.Position - offset;
            } else
            {
                return -startDelayMs + (activeThreadLimiterWatch ? threadLimiterWatch.ElapsedMilliseconds : 0);
            }
        }

        public static void deltaTime(long time)
        {
            if(active)
            {
                song.Seek(song.Position + time);
            }
        }

        public static void Pause()
        {
            if (active && !paused && song.IsPlaying)
            {
                song.Pause();
                paused = true;
            }
        }

        public static void Resume()
        {
            if (active && paused && !song.IsPlaying)
            {
                song.Play();
                paused = false;
            }
        }

        public static void Stop()
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

        public static void Reset()
        {
            active = false;
            paused = false;
            running = false;
            songPath = "";
            threadLimiterWatch.Reset();
        }

        public static bool FinishedPlaying()
        {
            return !paused && !song.IsPlaying && song.IsStopped;
        }
    }
}