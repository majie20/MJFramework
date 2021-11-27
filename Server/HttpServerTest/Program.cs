using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await new HotHttpServer().Start("http://127.0.0.1:8080/");//这里以本机测试的，可以修改对应服务端的ip
        }

        private class HotHttpServer
        {
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
                    HttpListenerContext context = await listener.GetContextAsync();//获取HttpListenerContext，等待传入的请求，该方法将阻塞进程，直到收到请求
                                                                                   //处理Http请求，通过Request属性获取表示客户端请求的对象
                    await HandleContext(context);
                }
                listener.Stop();//关闭侦听器，并释放相关资源
            }

            private async Task HandleContext(HttpListenerContext context)
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

                            byte[] buffer = new byte[1024];

                            using (FileStream stream = File.OpenRead("59298b37e5d541179a8081cb9a38e949.jpg"))
                            {
                                response.ContentLength64 = stream.Length;
                                while (true)
                                {
                                    int len = await stream.ReadAsync(buffer);

                                    await output.WriteAsync(buffer, 0, len);

                                    //await Task.Delay(100);

                                    if (len < 1024)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else if (methodStr == "/Hot/GetTextureABPack")
                        {
                            response.Headers.Add("content-type", "application/octet-stream");
                            response.Headers.Add("FileName", "AssetBundleRes.ma");
                            response.Headers.Add("Content-Disposition", "attachment;FileName=" + "AssetBundleRes.ma");
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusDescription = "OK";

                            byte[] buffer = new byte[1024];

                            using (FileStream stream = File.OpenRead("zip_src/AssetBundleRes.ma"))
                            {
                                response.ContentLength64 = stream.Length;
                                while (true)
                                {
                                    int len = await stream.ReadAsync(buffer);

                                    await output.WriteAsync(buffer, 0, len);

                                    //await Task.Delay(100);

                                    if (len < 1024)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            response.Headers.Add("content-type", "text/plain");
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            response.StatusDescription = "NotFound";

                            byte[] buffer = Encoding.UTF8.GetBytes("null");
                            response.ContentLength64 = buffer.Length;
                            await output.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    response.Abort();
                    response.Close();
                }
            }
        }
    }
}