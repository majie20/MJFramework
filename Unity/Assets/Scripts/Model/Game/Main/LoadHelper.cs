using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using NLog;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace Model
{
    public class LoadSceneData
    {
        public string                            ScenePath;
        public List<AssetReferenceSettings.Info> AssetPaths;
        public Func<UniTask>                     Call;
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

            UniTask.Void(async () => await SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Single));

            UI2DRootComponent ui2DRootComponent = Game.Instance.GGetComponent<UI2DRootComponent>();
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.Low);
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.Normal);
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.High);
            ui2DRootComponent.ClearUIViewByLayer(UIViewLayer.Top);

            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.LoadAssets);
            ResHelper.GCAndUnload();
            await UniTask.Delay(200);

            data.AssetPaths ??= new List<AssetReferenceSettings.Info>();

            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
#if UNITY_WEBGL
            var paths = new string[data.AssetPaths.Count + 1];
            paths[0] = data.ScenePath;

            {
            for (int i = data.AssetPaths.Count - 1; i >= 0; i--)
                paths[i + 1] = data.AssetPaths[i].Path;
            }

            var downloader = YooAssets.CreateBundleDownloader(paths, 32, 3);
            downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
            downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
            downloader.OnDownloadOverCallback = OnDownloadOverCallback;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileCallback;
            downloader.BeginDownload();
            await downloader;

            if (downloader.CurrentDownloadCount > 0)
            {
                await UniTask.Delay(500);
            }
#endif
            var handles = new List<OperationHandleBase>();
            var tasks = new List<UniTask>();

            var progress = new Progress<float>(f =>
            {
                var score = 0.0f;

                for (int i = handles.Count - 1; i >= 0; i--)
                {
                    score += handles[i].Progress;
                }

                Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh2, float>(score / handles.Count);
            });
            var handle1 = component.CreateLoadSceneHandle(data.ScenePath);
            handles.Add(handle1);
            tasks.Add(handle1.ToUniTask(progress));

            for (int i = data.AssetPaths.Count - 1; i >= 0; i--)
            {
                var info = data.AssetPaths[i];

                if (info.Type == LoadType.Normal)
                {
                    var handle = component.CreateLoadHandle(typeof(Object), info.Path);
                    handles.Add(handle);
                    tasks.Add(handle.ToUniTask(progress));
                }
                else if (info.Type == LoadType.Sub)
                {
                    var handle = component.CreateLoadSubHandle(typeof(Object), info.Path);
                    handles.Add(handle);
                    tasks.Add(handle.ToUniTask(progress));
                }
                else if (info.Type == LoadType.RawFile)
                {
                    var handle = component.CreateLoadRawFileHandle(info.Path);
                    handles.Add(handle);
                    tasks.Add(handle.ToUniTask(progress));
                }
            }

            await UniTask.WhenAll(tasks);
            tasks.Clear();

            for (int i = data.AssetPaths.Count - 1; i >= 0; i--)
            {
                var info = data.AssetPaths[i];

                if (info.Type == LoadType.Normal && info.TypeName == typeof(AssetReferenceSettings).FullName)
                {
                    tasks.Add(component.LoadAssetReferenceSettingsAsync(info.Path));
                }
            }

            tasks.AddRange(component.SaveRawFileList(data.AssetPaths));

            await UniTask.WhenAll(tasks);

            NLog.Log.Debug("资源加载完成------------------------11----");

            ObjectHelper.CreateComponent<GameRootComponent>(ObjectHelper.CreateEntity<Entity>(Game.Instance.Scene, GameObject.Find("GameRoot")));

            if (data.Call is not null)
            {
                await data.Call();
            }

            ResHelper.GCAndUnload();

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