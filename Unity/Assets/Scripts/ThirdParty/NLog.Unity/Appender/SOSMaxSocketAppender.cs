using System.Net;
using NLog;
using UnityEngine;

namespace NLog.Unity {
    public class SOSMaxSocketAppender : MonoBehaviour {
        public string sendToIpAddress = "127.0.0.1";
        public bool cacheLogsWhenNotConnected = true;
        const int port = 4444;
        SOSMaxAppender _socket;

        void Awake() {
            _socket = new SOSMaxAppender();

            if (cacheLogsWhenNotConnected) {
                LoggerFactory.AddAppender(send);
            }
        }

        void OnEnable() {
            if (!cacheLogsWhenNotConnected) {
                LoggerFactory.AddAppender(send);
            }

            _socket.Connect(IPAddress.Parse(sendToIpAddress), port);
        }

        void OnDisable() {
            _socket.Disconnect();

            if (!cacheLogsWhenNotConnected) {
                LoggerFactory.RemoveAppender(send);
            }
        }

        void send(Logger logger, LogLevel logLevel, string message) {
            _socket.Send(logLevel, message);
        }
    }
}