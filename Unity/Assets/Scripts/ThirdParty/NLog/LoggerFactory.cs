using System.Collections.Generic;

namespace NLog {
    public static class LoggerFactory {
        public static LogLevel globalLogLevel {
            get { return _globalLogLevel; }
            set {
                _globalLogLevel = value;
                foreach (var logger in _loggers.Values) {
                    logger.logLevel = value;
                }
            }
        }

        static LogLevel _globalLogLevel;
        static Logger.LogDelegate _appenders;
        readonly static Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();

        public static void AddAppender(Logger.LogDelegate appender) {
            _appenders += appender;
            foreach (var logger in _loggers.Values) {
                logger.OnLog += appender;
            }
        }

        public static void RemoveAppender(Logger.LogDelegate appender) {
            _appenders -= appender;
            foreach (var logger in _loggers.Values) {
                logger.OnLog -= appender;
            }
        }

        public static Logger GetLogger(string name) {
            Logger logger;
            if (!_loggers.TryGetValue(name, out logger)) {
                logger = createLogger(name);
                _loggers.Add(name, logger);
            }

            return logger;
        }

        public static void Reset() {
            _loggers.Clear();
            _appenders = null;
        }

        static Logger createLogger(string name) {
            var logger = new Logger(name);
            logger.logLevel = globalLogLevel;
            logger.OnLog += _appenders;
            return logger;
        }
    }
}

