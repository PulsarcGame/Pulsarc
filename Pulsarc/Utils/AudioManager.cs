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
        public static bool Running = false;
        public static bool Initialized = false;
        public static bool Active = false;
        public static bool ActiveThreadLimiterWatch = false;
        public static bool Paused = false;
        public static string SongPath = "";
        public static double Offset = 35;
        public static double StartDelayMs = 1000;
        public static float AudioRate = 1;

        private static Thread audioThread;
        private static AudioTrack song;
        private static Stopwatch threadLimiterWatch;

        /// <summary>
        /// Start playing audio
        /// </summary>
        public static void StartLazyPlayer()
        {
            Stop();

            // Keep trying to initialize the Audio Manager
            if (!Initialized)
                while (!Initialized)
                {
                    try
                    {
                        Wobble.Audio.AudioManager.Initialize(null, null);
                        Initialized = true;
                    }
                    catch
                    {
                        Console.WriteLine("BASS failed to initialize, retrying...");
                    }
                }

            if (SongPath == "")
                return;

            // Initialize the song
            song = new AudioTrack(SongPath, false)
            {
                Rate = AudioRate,
                Volume = Config.GetInt("Audio", "MusicVolume"),
            };

            song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            song.Play();
            Active = true;
        }

        /// <summary>
        /// Start playing gameplay audio on a new thread
        /// </summary>
        public static void StartGamePlayer()
        {
            audioThread = new Thread(new ThreadStart(GameAudioPlayer));
            audioThread.Start();
        }

        /// <summary>
        /// Initializes the GameAudioPlayer
        /// </summary>
        public static void GameAudioPlayer()
        {
            // Initialize variables
            threadLimiterWatch = new Stopwatch();
            threadLimiterWatch.Start();
            ActiveThreadLimiterWatch = true;

            if (SongPath == "")
                return;

            // Keep trying to initialize the Audio Manager
            if (!Initialized)
                while (!Initialized)
                {
                    try
                    {
                        Wobble.Audio.AudioManager.Initialize(null, null);
                        Initialized = true;
                    }
                    catch
                    {
                        Console.WriteLine("BASS failed to initialize, retrying...");
                    }
                }

            Running = true;
            var threadTime = new Stopwatch();

            // Initialize the song
            song = new AudioTrack(SongPath, false)
            {
                Rate = AudioRate,
                Volume = Config.GetInt("Audio", "MusicVolume"),
            };

            song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            threadLimiterWatch.Start();

            // Add a delay
            while (threadLimiterWatch.ElapsedMilliseconds < StartDelayMs - Offset) { }

            // Start playing if the gameplay engine is active
            if (GameplayEngine.Active)
            {
                threadLimiterWatch.Restart();

                song.Play();
                threadTime.Start();

                Active = true;

                while (Running)
                    if (threadLimiterWatch.ElapsedMilliseconds >= 1)
                    {
                        threadLimiterWatch.Restart();
                        Wobble.Audio.AudioManager.Update(threadTime.ElapsedMilliseconds);
                    }
            }
        }

        /// <summary>
        /// Get the current time of the audio playing
        /// </summary>
        /// <returns>The current time of the audio (in ms)</returns>
        public static double GetTime()
        {
            if (Active && song.StreamLoaded)
                return song.Position - Offset;
            else
                return -StartDelayMs + (ActiveThreadLimiterWatch ? threadLimiterWatch.ElapsedMilliseconds : 0);
        }

        /// <summary>
        /// Move the audio position forward or backward in time
        /// </summary>
        /// <param name="time">The amount of time to move</param>
        public static void DeltaTime(long time)
        {
            if (Active)
                song.Seek(song.Position + time);
        }

        /// <summary>
        /// Pause the audio
        /// </summary>
        public static void Pause()
        {
            if (Active && !Paused && song.IsPlaying)
            {
                song.Pause();
                Paused = true;
            }
        }

        /// <summary>
        /// Resume the audio
        /// </summary>
        public static void Resume()
        {
            if (Active && Paused && !song.IsPlaying)
            {
                song.Play();
                Paused = false;
            }
        }

        /// <summary>
        /// Stop the audio
        /// </summary>
        public static void Stop()
        {
            if (Active)
            {
                Pause();

                if (song.IsPlaying)
                    song.Stop();

                song = null;
                Reset();
            }
        }

        /// <summary>
        /// Reset the audio
        /// </summary>
        public static void Reset()
        {
            Active = false;
            Paused = false;
            Running = false;

            if (threadLimiterWatch != null)
                threadLimiterWatch.Reset();
        }

        public static void updateRate()
        {
            if (Active)
            {
                double time = GetTime();
                string audio = SongPath;
                Stop();

                SongPath = audio;
                AudioRate = Config.GetFloat("Gameplay", "SongRate");
                StartLazyPlayer();

                if (time != 0)
                    DeltaTime((long)time);

                Console.WriteLine($"Now Playing: {SongPath} at {AudioRate} rate");
            }
        }

        /// <summary>
        /// Has the song finished playing?
        /// </summary>
        /// <returns>Whether the song has finished (not stopped or paused)</returns>
        public static bool FinishedPlaying()
        {
            return !Paused && !song.IsPlaying && song.IsStopped;
        }
    }
}
