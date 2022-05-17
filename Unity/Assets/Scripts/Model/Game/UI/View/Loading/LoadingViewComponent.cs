using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [UIBaseData(UIViewType = (int)UIViewType.Normal,
        PrefabPath = "Assets/Res/Prefabs/UI/Loading/LoadingView.prefab",
        UIMaskMode = (int)UIMaskMode.Transparent,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class LoadingViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private LoadProgressType _loadProgressType;
        private Text TextProgress;
        private Slider SliderProgress;
        private string _hintText;

        public override void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            TextProgress = rc.Get<GameObject>("TextProgress").GetComponent<Text>();
            SliderProgress = rc.Get<GameObject>("SliderProgress").GetComponent<Slider>();
        }

        public override void Dispose()
        {
            TextProgress = null;
            base.Dispose();
        }

        public async void Open()
        {
            OnOpen();
            _hintText = "";
            _loadProgressType = LoadProgressType.None;
            SliderProgress.value = 0;

            Game.Instance.EventSystem.AddListener<E_LoadStateSwitch, LoadProgressType>(this, OnLoadStateSwitch);
            Game.Instance.EventSystem.AddListener<E_LoadingViewProgressRefresh, int, int, long, long>(this, OnLoadingViewProgressRefresh);

            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            await Game.Instance.Scene.GetComponent<HotComponent>().Run();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        private void OnLoadingViewProgressRefresh(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            var value = totalDownloadBytes == 0 ? 1 : currentDownloadBytes / totalDownloadBytes;
            SliderProgress.value = value;
            TextProgress.text = $"{_hintText}：{currentDownloadCount}/{totalDownloadCount},{value * 100:F2}%";
        }

        private void OnLoadStateSwitch(LoadProgressType type)
        {
            _loadProgressType = type;
            switch (type)
            {
                case LoadProgressType.UpdateStaticVersion:
                    _hintText = "正在更新资源版本号";
                    NLog.Log.Debug("正在更新资源版本号"); // MDEBUG:
                    break;

                case LoadProgressType.UpdatePatchManifest:
                    _hintText = "正在更新资源清单";
                    NLog.Log.Debug("正在更新资源清单"); // MDEBUG:
                    break;

                case LoadProgressType.DownloadHotAssets:
                    _hintText = "正在下载资源";
                    NLog.Log.Debug("正在下载资源"); // MDEBUG:
                    break;

                case LoadProgressType.DownloadHotAssetsSuccess:
                    _hintText = "下载资源完成";
                    NLog.Log.Debug("下载资源完成"); // MDEBUG:
                    break;
            }
            TextProgress.text = $"{_hintText}";
        }
    }
}