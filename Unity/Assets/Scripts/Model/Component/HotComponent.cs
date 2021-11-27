using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class HotComponent : Component, IAwake
    {
        private byte[] buffer = new byte[1024];

        private string path;

        public void Awake()
        {
        }

        public async void Start()
        {
            await Game.Instance.Scene.GetComponent<HttpComponent>().ToStreamAsync(HttpConfig.TestURL1,
                HttpConfig.HOT_GET_AB_PACK, HttpMethod.Get, this.Download);

            var outputPath = FileHelper.JoinPath("Assets/", FileHelper.FilePos.persistentDataPath, FileHelper.LoadMode.Stream);
            await Task.Run(() => ZipWrapper.UnzipFile(this.path, outputPath, "majie"));

            await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAssetBundleManifestByUWRAsync(FileHelper.JoinPath("Assets/AssetBundleRes/AssetBundleRes", FileHelper.FilePos.persistentDataPath, FileHelper.LoadMode.Stream));
        }

        private async Task Download(HttpResponseMessage response)
        {
            string fileName = null;

            foreach (var v in response.Headers)
            {
                if (v.Key == "FileName")
                {
                    fileName = string.Join("", v.Value);
                    break;
                }
            }

            if (fileName != null)
            {
                var assetsPath = FileHelper.JoinPath("Assets/", FileHelper.FilePos.persistentDataPath,
                    FileHelper.LoadMode.Stream);

                FileHelper.CreateDir(assetsPath);

                FileHelper.DelectDir(assetsPath);

                this.path = FileHelper.JoinPath($"Assets/{fileName}", FileHelper.FilePos.persistentDataPath,
                    FileHelper.LoadMode.Stream);
                using (FileStream fileStream = File.Create(path))
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
            else
            {
                Debug.LogError($"{fileName}为空");
            }
        }

        public override void Dispose()
        {
            Entity = null;
        }
    }
}