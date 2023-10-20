using System.Text.RegularExpressions;
using UnityEngine;

namespace NLog.Unity {

    [DisallowMultipleComponent]
    public class NLogConfig : MonoBehaviour {
        public LogLevel logLevel;
        public bool catchUnityLogs = true;
        Logger _unityLog;

        void Awake() {
            DontDestroyOnLoad(gameObject);
            LoggerFactory.globalLogLevel = LogLevel.Off;
        }

        void OnEnable() {
            LoggerFactory.globalLogLevel = logLevel;
        }

        void OnDisable() {
            LoggerFactory.globalLogLevel = LogLevel.Off;
        }

        void Start() {
            if (catchUnityLogs) {
                _unityLog = LoggerFactory.GetLogger("Unity");
                //Application.logMessageReceived += onLog;
                Application.logMessageReceivedThreaded += onLog;
            }
        }

        void OnDestroy() {
            //Application.logMessageReceived -= onLog;
            Application.logMessageReceivedThreaded -= onLog;
        }

        void onLog(string condition, string stackTrace, LogType type) {
            if (Regex.IsMatch(condition, "\\[(?:Debug|Warn|Error|Assert)\\]:"))
            {
                return;
            }
            if (type == LogType.Log) {
                _unityLog.Debug(condition);
            } else if (type == LogType.Warning) {
                _unityLog.Warn(condition);
            } else {
                _unityLog.Error(condition + "\n" + stackTrace);
            }
        }
    }
}