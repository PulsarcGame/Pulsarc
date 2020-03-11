using Pulsarc.UI.Screens.Editor;
using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Diagnostics;
using System.Threading;
using Wobble.Audio;
using Wobble.Audio.Tracks;
using Wobble.Logging;

namespace Pulsarc.Utils.Audio
{
    static class AudioManager
    {
        public static bool Running = false;
        public static bool Active = false;
        public static bool Paused = false;
        public static string SongPath = "";
        public static double Offset = 0;

        private static bool activeThreadLimiterWatch = false;
        private static double startDelayMs = 1000;

        // The current speed that the audio is playing at.
        private static float _audioRate;
        public static float AudioRate
        {
            // If song hasn't been initialized yet, default to 1
            // Otherwise get the song's rate.
            get => song == null ? _audioRate : song.Rate;
            // When we change AudioRate we'll use the SetRate method.
            set
            {
                // Don't set Rate below 0 or above 10
                _audioRate = value < 0 ? 0
                           : value > 10 ? 10
                           : value;
                UpdateRate();
            }
        }

        private static Thread audioThread;
        private static AudioTrack song;
        private static Stopwatch threadLimiterWatch;

        /// <summary>
        /// Start playing audio
        /// </summary>
        public static void StartLazyPlayer()
        {
            Stop();

            if (SongPath == "") { return; }

            // Initialize the song
            try
            {
                song = new AudioTrack(SongPath)
                {
                    Rate = AudioRate,
                    Volume = Config.GetInt("Audio", "MusicVolume"),
                };
            }
            catch (AudioEngineException)
            {
                PulsarcLogger.Error(ManagedBass.Bass.LastError.ToString(), LogType.Runtime);
            }

            song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            song.Play();
            Active = true;
        }

        /// <summary>
        /// Start playing gameplay audio on a new thread
        /// </summary>
        public static void StartAudioPlayer(double withDelay = 100)
        {
            // Minimum delay of 100 ms
            startDelayMs = withDelay < 100 ? 100 : withDelay;

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
            activeThreadLimiterWatch = true;

            if (SongPath == "") { return; }

            Running = true;
            var threadTime = new Stopwatch();

            // Initialize the song
            song = new AudioTrack(SongPath)
            {
                Rate = AudioRate,
                Volume = Config.GetInt("Audio", "MusicVolume"),
            };

            song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            threadLimiterWatch.Start();

            // Add a delay
            while (threadLimiterWatch.ElapsedMilliseconds * AudioRate
                < startDelayMs - Offset) { }

            // Start playing if an engine is active
            if (GameplayEngine.Active || Editor.Active)
            {
                threadLimiterWatch.Restart();

                song.Play();
                threadTime.Start();

                Active = true;
            }
            else
            {
                PulsarcLogger.Warning("Holy shit, your computer is slow, " +
                    "how the fuck did you not load this fast enough?" +
                    "Your computer has at least a whole tenth of a fuckin' second to load the audio " +
                    "but it couldn't even do that. You should really consider upgrading your " +
                    "PC if you see this message...");
            }
        }

        /// <summary>
        /// Get the current time of the audio playing
        /// </summary>
        /// <returns>The current time of the audio (in ms)</returns>
        public static double GetTime()
        {
            return (Active && song.StreamLoaded)
                ? song.Position - Offset
                : -startDelayMs
                    + (activeThreadLimiterWatch 
                    ? threadLimiterWatch.ElapsedMilliseconds * AudioRate : 0);
        }

        /// <summary>
        /// Move the audio position forward or backward by the time
        /// provided
        /// </summary>
        /// <param name="time">The amount of time to move</param>
        public static void DeltaTime(double time)
        {
            if (Active)
            {
                Seek(song.Position + time);
            }
        }

        /// <summary>
        /// Move the audio position to the provided time.
        /// </summary>
        /// <param name="time">The time to jump to in the audio.</param>
        public static void Seek(double time)
        {
            bool withinRange = time >= -1 && time <= song.Length;

            // If audio is active and the time was within range,
            // seek to that time
            if (Active && withinRange)
            {
                song.Seek(time);
            }
            // Otherwise seek to the beginning or end.
            else if (Active)
            {
                if (time < -1)
                {
                    song.Seek(-1);
                }
                else
                {
                    song.Seek(song.Length);
                }
            }
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
                {
                    song.Stop();
                }

                song.Dispose();
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

        /// <summary>
        /// Change the the audio rate to the one defined
        /// in the config.
        /// </summary>
        public static void UpdateRate()
        {
            if (Active)
            {
                // Save the position and audio path.
                double time = GetTime();
                string audio = SongPath;
                Stop();

                // Find the audio rate.
                SongPath = audio;
                AudioRate = Config.GetFloat("Gameplay", "SongRate");
                StartLazyPlayer();

                // Play the rate-changed song at the time saved earlier.
                if (time != 0)
                    DeltaTime((long)time);

                PulsarcLogger.Important($"Now Playing: {SongPath} at {AudioRate} rate", LogType.Runtime);
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
