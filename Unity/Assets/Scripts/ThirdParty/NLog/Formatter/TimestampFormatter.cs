using System;

namespace NLog {
    public class TimestampFormatter {
        const string Format = "{0:yyyy/MM/dd/hh:mm:ss:fff}";

        public string FormatMessage(Logger logger, LogLevel logLevel, string message) {
            var time = string.Format(Format, DateTime.Now);
            return string.Format(Format, time + " " + message);
        }
    }
}

