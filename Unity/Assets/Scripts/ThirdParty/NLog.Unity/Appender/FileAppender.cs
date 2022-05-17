using NLog;
using UnityEngine;

namespace NLog.Unity {
    public class FileAppender : MonoBehaviour {
        public string filePath = "Log.txt";
        FileWriter _fileWriter;
        DefaultLogMessageFormatter _defaultFormatter;
        TimestampFormatter _timestampFormatter;

        void Awake() {
            _fileWriter = new FileWriter(filePath);
            _defaultFormatter = new DefaultLogMessageFormatter();
            _timestampFormatter = new TimestampFormatter();
        }

        void OnEnable() {
            LoggerFactory.AddAppender(write);
        }

        void OnDisable() {
            LoggerFactory.RemoveAppender(write);
        }

        void write(Logger logger, LogLevel logLevel, string message) {
            message = _defaultFormatter.FormatMessage(logger, logLevel, message);
            message = _timestampFormatter.FormatMessage(logger, logLevel, message);
            _fileWriter.WriteLine(message);
        }
    }
}