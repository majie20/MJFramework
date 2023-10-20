using Cysharp.Threading.Tasks;
using System;
using YooAsset;

namespace Model
{
    public class HotComponent : Component, IAwake
    {
        public static int DOWNLOADING_MAX_NUM = 10;
        public static int FAILED_TRY_AGAIN    = 3;

        private LoadSceneData _loadSceneData;

        public void Awake()
        {
        }

        public override void Dispose()
        {
            _loadSceneData = null;
            base.Dispose();
        }

        public async UniTask Run(LoadSceneData data)
        {
            _loadSceneData = data;
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();

            if (component.PlayMode == EPlayMode.WebPlayMode)
            {
                await LoadHelper.LoadScene(data);
            }
            else
            {
                if (component.PlayMode == EPlayMode.HostPlayMode)
                {
                    await UpdateStaticVersion();
                }
                else
                {
                    await LoadHelper.LoadScene(data);
                }
            }
        }

        /// <summary>
        /// 获取资源版本
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateStaticVersion()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.UpdateStaticVersion);
            var package = YooAssets.GetPackage(component.ABSettings.PackageName);
            var operation = package.UpdatePackageVersionAsync(true, 30);
            var progressCall = Progress.Create<float>(f => { Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh2, float>(f); });
            await operation.ToUniTask(progressCall);
            progressCall.Report(1);

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                string packageVersion = operation.PackageVersion;
                NLog.Log.Debug($"获取资源版本 : {packageVersion}，完成");
                await UniTask.Delay(1000);
                await UpdatePatchManifest(packageVersion);
            }
            else
            {
                //更新失败
                NLog.Log.Error(operation.Error);

                // 如果获取远端资源版本失败，说明当前网络无连接。 在正常开始游戏之前，需要验证本地清单内容的完整性。
                string packageVersion = package.GetPackageVersion();
                var downloadContentOperation = package.PreDownloadContentAsync(packageVersion);
                await downloadContentOperation;

                if (operation.Status != EOperationStatus.Succeed)
                {
                    // 资源内容本地并不完整，需要提示玩家联网更新。
                    NLog.Log.Error("请检查本地网络，有新的游戏内容需要更新！");

                    return;
                }

                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                int timeout = 60;
                var downloader = downloadContentOperation.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);

                if (downloader.TotalDownloadCount > 0)
                {
                    // 资源内容本地并不完整，需要提示玩家联网更新。
                    NLog.Log.Error("请检查本地网络，有新的游戏内容需要更新！");

                    return;
                }

                await LoadHelper.LoadScene(_loadSceneData);
            }
        }

        /// <summary>
        /// 更新补丁清单
        /// </summary>
        /// <param name="resourceVersion"></param>
        /// <returns></returns>
        private async UniTask UpdatePatchManifest(string packageVersion)
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            //component.ForceUnloadAllAssets();
            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.UpdatePatchManifest);
            var operation = YooAssets.GetPackage(component.ABSettings.PackageName).UpdatePackageManifestAsync(packageVersion);
            var progressCall = Progress.Create<float>(f => { Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh2, float>(f); });
            await operation.ToUniTask(progressCall);
            progressCall.Report(1);

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                NLog.Log.Debug($"更新补丁清单完成");
                await UniTask.Delay(1000);
                await Download();
            }
            else
            {
                //更新失败
                NLog.Log.Error(operation.Error);
            }
        }

        /// <summary>
        /// 补丁包下载
        /// </summary>
        /// <returns></returns>
        private async UniTask Download()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            DownloaderOperation downloader = YooAssets.GetPackage(component.ABSettings.PackageName).CreateResourceDownloader(DOWNLOADING_MAX_NUM, FAILED_TRY_AGAIN);

            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {
                await LoadHelper.LoadScene(_loadSceneData);

                return;
            }

            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.DownloadHotAssets);

            //注册回调方法
            downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
            downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
            downloader.OnDownloadOverCallback = OnDownloadOverCallback;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileCallback;

            //开启下载
            downloader.BeginDownload();
            await downloader;

            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
            {
                //下载成功
                await UniTask.Delay(1000);
                await LoadHelper.LoadScene(_loadSceneData);
            }
            else
            {
                //下载失败
                Error();
            }
        }

        private void OnDownloadErrorCallback(string fileName, string error)
        {
            NLog.Log.Error($"FileName:{fileName}\nError:{error}");
        }

        private void Error()
        {
            DialogBoxInfo info = new DialogBoxInfo();
            info.title = "提示";
            info.content = "下载资源失败，是否重新下载？";
            info.btnCallList = new[] {(Action) (async () => await Download()), () => UIHelper.CloseUIView<DialogBoxViewComponent>()};
            info.btnTextList = new[] {"重试", "取消"};
            UIHelper.OpenUIView<DialogBoxViewComponent, DialogBoxInfo>(info).Forget();
        }

        private void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh1, int, int, long, long>(totalDownloadCount, currentDownloadCount, totalDownloadBytes,
                currentDownloadBytes);
        }

        private void OnDownloadOverCallback(bool isSucceed)
        {
        }

        private void OnStartDownloadFileCallback(string fileName, long sizeBytes)
        {
        }
    }
}