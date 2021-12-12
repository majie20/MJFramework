using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class NetComponent : Component, IAwake
    {
        public static Uri URL = new Uri("http://127.0.0.1:8080/");
        public static string HOT_GET_NAME = "Hot/GetName";
        public static string HOT_GET_TEXTURE = "Hot/GetTexture";

        private HttpClient httpClient = new HttpClient();

        private byte[] buffer = new byte[1024];


        public void Awake()
        {

        }

        public async Task<string> GetMajie()
        {
            HttpResponseMessage response = await httpClient.GetAsync(URL);

            foreach (var v in response.Headers)
            {
                if (v.Key == "FileName")
                {
                    return string.Join("", v.Value);
                }
            }

            return "";
        }

        public async Task GetTexture()
        {
            HttpResponseMessage response = await httpClient.GetAsync(new Uri(URL, HOT_GET_TEXTURE));
            response.EnsureSuccessStatusCode();

            Debug.LogError($"Response Status Code: {response.StatusCode} {response.ReasonPhrase}");

            string fileName = null;

            foreach (var v in response.Headers)
            {
                if (v.Key == "FileName")
                {
                    fileName = string.Join("", v.Value);
                }
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            if (fileName != null)
            {
                using (FileStream fileStream = File.Create(FileHelper.JoinPath(fileName, FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream)))
                {
                    using (BufferedStream bufferedStream = new BufferedStream(fileStream))
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            while (true)
                            {
                                int len = await stream.ReadAsync(buffer, 0, 1024);

                                await bufferedStream.WriteAsync(buffer, 0, len);
                                if (len < 1024)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }



        public override void Dispose()
        {

            Entity = null;
        }
    }
}