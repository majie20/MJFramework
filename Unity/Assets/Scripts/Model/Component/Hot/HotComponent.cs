using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class HotComponent : Component, IAwake
    {
        private byte[] Buffer = new byte[1024];

        private AssetsBundleSettings settings;

        private List<ABConfig> abConfigs;

        public List<ABConfig> AbConfigs
        {
            private set
            {
                abConfigs = value;
            }
            get
            {
                return abConfigs;
            }
        }

        public AssetsBundleSettings Settings
        {
            private set
            {
                settings = value;
            }
            get
            {
                return settings;
            }
        }

        private int sumSize;

        public int SumSize
        {
            private set
            {
                sumSize = value;
            }
            get
            {
                return sumSize;
            }
        }

        private int curDownloadNum;

        public int CurDownloadNum
        {
            private set
            {
                curDownloadNum = value;
            }
            get
            {
                return curDownloadNum;
            }
        }

        private bool isStart;

        private static readonly object padlock = new object();

        public void Awake()
        {
            Settings = Resources.Load<AssetsBundleSettings>("AssetsBundleSettings");
            Game.Instance.EventSystem.AddListener<HotProgressRefresh, int>(OnHotProgressRefresh, this);
        }

        private void OnHotProgressRefresh(int num)
        {
            Debug.Log($"------{num}------"); // MDEBUG:
        }

        public void Run(bool isHot)
        {
            if (isHot)
            {
                Hot();
            }
            else
            {
                Normal();
            }
        }

        public async void Normal()
        {
            var configPath = FileHelper.JoinPath($"{HotConfig.AB_SAVE_RELATIVELY_PATH}{HotConfig.AB_CONFIG_NAME}.json", FileHelper.FilePos.StreamingAssetsPath, FileHelper.LoadMode.UnityWebRequest);
            Debug.LogWarning("正在加载资源配置"); // MDEBUG:
            byte[] bytes = await FileHelper.LoadFileByUnityWebRequestAsync(configPath);
            string configStr = Encoding.UTF8.GetString(bytes);
            AbConfigs = JsonConvert.DeserializeObject<List<ABConfig>>(configStr);

            await Game.Instance.Scene.GetComponent<AssetsComponent>().Run(false);
        }

        public async void Hot()
        {
            isStart = true;
            var httpComponent = Game.Instance.Scene.GetComponent<HttpComponent>();
            var assetsPath = FileHelper.JoinPath(HotConfig.AB_SAVE_RELATIVELY_PATH, FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
            FileHelper.CreateDir(assetsPath);

            Debug.LogWarning("正在加载资源配置"); // MDEBUG:
            string configStr = null;
            var configPath = FileHelper.JoinPath($"{HotConfig.AB_SAVE_RELATIVELY_PATH}{HotConfig.AB_CONFIG_NAME}.json", FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
            if (File.Exists(configPath))
            {
                byte[] bytes = await FileHelper.LoadFileByStreamAsync(configPath);
                configStr = Encoding.UTF8.GetString(bytes);
            }

            Debug.LogWarning("正在检查资源更新"); // MDEBUG:
            List<ABConfig> oldAbConfigs = configStr == null ? null : JsonConvert.DeserializeObject<List<ABConfig>>(configStr);
            configStr = await httpComponent.ToStringAsync(HttpConfig.TestURL1, HttpConfig.HOT_VERIFY_AB_PACK, HttpMethod.Post, string.IsNullOrEmpty(configStr) ? null : Encoding.UTF8.GetBytes(configStr));
            List<ABConfig> newAbConfigs = JsonConvert.DeserializeObject<List<ABConfig>>(configStr);

            if (oldAbConfigs == null)
            {
                oldAbConfigs = newAbConfigs;
            }
            else
            {
                for (int i = 0; i < oldAbConfigs.Count; i++)
                {
                    for (int j = 0; j < newAbConfigs.Count; j++)
                    {
                        if (oldAbConfigs[i].ABName == newAbConfigs[j].ABName)
                        {
                            oldAbConfigs[i].Size = newAbConfigs[j].Size;
                            oldAbConfigs[i].CRC = newAbConfigs[j].CRC;
                        }
                    }
                }
            }

            for (int i = 0; i < newAbConfigs.Count; i++)
            {
                SumSize += newAbConfigs[i].Size;
            }

            this.AbConfigs = oldAbConfigs;

            new Thread(() => FileHelper.SaveFileByStream(configPath, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(oldAbConfigs)))).Start();

            Debug.LogWarning("正在下载更新资源"); // MDEBUG:
            var eventSystem = Game.Instance.EventSystem;
            for (int i = 0; i < newAbConfigs.Count; i++)
            {
                var abPackPath = FileHelper.JoinPath(
                    $"{HotConfig.AB_SAVE_RELATIVELY_PATH}{newAbConfigs[i].ABName}",
                    FileHelper.FilePos.PersistentDataPath,
                    FileHelper.LoadMode.Stream);
                byte[] bytes = Encoding.UTF8.GetBytes(newAbConfigs[i].ABName);
                new Thread(() =>
                {
                    httpComponent.ToStream(HttpConfig.TestURL1,
                        HttpConfig.HOT_GET_AB_PACK, HttpMethod.Post, (response) =>
                        {
                            using (FileStream fileStream = File.Create(abPackPath))
                            {
                                using (BufferedStream bufferedStream = new BufferedStream(fileStream))
                                {
                                    using (Stream stream = response.Content.ReadAsStreamAsync().Result)
                                    {
                                        while (this.isStart)
                                        {
                                            int len = stream.Read(Buffer, 0, 1024);

                                            bufferedStream.Write(Buffer, 0, len);
                                            lock (padlock)
                                            {
                                                curDownloadNum += len;
                                                System.Threading.Thread.MemoryBarrier();
                                                if ((curDownloadNum / 1024) % 5 == 0)
                                                {
                                                    eventSystem.Invoke<HotProgressRefresh, int>(curDownloadNum);
                                                }
                                            }

                                            if (len < 1024)
                                            {
                                                eventSystem.Invoke<HotProgressRefresh, int>(curDownloadNum);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }, bytes);
                }).Start();
            }

            while (CurDownloadNum < SumSize && this.isStart)
            {
                await Task.Delay(100);
            }
            Debug.LogWarning("资源更新完毕"); // MDEBUG:

            Debug.LogWarning("正在解压资源"); // MDEBUG:
            var outputPath = FileHelper.JoinPath(HotConfig.AB_SAVE_RELATIVELY_PATH, FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
            Task[] tasks = new Task[newAbConfigs.Count];
            for (int i = 0; i < newAbConfigs.Count; i++)
            {
                var abPackPath = FileHelper.JoinPath(
                    $"{HotConfig.AB_SAVE_RELATIVELY_PATH}{newAbConfigs[i].ABName}",
                    FileHelper.FilePos.PersistentDataPath,
                    FileHelper.LoadMode.Stream);

                tasks[i] = new Task(() =>
                {
                    ZipWrapper.UnzipFile(abPackPath, outputPath, Settings.ZipPassword);
                    File.Delete(abPackPath);
                });
            }
            await Task.WhenAll(tasks);

            await Game.Instance.Scene.GetComponent<AssetsComponent>().Run(true);
        }

        public override void Dispose()
        {
            Entity = null;
            isStart = false;
            Game.Instance.EventSystem.RemoveListener<HotProgressRefresh, int>(this);
        }
    }
}