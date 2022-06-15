using Cysharp.Threading.Tasks;
using System;
using YooAsset;

namespace Model
{
    public class HotComponent : Component, IAwake
    {
        public static int DOWNLOADING_MAX_NUM = 10;
        public static int FAILED_TRY_AGAIN = 3;

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
            if (component.PlayMode == YooAsset.YooAssets.EPlayMode.HostPlayMode)
            {
                await UpdateStaticVersion();
            }
            else
            {
                await LoadHelper.LoadScene(data);
            }
        }

        /// <summary>
        /// 获取资源版本
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateStaticVersion()
        {
            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.UpdateStaticVersion);
            UpdateStaticVersionOperation operation = YooAssets.UpdateStaticVersionAsync();
            var progressCall = Progress.Create<float>(f =>
            {
                Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh2, float>(f);
            });
            await operation.ToUniTask(progressCall);
            progressCall.Report(1);

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                int resourceVersion = operation.ResourceVersion;
                NLog.Log.Debug($"获取资源版本 : {resourceVersion}，完成");
                await UniTask.Delay(1000);
                await UpdatePatchManifest(resourceVersion);
            }
            else
            {
                //更新失败
                NLog.Log.Error(operation.Error);
            }
        }

        /// <summary>
        /// 更新补丁清单
        /// </summary>
        /// <param name="resourceVersion"></param>
        /// <returns></returns>
        private async UniTask UpdatePatchManifest(int resourceVersion)
        {
            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.UpdatePatchManifest);
            UpdateManifestOperation operation = YooAssets.UpdateManifestAsync(resourceVersion);
            var progressCall = Progress.Create<float>(f =>
            {
                Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh2, float>(f);
            });
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
            DownloaderOperation downloader = YooAssets.CreatePatchDownloader(DOWNLOADING_MAX_NUM, FAILED_TRY_AGAIN);

            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {
                await LoadHelper.LoadScene(_loadSceneData);
                return;
            }
            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.DownloadHotAssets);

            //注册回调方法
            downloader.OnDownloadFileFailedCallback = OneDownloadFileFailed;
            downloader.OnDownloadProgressCallback = OnDownloadProgressUpdate;
            downloader.OnDownloadOverCallback = OnDownloadOver;

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
            }
        }

        private void OneDownloadFileFailed(string fileName)
        {
            DialogBoxInfo info = new DialogBoxInfo();
            info.title = "提示";
            info.content = "下载文件失败，是否重新下载？";
            info.btnCallList = new[] { (Action)(async () => await Download()), () => ObjectHelper.CloseUIView<DialogBoxViewComponent>() };
            info.btnTextList = new[] { "重试", "取消" };
            ObjectHelper.OpenUIView<DialogBoxViewComponent, DialogBoxInfo>(info);
        }

        private void OnDownloadProgressUpdate(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh1, int, int, long, long>(totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes);
        }

        private void OnDownloadOver(bool isSucceed)
        {
        }
    }
}