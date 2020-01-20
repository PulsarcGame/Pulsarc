using System.Diagnostics;
using System.Threading;
using Pulsarc.UI.Screens.Gameplay;
using Wobble.Audio.Tracks;
using Wobble.Logging;

namespace Pulsarc.Utils
{
    static class AudioManager
    {
        public static bool Running;
        private static bool _initialized;
        public static bool Active;
        private static bool _activeThreadLimiterWatch;
        public static bool Paused;
        public static string SongPath = "";
        public static double Offset = 35;
        private static double startDelayMs = 1000;
        public static float AudioRate = 1;

        private static Thread _audioThread;
        private static AudioTrack _song;
        private static Stopwatch _threadLimiterWatch;

        /// <summary>
        /// Start playing audio
        /// </summary>
        public static void StartLazyPlayer()
        {
            Stop();

            // Keep trying to initialize the Audio Manager
            if (!_initialized)
                while (!_initialized)
                {
                    try
                    {
                        Wobble.Audio.AudioManager.Initialize(null, null);
                        _initialized = true;
                    }
                    catch
                    {
                        PulsarcLogger.Warning("BASS failed to initialize, retrying...", LogType.Runtime);
                    }
                }

            if (SongPath == "")
                return;

            // Initialize the song
            _song = new AudioTrack(SongPath)
            {
                Rate = AudioRate,
                Volume = Config.GetInt("Audio", "MusicVolume")
            };

            _song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            _song.Play();
            Active = true;
        }

        /// <summary>
        /// Start playing gameplay audio on a new thread
        /// </summary>
        public static void StartGamePlayer()
        {
            _audioThread = new Thread(GameAudioPlayer);
            _audioThread.Start();
        }

        /// <summary>
        /// Initializes the GameAudioPlayer
        /// </summary>
        private static void GameAudioPlayer()
        {
            // Initialize variables
            _threadLimiterWatch = new Stopwatch();
            _threadLimiterWatch.Start();
            _activeThreadLimiterWatch = true;

            if (SongPath == "")
                return;

            // Keep trying to initialize the Audio Manager
            while (!_initialized)
            {
                try
                {
                    Wobble.Audio.AudioManager.Initialize(null, null);
                    _initialized = true;
                }
                catch
                {
                    PulsarcLogger.Warning("BASS failed to initialize, retrying...", LogType.Runtime);
                }
            }

            Running = true;
            var threadTime = new Stopwatch();

            // Initialize the song
            _song = new AudioTrack(SongPath)
            {
                Rate = AudioRate,
                Volume = Config.GetInt("Audio", "MusicVolume")
            };

            _song.ApplyRate(Config.GetBool("Audio", "RatePitch"));

            _threadLimiterWatch.Start();

            // Add a delay
            while (_threadLimiterWatch.ElapsedMilliseconds < startDelayMs - Offset)
            { }

            // Start playing if the gameplay engine is active
            if (!GameplayEngine.Active) return;
            _threadLimiterWatch.Restart();

            _song.Play();
            threadTime.Start();

            Active = true;

            while (Running)
                if (_threadLimiterWatch.ElapsedMilliseconds >= 1)
                {
                    _threadLimiterWatch.Restart();
                    Wobble.Audio.AudioManager.Update(threadTime.ElapsedMilliseconds);
                }
        }

        /// <summary>
        /// Get the current time of the audio playing
        /// </summary>
        /// <returns>The current time of the audio (in ms)</returns>
        public static double GetTime()
        {
            if (Active && _song.StreamLoaded)
                return _song.Position - Offset;
            return -startDelayMs + (_activeThreadLimiterWatch ? _threadLimiterWatch.ElapsedMilliseconds : 0);
        }

        /// <summary>
        /// Move the audio position forward or backward by the time
        /// provided
        /// </summary>
        /// <param name="time">The amount of time to move</param>
        public static void DeltaTime(long time)
        {
            if (Active)
                Seek(_song.Position + time);
        }

        /// <summary>
        /// Move the audio position to the provided time.
        /// </summary>
        /// <param name="time">The time to jump to in the audio.</param>
        private static void Seek(double time)
        {
            bool withinRange = time >= -1 && time <= _song.Length;

            // If audio is active and the time was within range,
            // seek to that time
            if (Active && withinRange)
                _song.Seek(time);
            // Otherwise seek to the beginning or end.
            else if (Active)
                if (time < -1)
                    _song.Seek(-1);
                else
                    _song.Seek(_song.Length);
        }

        /// <summary>
        /// Pause the audio
        /// </summary>
        public static void Pause()
        {
            if (!Active || Paused || !_song.IsPlaying) return;
            _song.Pause();
            Paused = true;
        }

        /// <summary>
        /// Resume the audio
        /// </summary>
        public static void Resume()
        {
            if (!Active || !Paused || _song.IsPlaying) return;
            _song.Play();
            Paused = false;
        }

        /// <summary>
        /// Stop the audio
        /// </summary>
        public static void Stop()
        {
            if (!Active) return;
            Pause();

            if (_song.IsPlaying)
                _song.Stop();

            _song = null;
            Reset();
        }

        /// <summary>
        /// Reset the audio
        /// </summary>
        private static void Reset()
        {
            Active = false;
            Paused = false;
            Running = false;

            _threadLimiterWatch?.Reset();
        }

        /// <summary>
        /// Change the the audio rate to the one defined
        /// in the config.
        /// </summary>
        public static void UpdateRate()
        {
            if (!Active) return;
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

        /// <summary>
        /// Has the song finished playing?
        /// </summary>
        /// <returns>Whether the song has finished (not stopped or paused)</returns>
        public static bool FinishedPlaying()
        {
            return !Paused && !_song.IsPlaying && _song.IsStopped;
        }
    }
}
