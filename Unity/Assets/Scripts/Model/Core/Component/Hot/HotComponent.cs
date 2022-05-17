using Cysharp.Threading.Tasks;
using System;
using YooAsset;

namespace Model
{
    public class HotComponent : Component, IAwake
    {
        public static int DOWNLOADING_MAX_NUM = 10;
        public static int FAILED_TRY_AGAIN = 3;

        public void Awake()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public async UniTask Run()
        {
            await UpdateStaticVersion();
        }

        /// <summary>
        /// 获取资源版本
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateStaticVersion()
        {
            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.UpdateStaticVersion);
            UpdateStaticVersionOperation operation = YooAssets.UpdateStaticVersionAsync();
            await operation.ToUniTask();

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                int resourceVersion = operation.ResourceVersion;
                NLog.Log.Debug($"获取资源版本 : {resourceVersion}，完成");
                await UniTask.Delay(500);
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
            await operation.ToUniTask();

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                NLog.Log.Debug($"更新补丁清单完成");
                await UniTask.Delay(500);
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
                await UniTask.Delay(200);
                Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh, int, int, long, long>(0, 0, 0, 0);
                await UniTask.Delay(500);
                Game.Instance.EventSystem.InvokeAsync<E_GameLoadComplete>();
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
                Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.DownloadHotAssetsSuccess);
                Game.Instance.Scene.GetComponent<AssetsComponent>().ClearUnusedCacheFiles();
                await Game.Instance.Scene.GetComponent<AssetsComponent>()
                    .LoadAssetReferenceSettingsAsync(AssetsComponent.INIT_LOAD_CONFIG_PATH, true);
                await UniTask.Delay(500);
                Game.Instance.EventSystem.InvokeAsync<E_GameLoadComplete>();
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
            Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh, int, int, long, long>(totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes);
        }

        private void OnDownloadOver(bool isSucceed)
        {
        }
    }
}