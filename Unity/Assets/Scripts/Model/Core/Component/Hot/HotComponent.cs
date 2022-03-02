using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using LitJson;
using UnityEngine;

namespace Model
{
    public class HotComponent : Component, IAwake
    {
        private byte[] Buffer = new byte[1024];

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

        private AssetsBundleSettings settings;

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
        }

        public override void Dispose()
        {
            Entity = null;
            isStart = false;
        }

        public void Run(bool isHot)
        {
            if (isHot)
            {
                Hot();
            }
            else
            {
                SumSize = 0;
                CurDownloadNum = 0;
                Normal();
            }
        }

        public async void Normal()
        {
            EventSystem eventSystem = Game.Instance.EventSystem;
            var configPath = FileHelper.JoinPath($"{HotConfig.AB_SAVE_RELATIVELY_PATH}{HotConfig.AB_CONFIG_NAME}.json", FileHelper.FilePos.StreamingAssetsPath, FileHelper.LoadMode.UnityWebRequest);
            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.LoadAssetsConfig);
            byte[] bytes = await FileHelper.LoadFileByUnityWebRequestAsync(configPath);
            string configStr = Encoding.UTF8.GetString(bytes);
            AbConfigs = JsonMapper.ToObject<List<ABConfig>>(configStr);

            for (int i = 0; i < AbConfigs.Count; i++)
            {
                SumSize += AbConfigs[i].Size;
            }
            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.LoadAssetsConfigComplete);
            await Game.Instance.Scene.GetComponent<AssetsComponent>().Run(false);
        }

        public async void Hot()
        {
            EventSystem eventSystem = Game.Instance.EventSystem;
            isStart = true;
            var httpComponent = Game.Instance.Scene.GetComponent<HttpComponent>();
            var assetsPath = FileHelper.JoinPath(HotConfig.AB_SAVE_RELATIVELY_PATH, FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
            FileHelper.CreateDir(assetsPath);

            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.LoadAssetsConfig);
            string configStr = null;
            var configPath = FileHelper.JoinPath($"{HotConfig.AB_SAVE_RELATIVELY_PATH}{HotConfig.AB_CONFIG_NAME}.json", FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
            if (File.Exists(configPath))
            {
                byte[] bytes = await FileHelper.LoadFileByStreamAsync(configPath);
                configStr = Encoding.UTF8.GetString(bytes);
            }
            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.LoadAssetsConfigComplete);

            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.CheckAssetsUpdate);
            List<ABConfig> oldAbConfigs = configStr == null ? null : JsonMapper.ToObject<List<ABConfig>>(configStr);
            configStr = await httpComponent.ToStringAsync(HttpConfig.TestURL1, HttpConfig.HOT_VERIFY_AB_PACK, HttpMethod.Post, string.IsNullOrEmpty(configStr) ? null : Encoding.UTF8.GetBytes(configStr));
            List<ABConfig> newAbConfigs = JsonMapper.ToObject<List<ABConfig>>(configStr);

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

            new Thread(() => FileHelper.SaveFileByStream(configPath, Encoding.UTF8.GetBytes(JsonMapper.ToJson(oldAbConfigs)))).Start();

            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.DownloadHotAssets);
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
                                                CurDownloadNum += len;
                                                System.Threading.Thread.MemoryBarrier();
                                                if ((CurDownloadNum / 1024) % 5 == 0)
                                                {
                                                    eventSystem.Invoke<LoadingViewProgressRefresh, float>((float)CurDownloadNum / SumSize);
                                                }
                                            }

                                            if (len < 1024)
                                            {
                                                eventSystem.Invoke<LoadingViewProgressRefresh, float>((float)CurDownloadNum / SumSize);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }, bytes);
                }).Start();
            }

            await UniTask.WaitWhile(() => CurDownloadNum < SumSize && this.isStart);

            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.DownloadHotAssetsComplete);

            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.UnzipAssets);
            var outputPath = FileHelper.JoinPath(HotConfig.AB_SAVE_RELATIVELY_PATH, FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
            UniTask[] tasks = new UniTask[newAbConfigs.Count];
            for (int i = 0; i < newAbConfigs.Count; i++)
            {
                var abPackPath = FileHelper.JoinPath(
                    $"{HotConfig.AB_SAVE_RELATIVELY_PATH}{newAbConfigs[i].ABName}",
                    FileHelper.FilePos.PersistentDataPath,
                    FileHelper.LoadMode.Stream);

                tasks[i] = UniTask.RunOnThreadPool(() =>
               {
                   ZipWrapper.UnzipFile(abPackPath, outputPath, Settings.ZipPassword);
                   File.Delete(abPackPath);
               });
            }
            await UniTask.WhenAll(tasks);
            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.UnzipAssetsComplete);

            await Game.Instance.Scene.GetComponent<AssetsComponent>().Run(true);
        }
    }
}