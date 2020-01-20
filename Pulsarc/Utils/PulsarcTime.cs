using System.Diagnostics;

namespace Pulsarc.Utils
{
    // A static class that keeps track of time.
    public static class PulsarcTime
    {
        // Whether or not this Class is keeping track of time.
        private static bool IsRunning { get; set; }
        
        // Delta Time (in ms) that is smoothed in a similar fashion to Unity's Time.SmoothDeltaTime
        public static double SmoothDeltaTime { get; private set; }

        // The amount of time (in ms) between the last two frames
        public static double DeltaTime { get; private set; }

        // The amount of time (in ms) since this timer started, as of last frame.
        private static double PrevFrameElapsedTime { get; set; }

        // The amount of time (in ms) since this timer was started.
        public static double CurrentElapsedTime => Stopwatch.ElapsedMilliseconds;

        // The stopwatch doing most of the work for us.
        private static readonly Stopwatch Stopwatch = new Stopwatch();

        public static void Start()
        {
            Stopwatch.Start();

            PrevFrameElapsedTime = Stopwatch.ElapsedMilliseconds;
            SmoothDeltaTime = PrevFrameElapsedTime;
            DeltaTime = PrevFrameElapsedTime;

            IsRunning = true;
        }

        public static void Resume()
        {
            Stopwatch.Start();
            IsRunning = true;
        }

        public static void Stop()
        {
            Stopwatch.Stop();
            IsRunning = false;
        }

        private static void Reset()
        {
            Stopwatch.Reset();

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

            double currentFrameTime = Stopwatch.ElapsedMilliseconds;

            DeltaTime = currentFrameTime - PrevFrameElapsedTime;
            PrevFrameElapsedTime = currentFrameTime;

            SmoothDeltaTime = FindSmoothDeltaTime();
        }

        /// <summary>
        /// Replicates how Unity calculates Smooth Delta Time.
        /// Found this equation in some random Unity forum after
        /// 2 hours of internet searching 
        /// </summary>
        /// <returns>The SmoothDeltaTime for the last frame</returns>
        private static double FindSmoothDeltaTime()
        {
            const double smoothAmount = .8;

            double newSmoothDeltaTime = (1 - smoothAmount) * DeltaTime + smoothAmount * SmoothDeltaTime;

            return newSmoothDeltaTime;
        }
    }
}
