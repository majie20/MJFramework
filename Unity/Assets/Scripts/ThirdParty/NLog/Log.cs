namespace NLog
{
    public static class Log
    {
        private static readonly Logger _log = LoggerFactory.GetLogger("Log");

        private static void Debug(string message)
        {
            _log.Debug(message);
        }

        public static void Debug(object message)
        {
            _log.Debug(message?.ToString());
        }

        private static void Warn(string message)
        {
            _log.Warn(message);
        }

        public static void Warn(object message)
        {
            _log.Warn(message?.ToString());
        }

        private static void Error(string message)
        {
            _log.Error(message);
        }

        public static void Error(object message)
        {
            _log.Error(message?.ToString());
        }

        public static void Assert(bool condition, string message)
        {
            _log.Assert(condition, message);
        }

        public static void Assert(bool condition, object message)
        {
            _log.Assert(condition, message?.ToString());
        }
    }
}