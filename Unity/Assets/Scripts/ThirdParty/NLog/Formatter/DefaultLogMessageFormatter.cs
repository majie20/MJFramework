using System;

namespace NLog {
    public class DefaultLogMessageFormatter {
        const string Format = "{0} {1}: {2}";
        static readonly int _maxLogLevelLength;

        static DefaultLogMessageFormatter() {
            _maxLogLevelLength = 0;
            var names = Enum.GetNames(typeof(LogLevel));
            foreach (var name in names) {
                var length = name.Length;
                if (length > _maxLogLevelLength) {
                    _maxLogLevelLength = length;
                }
            }

            _maxLogLevelLength += 2;
        }

        public string FormatMessage(Logger logger, LogLevel logLevel, string message) {
            var logLevelStr = ("[" + logLevel.ToString().ToUpper() + "]").PadRight(_maxLogLevelLength);
            return string.Format(Format, logLevelStr, logger.name, message);
        }
    }
}

