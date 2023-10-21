using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    public enum LoadProgressType
    {
        None,
        UpdateStaticVersion,
        UpdatePatchManifest,
        DownloadHotAssets,
        LoadAssets,
    }

    public enum LoadUseType
    {
        Hot,
        Normal,
    }

    [UIBaseData(UIViewType = (int)UIViewType.Normal, PrefabPath = "Assets/Res/UI/Prefab/Loading/LoadingView.prefab", AtlasPath = "Assets/Res/UI/Texture/StaticSprite/S_Loading",
        UIMaskMode = (int)UIMaskMode.Transparent, UILayer = (int)Model.UIViewLayer.Highest, IsOperateMask = false, IsFullScreen = true)]
    public class LoadingViewComponent : UIBaseComponent, IOpen<LoadUseType, LoadSceneData>
    {
        private LoadProgressType _loadProgressType;
        private Text             TextProgress;
        private Slider           SliderProgress;
        private string           _hintText;

        private LoadUseType   _loadUseType;
        private LoadSceneData _loadSceneData;

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
            SliderProgress = null;
            base.Dispose();
        }

        public override async UniTaskVoid OnLoadComplete()
        {
            _hintText = "";
            _loadProgressType = LoadProgressType.None;
            SliderProgress.value = 0;

            Game.Instance.EventSystem.AddListener<E_LoadStateSwitch, LoadProgressType>(this, OnLoadStateSwitch);
            Game.Instance.EventSystem.AddListener<E_LoadingViewProgressRefresh1, int, int, long, long>(this, OnLoadingViewProgressRefresh);
            Game.Instance.EventSystem.AddListener<E_LoadingViewProgressRefresh2, float>(this, OnLoadingViewProgressRefresh);

            if (_loadUseType == LoadUseType.Hot)
            {
                await Game.Instance.Scene.GetComponent<HotComponent>().Run(_loadSceneData);
            }
            else if (_loadUseType == LoadUseType.Normal)
            {
                await Game.Instance.Scene.GetComponent<GameManagerComponent>().Run(_loadSceneData);
            }
        }

        public void Open(LoadUseType useType, LoadSceneData data)
        {
            _loadUseType = useType;
            _loadSceneData = data;
        }

        protected override void OnClose()
        {
        }

        private void OnLoadingViewProgressRefresh(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            var value = (float)currentDownloadBytes / totalDownloadBytes;
            SliderProgress.value = value;
            TextProgress.text = $"{_hintText}：{(float)currentDownloadCount}/{totalDownloadCount},{value * 100:F2}%";
        }

        private void OnLoadingViewProgressRefresh(float value)
        {
            SliderProgress.value = value;
            TextProgress.text = $"{_hintText}：{value * 100:F2}%";
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

                case LoadProgressType.LoadAssets:
                    _hintText = "正在加载资源";
                    NLog.Log.Debug("正在加载资源"); // MDEBUG:

                    break;
            }

            TextProgress.text = $"{_hintText} : 0.0%";
        }
    }
}