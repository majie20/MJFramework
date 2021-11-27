using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public class Client
    {
        private Socket clientSocket;
        private Thread t;
        private byte[] data = new byte[1024];
        private IPEndPoint ipEndPoint;

        public Client(Socket socket)
        {
            clientSocket = socket;
            ipEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            Console.WriteLine($"客户端 {ipEndPoint.Address}:{ipEndPoint.Port} 加入聊天室！");
            t = new Thread(ReceiveMessage);
            t.Start();
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                if (!clientSocket.Connected || clientSocket.Poll(10, SelectMode.SelectRead))
                {
                    Console.WriteLine($"客户端 {ipEndPoint.Address}:{ipEndPoint.Port} 断开连接！");
                    clientSocket.Close();
                    return;
                }


                StringBuilder message = new StringBuilder();
                while (true)
                {
                    int len = clientSocket.Receive(data);
                    message.Append(Encoding.UTF8.GetString(data, 0, len));
                    if (len < 1024)
                    {
                        break;
                    }
                }

                if (message.Length == 0)
                {
                    continue;
                }

                Program.BroadcastMessage(message.ToString());

                Console.WriteLine($"客户端 {ipEndPoint.Address}:{ipEndPoint.Port}：{message}");
            }
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(data);
        }

        public bool Connected
        {
            get
            {
                return clientSocket.Connected;
            }
        }
    }
}
