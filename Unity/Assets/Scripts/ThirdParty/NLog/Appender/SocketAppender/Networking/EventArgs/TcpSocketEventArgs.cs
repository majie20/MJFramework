using System;
using System.Net.Sockets;

namespace NLog {
    public class TcpSocketEventArgs : EventArgs {
        public Socket socket { get { return _socket; } }

        readonly Socket _socket;

        public TcpSocketEventArgs(Socket socket) {
            _socket = socket;
        }
    }
}

