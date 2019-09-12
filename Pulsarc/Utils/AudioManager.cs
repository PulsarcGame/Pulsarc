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

        /// <summary>
        /// Start playing audio on a new thread
        /// </summary>
        static public void StartGamePlayer()
        {
            audioThread = new Thread(new ThreadStart(GameAudioPlayer));
            audioThread.Start();
        }

        /// <summary>
        /// Initializes the AudioPlayer
        /// </summary>
        static public void GameAudioPlayer()
        {
            // Initialize variables
            threadLimiterWatch = new Stopwatch();
            threadLimiterWatch.Start();
            activeThreadLimiterWatch = true;

            if (song_path == "")
            {
                return;
            }

            // Keep trying to initizlie the Audio Manager
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

            // Initialize the song
            song = new AudioTrack(song_path, false)
            {
                Rate = audioRate,
            };
            song.Volume = Config.getInt("Audio", "MusicVolume");
            song.ApplyRate(Config.getBool("Audio", "RatePitch"));

            threadLimiterWatch.Start();

            // Add a delay
            while (threadLimiterWatch.ElapsedMilliseconds < startDelayMs - offset) { }

            // Start playing if the gameplay engine is active
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

        /// <summary>
        /// Get the current time of the audio playing
        /// </summary>
        /// <returns>The current time of the audio (in ms)</returns>
        static public double getTime()
        {
            if(active && song.StreamLoaded) { 
                return song.Position - offset;
            } else
            {
                return -startDelayMs + (activeThreadLimiterWatch ? threadLimiterWatch.ElapsedMilliseconds : 0);
            }
        }

        /// <summary>
        /// Move the audio position forward or backward in time
        /// </summary>
        /// <param name="time">The amount of time to move</param>
        static public void deltaTime(long time)
        {
            if(active)
            {
                song.Seek(song.Position + time);
            }
        }

        /// <summary>
        /// Pause the audio
        /// </summary>
        static public void Pause()
        {
            if (active && !paused && song.IsPlaying)
            {
                song.Pause();
                paused = true;
            }
        }

        /// <summary>
        /// Resume the audio
        /// </summary>
        static public void Resume()
        {
            if (active && paused && !song.IsPlaying)
            {
                song.Play();
                paused = false;
            }
        }

        /// <summary>
        /// Stop the audio
        /// </summary>
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

        /// <summary>
        /// Reset the audio
        /// </summary>
        static public void Reset()
        {
            active = false;
            paused = false;
            running = false;
            song_path = "";
            threadLimiterWatch.Reset();
        }

        /// <summary>
        /// Has the song finished playing?
        /// </summary>
        /// <returns>Whether the song has finished (not stopped or paused)</returns>
        static public bool FinishedPlaying()
        {
            return !paused && !song.IsPlaying && song.IsStopped;
        }
    }
}
