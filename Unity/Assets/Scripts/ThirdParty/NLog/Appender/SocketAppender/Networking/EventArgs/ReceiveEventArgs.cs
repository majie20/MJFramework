using System;
using System.Net.Sockets;

namespace NLog {
    public class ReceiveEventArgs : EventArgs {
        public Socket client { get { return _client; } }
        public byte[] bytes { get { return _bytes; } }

        readonly Socket _client;
        readonly byte[] _bytes;

        public ReceiveEventArgs(Socket client, byte[] bytes) {
            _client = client;
            _bytes = bytes;
        }
    }
}

