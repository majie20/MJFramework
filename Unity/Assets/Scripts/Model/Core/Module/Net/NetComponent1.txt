using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Model
{
    [LifeCycle]
    public class NetComponent : Component, IAwake
    {
        private Socket tcpClient;
        public static int PORT = 7788;
        public static string IP = "192.168.31.141";
        private Thread thread;
        private byte[] data = new byte[1024];
        private IPEndPoint ipEndPoint;

        public void Awake()
        {
            //1、创建socket
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //2、发起建立连接的请求
            IPAddress ipAddress = IPAddress.Parse(IP);
            EndPoint point = new IPEndPoint(ipAddress, PORT);
            tcpClient.Connect(point);

            thread = new Thread(ReceiveMessage);
            thread.Start();
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                if (!tcpClient.Connected)
                {
                    tcpClient.Close();
                    return;
                }

                StringBuilder message = new StringBuilder();
                while (true)
                {
                    int len = tcpClient.Receive(data);
                    message.Append(Encoding.UTF8.GetString(data, 0, len));
                    if (len < 1024)
                    {
                        break;
                    }
                }

                Game.Instance.EventSystem.Invoke<NetTestSend, string>(message.ToString());
            }
        }

        public IPEndPoint IpEndPoint
        {
            get
            {
                if (ipEndPoint == null)
                {
                    ipEndPoint = (IPEndPoint)tcpClient.LocalEndPoint;
                }
                return ipEndPoint;
            }
        }

        public void Send(string message)
        {
            tcpClient.Send(Encoding.UTF8.GetBytes(message));
        }

        public override void Dispose()
        {
            thread.Abort();
            tcpClient.Shutdown(SocketShutdown.Both);
            tcpClient.Close();
            thread = null;
            tcpClient = null;
            Entity = null;
        }
    }
}