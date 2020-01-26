using System.Diagnostics;

namespace Pulsarc.Utils
{
    // A static class that keeps track of time.
    public static class PulsarcTime
    {
        // Whether or not this Class is keeping track of time.
        public static bool IsRunning { get; private set; } = false;
        
        // Delta Time (in ms) that is smoothed in a similar fashion to Unity's Time.SmoothDeltaTime
        public static double SmoothDeltaTime { get; private set; }

        // The amount of time (in ms) between the last two frames
        public static double DeltaTime { get; private set; }

        // <summary> The amount of time (in ms) since this timer started, as of last frame. </summary>
        public static double PrevFrameElapsedTime { get; private set; }

        /// <summary> The amount of time (in ms) since this timer was started. </summary>
        public static double CurrentElapsedTime => stopwatch.ElapsedMilliseconds;

        // The stopwatch doing most of the work for us.
        private static Stopwatch stopwatch = new Stopwatch();

        public static void Start()
        {
            stopwatch.Start();

            PrevFrameElapsedTime = stopwatch.ElapsedMilliseconds;
            SmoothDeltaTime = PrevFrameElapsedTime;
            DeltaTime = PrevFrameElapsedTime;

            IsRunning = true;
        }

        public static void Resume()
        {
            stopwatch.Start();
            IsRunning = true;
        }

        public static void Stop()
        {
            stopwatch.Stop();
            IsRunning = false;
        }

        public static void Reset()
        {
            stopwatch.Reset();

            PrevFrameElapsedTime = 0;
            SmoothDeltaTime = 0;
            DeltaTime = 0;

            IsRunning = false;
        }

        public static void Restart()
        {
            Reset();
            Start();
        }

        /// <summary>
        /// This method must be called every frame for accurate
        /// SmoothDeltaTime values.
        /// </summary>
        public static void Update()
        {
            if (!IsRunning)
                return;

            double currentFrameTime = stopwatch.ElapsedMilliseconds;

            DeltaTime = currentFrameTime - PrevFrameElapsedTime;
            PrevFrameElapsedTime = currentFrameTime;

            SmoothDeltaTime = FindSmoothDeltaTime();
        }

        /// <summary>
        /// Replicates how Unity calculates Smooth Delta Time.
        /// Found this equation in some random Unity forum after
        /// 2 hours of internet searching <_<
        /// </summary>
        /// <returns>The SmoothDeltaTime for the last frame</returns>
        private static double FindSmoothDeltaTime()
        {
            double smoothAmount = .8;

            double newSmoothDeltaTime = (1 - smoothAmount) * DeltaTime + (smoothAmount * SmoothDeltaTime);

            return newSmoothDeltaTime;
        }
    }
}
