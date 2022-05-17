using System;
using System.Net;
using System.Net.Sockets;
using NLog;

namespace NLog {
    public class TcpClientSocket : AbstractTcpSocket {
        public event EventHandler OnConnect;

        public TcpClientSocket() {
            _log = LoggerFactory.GetLogger(GetType().Name);
        }

        public void Connect(IPAddress ip, int port) {
            _log.Debug(string.Format("Connecting to {0}:{1}...", ip, port));
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(ip, port, onConnected, _socket);
        }

        void onConnected(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            try {
                socket.EndConnect(ar);
                isConnected = true;
                IPEndPoint clientEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                _log.Info(string.Format("Connected to {0}:{1}",
                    clientEndPoint.Address, clientEndPoint.Port));
                if (OnConnect != null) {
                    OnConnect(this, null);
                }
                startReceiving(socket);
            } catch (Exception ex) {
                _log.Warn(ex.Message);
                triggerOnDisconnect();
            }
        }

        public override void Send(byte[] bytes) {
            SendWith(_socket, bytes);
        }

        protected override void disconnectedByRemote(Socket socket) {
            _log.Info("Disconnected by remote.");
            Disconnect();
        }

        public override void Disconnect() {
            if (isConnected) {
                _log.Debug("Disconnecting...");
                isConnected = false;
                _socket.BeginDisconnect(false, onDisconnected, _socket);
            } else {
                _log.Debug("Already diconnected.");
            }
        }

        void onDisconnected(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            socket.EndDisconnect(ar);
            socket.Close();
            _socket = null;
            _log.Debug("Disconnected.");
            triggerOnDisconnect();
        }
    }
}

