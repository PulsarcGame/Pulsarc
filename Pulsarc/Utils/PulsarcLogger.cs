using Wobble.Logging;

namespace Pulsarc.Utils
{
    public static class PulsarcLogger
    {
        public static bool Logging = true;

        public static void Initialize()
        {
            Logger.Initialize();
        }

        public static string GetLogPath(LogType type = LogType.Runtime)
        {
            if (Logging)
                return GetLogPath(type);

            return "";
        }

        public static void Debug(string value, LogType type = LogType.Runtime, bool writeToFile = true)
        {
            if (Logging)
                Logger.Debug(value, type, writeToFile);
        }

        public static void Important(string value, LogType type = LogType.Runtime, bool writeToFile = true)
        {
            if (Logging)
                Logger.Important(value, type, writeToFile);
        }

        public static void Warning(string value, LogType type = LogType.Runtime, bool writeToFile = true)
        {
            Logger.Warning(value, type, writeToFile);
        }

        public static void Error(string value, LogType type = LogType.Runtime, bool writeToFile = true)
        {
            Logger.Error(value, type, writeToFile);
        }

        public static void Log(string value, LogLevel level, LogType type = LogType.Runtime, bool writeToFile = true)
        {
            if (level == LogLevel.Error || level == LogLevel.Warning)
                Logger.Log(value, level, type, writeToFile);
        }
    }
}
