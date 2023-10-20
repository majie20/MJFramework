using System;
using System.Net.Sockets;
using NLog;

namespace NLog {
    public struct ReceiveVO {
        public Socket socket;
        public byte[] buffer;
    }

    public abstract class AbstractTcpSocket {
        public event EventHandler<ReceiveEventArgs> OnReceive;
        public event EventHandler OnDisconnect;

        public bool isConnected { get; protected set; }

        protected Logger _log;
        protected Socket _socket;

        protected void triggerOnReceive(ReceiveVO receiveVO, int bytesReceived) {
            if (OnReceive != null) {
                OnReceive(this, new ReceiveEventArgs(receiveVO.socket,
                    trimmedBuffer(receiveVO.buffer, bytesReceived)));
            }
        }

        protected void triggerOnDisconnect() {
            if (OnDisconnect != null) {
                OnDisconnect(this, null);
            }
        }

        protected void startReceiving(Socket socket) {
            var receiveVO = new ReceiveVO {
                socket = socket,
                buffer = new byte[socket.ReceiveBufferSize]
            };
            receive(receiveVO);
        }

        protected void receive(ReceiveVO receiveVO) {
            receiveVO.socket.BeginReceive(receiveVO.buffer, 0,
                receiveVO.buffer.Length, SocketFlags.None, onReceived, receiveVO);
        }

        protected void onReceived(IAsyncResult ar) {
            var receiveVO = (ReceiveVO)ar.AsyncState;
            if (isConnected) {
                var bytesReceived = receiveVO.socket.EndReceive(ar);

                if (bytesReceived == 0) {
                    disconnectedByRemote(receiveVO.socket);
                } else {
                    _log.Debug(string.Format("Received {0} bytes.", bytesReceived));
                    triggerOnReceive(receiveVO, bytesReceived);

                    receive(receiveVO);
                }
            }
        }

        public void SendWith(Socket socket, byte[] bytes) {
            if (isConnected && socket.Connected) {
                socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, onSent, socket);
            }
        }

        void onSent(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        protected abstract void disconnectedByRemote(Socket socket);

        protected byte[] trimmedBuffer(byte[] buffer, int length) {
            var trimmed = new byte[length];
            Array.Copy(buffer, trimmed, length);
            return trimmed;
        }

        public abstract void Send(byte[] bytes);

        public abstract void Disconnect();
    }
}
