using NLog;
using UnityEngine;

namespace NLog.Unity {
    public class DebugLogAppender : MonoBehaviour {
        void OnEnable() {
            LoggerFactory.AddAppender(log);
        }

        void OnDisable() {
            LoggerFactory.RemoveAppender(log);
        }

        void log(Logger logger, LogLevel logLevel, string message) {
            if (logLevel <= LogLevel.Info) {
                Debug.Log(message);
            } else if (logLevel == LogLevel.Warn) {
                Debug.LogWarning(message);
            } else {
                Debug.LogError(message);
            }
        }
    }
}
