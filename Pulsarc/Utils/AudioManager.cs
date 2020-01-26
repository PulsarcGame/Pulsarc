using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Diagnostics;
using System.Threading;
using Wobble.Audio;
using Wobble.Audio.Tracks;
using Wobble.Logging;

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

        private static Thread audioThread;
        private static AudioTrack song;
        private static Stopwatch threadLimiterWatch;

        /// <summary>
        /// Start playing audio
        /// </summary>
        public static void StartLazyPlayer()
        {
            Stop();

            if (songPath == "")
                return;

            // Initialize the song
            try
            {
                song = new AudioTrack(songPath)
                {
                    Rate = audioRate,
                    Volume = Config.GetInt("Audio", "MusicVolume"),
                };
            }
            catch (AudioEngineException)
            {
                PulsarcLogger.Debug(ManagedBass.Bass.LastError.ToString(), LogType.Runtime);
            }

            song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            song.Play();
            active = true;
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
            activeThreadLimiterWatch = true;

            if (songPath == "")
                return;

            running = true;
            var threadTime = new Stopwatch();

            // Initialize the song
            song = new AudioTrack(songPath, false)
            {
                Rate = audioRate,
                Volume = Config.GetInt("Audio", "MusicVolume"),
            };

            song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            threadLimiterWatch.Start();

            // Add a delay
            while (threadLimiterWatch.ElapsedMilliseconds < startDelayMs - offset)
            { }

            // Start playing if the gameplay engine is active
            if (GameplayEngine.Active)
            {
                threadLimiterWatch.Restart();

                song.Play();
                threadTime.Start();

                active = true;
            }
        }

        /// <summary>
        /// Get the current time of the audio playing
        /// </summary>
        /// <returns>The current time of the audio (in ms)</returns>
        public static double GetTime()
        {
            if (active && song.StreamLoaded)
                return song.Position - offset;
            else
                return -startDelayMs + (activeThreadLimiterWatch ? threadLimiterWatch.ElapsedMilliseconds : 0);
        }

        /// <summary>
        /// Move the audio position forward or backward by the time
        /// provided
        /// </summary>
        /// <param name="time">The amount of time to move</param>
        public static void DeltaTime(long time)
        {
            if (active)
                Seek(song.Position + time);
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
            if (active && withinRange)
                song.Seek(time);
            // Otherwise seek to the beginning or end.
            else if (active)
                if (time < -1)
                    song.Seek(-1);
                else
                    song.Seek(song.Length);
        }

        /// <summary>
        /// Pause the audio
        /// </summary>
        public static void Pause()
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
        public static void Resume()
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
        public static void Stop()
        {
            if (active)
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
            active = false;
            paused = false;
            running = false;

            if (threadLimiterWatch != null)
                threadLimiterWatch.Reset();
        }

        /// <summary>
        /// Change the the audio rate to the one defined
        /// in the config.
        /// </summary>
        public static void UpdateRate()
        {
            if (active)
            {
                // Save the position and audio path.
                double time = GetTime();
                string audio = songPath;
                Stop();

                // Find the audio rate.
                songPath = audio;
                audioRate = Config.GetFloat("Gameplay", "SongRate");
                StartLazyPlayer();

                // Play the rate-changed song at the time saved earlier.
                if (time != 0)
                    DeltaTime((long)time);

                PulsarcLogger.Important($"Now Playing: {songPath} at {audioRate} rate", LogType.Runtime);
            }
        }

        /// <summary>
        /// Has the song finished playing?
        /// </summary>
        /// <returns>Whether the song has finished (not stopped or paused)</returns>
        public static bool FinishedPlaying()
        {
            return !paused && !song.IsPlaying && song.IsStopped;
        }
    }
}
