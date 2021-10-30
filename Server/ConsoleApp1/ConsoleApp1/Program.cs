using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        private static List<Client> clientList = new List<Client>();

        private static void Main(string[] args)
        {
            //1、创建socket
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //2、绑定IP和端口号 192.168.31.141
            IPAddress iPAddress = new IPAddress(new byte[] { 192, 168, 31, 141 });
            EndPoint point = new IPEndPoint(iPAddress, 7788);
            tcpServer.Bind(point);
            //3、开始监听（等待客户端连接）
            tcpServer.Listen(100);

            while (true)
            {
                Socket clientSocket = tcpServer.Accept();//暂停当前线程，直到有一个客户端连接
                Client client = new Client(clientSocket);

                clientList.Add(client);
            }

            //string message = "majie";
            //var data = Encoding.UTF8.GetBytes(message);
            //clientSocket.Send(data);

            //byte[] data2 = new byte[1024];
            //int len = clientSocket.Receive(data2);
            //string message2 = Encoding.UTF8.GetString(data2, 0, len);
            //Console.WriteLine(message2);

        }

        public static void BroadcastMessage(string message)
        {
            List<Client> obsoleteClientList = new List<Client>();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].Connected)
                {
                    clientList[i].SendMessage(message);
                }
                else
                {
                    obsoleteClientList.Add(clientList[i]);
                }
            }

            for (int i = 0; i < obsoleteClientList.Count; i++)
            {
                clientList.Remove(obsoleteClientList[i]);
            }
        }
    }
}