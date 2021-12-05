using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServerTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            HotHttpServer server = new HotHttpServer();
            List<ABConfig> abConfigs = JsonConvert.DeserializeObject<List<ABConfig>>(Encoding.UTF8.GetString(await FileHelper.LoadFileByStreamAsync($"{HotConfig.AB_SAVE_RELATIVELY_PATH}{HotConfig.AB_CONFIG_NAME}.json")));
            for (int i = 0; i < abConfigs.Count; i++)
            {
                server.AbConfigDic.Add(abConfigs[i].ABName, abConfigs[i]);
            }

            await server.Start("http://127.0.0.1:8080/");//这里以本机测试的，可以修改对应服务端的ip
        }

        private class HotHttpServer
        {
            public Dictionary<string, ABConfig> AbConfigDic;

            public HotHttpServer()
            {
                AbConfigDic = new Dictionary<string, ABConfig>();
            }

            public async Task Start(string prefixes)//一个简单的HttpListener，使用Prefixes属性来访问集合，主要是用Prefixes属性获取和输出处理的 URI 前缀，这个类不能被继承
            {
                if (!HttpListener.IsSupported)
                {
                    //该类只能在Windows xp sp2或者Windows server 200 以上的操作系统中才能使用，因为这个类必须使用Http.sys系统组件才能完成工作
                    //所以在使用前应该先判断一下是否支持该类
                    Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }

                if (string.IsNullOrEmpty(prefixes))
                    throw new ArgumentException("prefixes");

                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(prefixes);
                listener.Start();//调用listener.Start()实现端口的绑定，并开始监听客户端的需求

                Console.WriteLine("服务器已经启动，开始监听.....\n");

                while (true)
                {
                    //通过HttpListenerContext对象提供对HttpListener类使用的请求和响应对象的访问
                    //获取HttpListenerContext，等待传入的请求，该方法将阻塞进程，直到收到请求
                    //处理Http请求，通过Request属性获取表示客户端请求的对象
                    HttpListenerContext context = await listener.GetContextAsync();

                    Console.WriteLine("开始连接");
                    Thread thread = new Thread(() => HandleContext(context));
                    thread.Start();
                }
                listener.Stop();//关闭侦听器，并释放相关资源
            }

            private void HandleContext(HttpListenerContext context)
            {
                HttpListenerRequest request = context.Request;
                //通过Response属性获取表示 HttpListener 将要发送到客户端的响应的对象
                HttpListenerResponse response = context.Response;

                var methodStr = request.Url.AbsolutePath;

                try
                {
                    //获得一个响应流并对客户端输出
                    using (Stream output = response.OutputStream)
                    {
                        if (methodStr == "/Hot/GetTexture")
                        {
                            response.Headers.Add("content-type", "application/octet-stream");
                            response.Headers.Add("FileName", "59298b37e5d541179a8081cb9a38e949.jpg");
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusDescription = "OK";

                            using (FileStream stream = File.OpenRead("59298b37e5d541179a8081cb9a38e949.jpg"))
                            {
                                var buffer = new byte[1024];
                                response.ContentLength64 = stream.Length;
                                while (true)
                                {
                                    int len = stream.Read(buffer);

                                    output.Write(buffer, 0, len);

                                    Thread.Sleep(100);

                                    if (len < 1024)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else if (methodStr == "/Hot/VerifyABPack")
                        {
                            response.Headers.Add("content-type", "application/json");
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusDescription = "OK";

                            using (Stream stream = request.InputStream)
                            {
                                List<ABConfig> dAbConfigs;
                                var buffer = new byte[request.ContentLength64];
                                if (request.ContentLength64 == 0)
                                {
                                    dAbConfigs = AbConfigDic.Values.ToList();
                                }
                                else
                                {
                                    dAbConfigs = new List<ABConfig>();
                                    stream.Read(buffer, 0, buffer.Length);

                                    List<ABConfig> abConfigs = JsonConvert.DeserializeObject<List<ABConfig>>(Encoding.UTF8.GetString(buffer));
                                    for (int i = 0; i < abConfigs.Count; i++)
                                    {
                                        if (AbConfigDic[abConfigs[i].ABName].CRC != abConfigs[i].CRC)
                                        {
                                            dAbConfigs.Add(abConfigs[i]);
                                        }
                                    }
                                }

                                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dAbConfigs));
                                response.ContentLength64 = buffer.Length;
                                output.Write(buffer, 0, buffer.Length);
                            }
                        }
                        else if (methodStr == "/Hot/GetABPack")
                        {
                            if (request.ContentLength64 == 0)
                            {
                                NotFound(response, output);
                            }
                            else
                            {
                                response.Headers.Add("content-type", "application/octet-stream");
                                response.StatusCode = (int)HttpStatusCode.OK;
                                response.StatusDescription = "OK";
                                using (Stream stream = request.InputStream)
                                {
                                    var buffer = new byte[request.ContentLength64];
                                    stream.Read(buffer, 0, buffer.Length);

                                    var name = Encoding.UTF8.GetString(buffer);

                                    response.Headers.Add("Content-Disposition", $"attachment;FileName={name}");

                                    using (FileStream fileStream = File.OpenRead($"zip_src/{name}"))
                                    {
                                        buffer = new byte[1024];
                                        response.ContentLength64 = fileStream.Length;
                                        while (true)
                                        {
                                            int len = fileStream.Read(buffer);

                                            output.Write(buffer, 0, len);

                                            Thread.Sleep(100);

                                            if (len < 1024)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            NotFound(response, output);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    response.Abort();
                    response.Close();
                }

                Console.WriteLine("结束");
            }

            private void NotFound(HttpListenerResponse response, Stream output)
            {
                response.Headers.Add("content-type", "text/plain");
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusDescription = "NotFound";

                byte[] buffer = Encoding.UTF8.GetBytes("null");
                response.ContentLength64 = buffer.Length;
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }
}