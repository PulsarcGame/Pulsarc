using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Diagnostics;
using System.Threading;
using Wobble.Audio.Tracks;

namespace Pulsarc.Utils
{
    static class AudioManager
    {
        static public bool running = false;
        static public bool initialized = false;
        static public bool active = false;
        static public bool activeThreadLimiterWatch = false;
        static public bool paused = false;
        static public string song_path = "";
        static public double offset = 35;
        static public double startDelayMs = 1000;
        static public float audioRate = 1;

        static Thread audioThread;
        static AudioTrack song;
        static Stopwatch threadLimiterWatch;
        static GameTime lastTime;

        static public void Start()
        {
            audioThread = new Thread(new ThreadStart(AudioPlayer));
            audioThread.Start();
        }

        static public void AudioPlayer()
        {
            threadLimiterWatch = new Stopwatch();
            threadLimiterWatch.Start();
            activeThreadLimiterWatch = true;

            if (song_path == "")
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

            song = new AudioTrack(song_path, false)
            {
                Rate = audioRate,
            };
            song.Volume = Config.getInt("Audio", "MusicVolume");

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

        static public double getTime()
        {
            if(active && song.StreamLoaded) { 
                return song.Position - offset;
            } else
            {
                return -startDelayMs + (activeThreadLimiterWatch ? threadLimiterWatch.ElapsedMilliseconds : 0);
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
            threadLimiterWatch.Reset();
        }

        static public bool FinishedPlaying()
        {
            return !paused && !song.IsPlaying && song.IsStopped;
        }
    }
}
