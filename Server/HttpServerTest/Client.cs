using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HttpServerTest
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
            Console.WriteLine($"客户端 {ipEndPoint.Address}:{ipEndPoint.Port} 进行Http请求！");
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

                StringBuilder stringBuilder = new StringBuilder();
                while (true)
                {
                    int len = clientSocket.Receive(data);
                    stringBuilder.Append(Encoding.UTF8.GetString(data, 0, len));
                    if (len < 1024)
                    {
                        break;
                    }
                }

                if (stringBuilder.Length == 0)
                {
                    continue;
                }
                //GET /(?<action>[^?]+)\?+(?<args>[^\s]{ 0,})
                //Program.BroadcastMessage(message.ToString());

                string message = stringBuilder.ToString();
                Regex regex = new Regex(@"GET /(?<action>[^?]+)\?+(?<args>[^\s]{ 0,})");
                string action = regex.Match(message).Groups["action"].Value;
                string args = regex.Match(message).Groups["args"].Value;

                string result = HandleRequest(action, args);

                Console.WriteLine($"客户端 {ipEndPoint.Address}:{ipEndPoint.Port}：{stringBuilder}");
            }
        }

        private string HandleRequest(string action, string args)
        {
            switch (action)
            {
                case "add":
                    return Add(args);

                default:
                    return "OK";
            }
        }

        private string Add(string args)
        {
            Regex regex = new Regex(@"a=(?<a>\d+)&b=(?<b>\d+)&c=(?<c>\d+)");
            int a = int.Parse(regex.Match(args).Groups["a"].Value);
            int b = int.Parse(regex.Match(args).Groups["b"].Value);
            int c = int.Parse(regex.Match(args).Groups["c"].Value);

            return (a + b + c).ToString();
        }

        //private string CreateBody(string data)
        //{
        //}

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

         //internal class Program
    //{
    //    private static async Task Main(string[] args)
    //    {
    //        await StartServerAsync(args);
    //        Console.ReadLine();
    //    }

    //    public static async Task StartServerAsync(params string[] prefixes)
    //    {
    //        try
    //        {
    //            Console.WriteLine($"server starting at");
    //            var listener = new WebListener();

    //            listener.Settings.UrlPrefixes.Add("http://localhost:8080");
    //            listener.Start();

    //            do
    //            {
    //                using (RequestContext context = await listener.AcceptAsync())
    //                {
    //                    context.Response.Headers.Add("content-type", new string[] { "image/jpeg" });
    //                    context.Response.StatusCode = (int)HttpStatusCode.OK;

    //                    //byte[] buffer = GetContent(context.Request);
    //                    byte[] buffer = new byte[104867840];

    //                    using (FileStream stream = File.OpenRead("59298b37e5d541179a8081cb9a38e949.jpg"))
    //                    {
    //                        int len = await stream.ReadAsync(buffer);

    //                        Console.WriteLine(stream.Length);

    //                        context.Response.ContentLength = len;
    //                        await context.Response.Body.WriteAsync(buffer, 0, len);
    //                    }
    //                }
    //            } while (true);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //        }
    //    }

    //    private static byte[] GetContent(Request request)
    //    {
    //        string html = "[1,2,3]";

    //        byte[] data = new byte[20480];

    //        return Encoding.UTF8.GetBytes(html);
    //    }
    //}
    }
}