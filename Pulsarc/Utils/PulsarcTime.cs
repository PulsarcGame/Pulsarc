using System.Diagnostics;

namespace Pulsarc.Utils
{
    public static class PulsarcTime
    {
        public static bool isRunning = false;

        private static double smoothDeltaTime;
        public static double SmoothDeltaTime { get => smoothDeltaTime; set { } }

        private static double deltaTime;
        public static double DeltaTime { get => deltaTime; set { } }

        private static double prevFrameTime;
        public static double PrevFrameElapsedTime { get => prevFrameTime; set { } }

        public static double CurrentElapsedTime { get => stopwatch.ElapsedMilliseconds; set { } }

        private static Stopwatch stopwatch = new Stopwatch();

        public static void Start()
        {
            stopwatch.Start();

            prevFrameTime = stopwatch.ElapsedMilliseconds;
            smoothDeltaTime = prevFrameTime;
            deltaTime = prevFrameTime;

            isRunning = true;
        }

        public static void Resume()
        {
            stopwatch.Start();
            isRunning = true;
        }

        public static void Stop()
        {
            stopwatch.Stop();
            isRunning = false;
        }

        public static void Reset()
        {
            stopwatch.Reset();

            prevFrameTime = 0;
            smoothDeltaTime = 0;
            deltaTime = 0;

            isRunning = false;
        }

        public static void Restart()
        {
            Reset();
            Start();
        }

        public static void Update()
        {
            if (!isRunning)
            {
                return;
            }

            double currentFrameTime = stopwatch.ElapsedMilliseconds;

            deltaTime = currentFrameTime - prevFrameTime;
            prevFrameTime = currentFrameTime;

            smoothDeltaTime = findSmoothDeltaTime();
        }

        private static double findSmoothDeltaTime()
        {
            double smoothAmount = .8;

            double newSmoothDeltaTime = (1 - smoothAmount) * deltaTime + (smoothAmount * smoothDeltaTime);

            return newSmoothDeltaTime;
        }
    }
}
