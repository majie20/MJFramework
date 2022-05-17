using System;
using System.Collections.Generic;
using System.Net;

namespace NLog {
    public abstract class SocketAppenderBase {
        public AbstractTcpSocket socket { get; private set; }

        readonly List<HistoryItem> _history = new List<HistoryItem>();

        public void Connect(IPAddress ip, int port) {
            var client = new TcpClientSocket();
            client.OnConnect += onConnected;
            client.Connect(ip, port);
            socket = client;
        }

        public void Listen(int port) {
            var server = new TcpServerSocket();
            server.OnClientConnect += onConnected;
            server.Listen(port);
            socket = server;
        }

        public void Disconnect() {
            socket.Disconnect();
        }

        public void Send(LogLevel logLevel, string message) {
            if (isSocketReady()) {
                socket.Send(serializeMessage(logLevel, message));
            } else {
                _history.Add(new HistoryItem(logLevel, message));
            }
        }

        bool isSocketReady() {
            return socket != null &&
            (socket is TcpClientSocket && socket.isConnected) ||
            (socket is TcpServerSocket && ((TcpServerSocket)socket).connectedClients > 0);
        }

        void onConnected(object sender, EventArgs e) {
            if (_history.Count > 0) {
                Send(LogLevel.Debug, "SocketAppenderBase: Flush history - - - - - - - - - - - - - - - - - - - -");
                foreach (HistoryItem item in _history) {
                    Send(item.logLevel, item.message);
                }

                Send(LogLevel.Debug, "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                _history.Clear();
            }
        }

        protected abstract byte[] serializeMessage(LogLevel logLevel, string message);

        struct HistoryItem {
            public LogLevel logLevel;
            public string message;

            public HistoryItem(LogLevel logLevel, string message) {
                this.logLevel = logLevel;
                this.message = message;
            }
        }
    }
}