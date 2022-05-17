using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using NLog;

namespace NLog {
    public class TcpServerSocket : AbstractTcpSocket {
        public event EventHandler<TcpSocketEventArgs> OnClientConnect;
        public event EventHandler<TcpSocketEventArgs> OnClientDisconnect;

        public int connectedClients { get { return _clients.Count; } }

        readonly List<Socket> _clients;

        public TcpServerSocket() {
            _log = LoggerFactory.GetLogger(GetType().Name);
            _clients = new List<Socket>();
        }

        public void Listen(int port) {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(100);
                isConnected = true;
                _log.Info(string.Format("Listening on port {0}...", port));
                accept();
            } catch (Exception ex) {
                _socket = null;
                _log.Warn(ex.Message);
            }
        }

        void accept() {
            _socket.BeginAccept(onAccepted, _socket);
        }

        void onAccepted(IAsyncResult ar) {
            if (isConnected) {
                var server = (Socket)ar.AsyncState;
                acceptedClientConnection(server.EndAccept(ar));
                accept();
            }
        }

        void acceptedClientConnection(Socket client) {
            _clients.Add(client);
            IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
            _log.Info(string.Format("New client connection accepted ({0}:{1})",
                clientEndPoint.Address, clientEndPoint.Port));
            if (OnClientConnect != null) {
                OnClientConnect(this, new TcpSocketEventArgs(client));
            }

            startReceiving(client);
        }

        protected override void disconnectedByRemote(Socket socket) {
            try {
                IPEndPoint clientEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                _log.Info(string.Format("Client disconnected ({0}:{1})",
                    clientEndPoint.Address, clientEndPoint.Port));
            } catch (Exception) {
                _log.Info("Client disconnected.");
            }
            socket.Close();
            _clients.Remove(socket);
            if (OnClientDisconnect != null) {
                OnClientDisconnect(this, new TcpSocketEventArgs(socket));
            }
        }

        public override void Send(byte[] bytes) {
            foreach (var client in _clients) {
                SendWith(client, bytes);
            }
        }

        public override void Disconnect() {
            foreach (var client in _clients) {
                client.BeginDisconnect(false, onClientDisconnected, client);
            }

            if (isConnected) {
                _log.Info("Stopped listening.");
                isConnected = false;
                _socket.Close();
                triggerOnDisconnect();
            } else {
                _log.Info("Already diconnected.");
            }
        }

        void onClientDisconnected(IAsyncResult ar) {
            var client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
            disconnectedByRemote(client);
        }
    }
}