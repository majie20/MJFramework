using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace Model
{
    public class LoadSceneData
    {
        public string        ScenePath;
        public List<string>  SettingsPath;
        public Func<UniTask> Call;
    }

    public class LoadHelper
    {
        public static async UniTask LoadScene(LoadSceneData data)
        {
            var gameRoot = Game.Instance.GGetComponent<GameRootComponent>();

            if (gameRoot != null)
            {
                ObjectHelper.RemoveEntity(gameRoot.Entity);
            }

            UI2DRootComponent ui2DRootComponent = Game.Instance.GGetComponent<UI2DRootComponent>();
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.Low);
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.Normal);
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.High);
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.Top);

            await SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Single);

            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.LoadAssets);
            ResHelper.GCAndUnload();
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            data.SettingsPath.Add(data.ScenePath);

            var downloader = YooAssets.CreateBundleDownloader(data.SettingsPath.ToArray(), 32, 3);
            downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
            downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
            downloader.OnDownloadOverCallback = OnDownloadOverCallback;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileCallback;
            downloader.BeginDownload();
            await downloader;

            List<UniTask> tasks = new List<UniTask> { component.LoadSceneAsync(data.ScenePath), component.LoadAssetReferenceSettingsAsync(data.SettingsPath[0]) };

            for (int i = data.SettingsPath.Count - 2; i >= 1; i--)
            {
                if (!string.IsNullOrEmpty(data.SettingsPath[i]))
                {
                    tasks.Add(component.CreateLoadHandle(typeof(Object), data.SettingsPath[i]).ToUniTask());
                }
            }

            await UniTask.WhenAll(tasks);

            //await UniTask.Delay(500);
            NLog.Log.Debug("资源加载完成------------------------11----");

            ObjectHelper.CreateComponent<GameRootComponent>(ObjectHelper.CreateEntity<Entity>(Game.Instance.Scene, GameObject.Find("GameRoot")));

            if (data.Call is not null)
            {
                await data.Call();
            }

            ResHelper.GCAndUnload();
            //await UniTask.Delay(300);

            UIHelper.CloseUIView<LoadingViewComponent>();
            Game.Instance.EventSystem.Invoke<E_LoadSceneFinish>();
        }

        private static void OnDownloadErrorCallback(string fileName, string error)
        {
            NLog.Log.Error($"FileName:{fileName}\nError:{error}");
        }

        private static void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            //NLog.Log.Error(
            //    $"totalDownloadCount = {totalDownloadCount}\ncurrentDownloadCount = {currentDownloadCount}\ntotalDownloadBytes = {totalDownloadBytes}\ncurrentDownloadBytes = {currentDownloadBytes}");

            Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh1, int, int, long, long>(totalDownloadCount, currentDownloadCount, totalDownloadBytes,
                currentDownloadBytes);
        }

        private static void OnDownloadOverCallback(bool isSucceed)
        {
        }

        private static void OnStartDownloadFileCallback(string fileName, long sizeBytes)
        {
            //NLog.Log.Debug($"fileName:{fileName}\nsizeBytes:{sizeBytes}");
        }
    }
}