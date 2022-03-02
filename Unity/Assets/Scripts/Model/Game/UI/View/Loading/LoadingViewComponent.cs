using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [UIBaseData(UIViewType = (int)UIViewType.Normal, PrefabPath = "Assets/Res/NoBuildAB/Prefabs/UI/Loading/LoadingView", UIMaskMode = (int)UIMaskMode.Transparent)]
    public class LoadingViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private LoadType loadType;
        private Text TextProgress;

        public override void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            TextProgress = rc.Get<GameObject>("TextProgress").GetComponent<Text>();
        }

        public override void Dispose()
        {
            base.Dispose();
            TextProgress = null;
        }

        public void Open()
        {
            OnOpen();
            loadType = LoadType.None;
#if UNITY_EDITOR
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            if (component.IsUseABPackPlay)
            {
                Game.Instance.Scene.GetComponent<HotComponent>().Run(false);
            }
            else
            {
                component.Run();
            }
#else
            Game.Instance.Scene.GetComponent<HotComponent>().Run(false);
#endif
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Game.Instance.EventSystem.AddListener<LoadStateSwitch, LoadType>(OnLoadStateSwitch, this);
            Game.Instance.EventSystem.AddListener<LoadingViewProgressRefresh, float>(OnLoadingViewProgressRefresh, this);
        }

        protected override void OnClose()
        {
            base.OnClose();
            Game.Instance.EventSystem.AddListener<LoadingViewProgressRefresh, float>(OnLoadingViewProgressRefresh, this);
        }

        private void OnLoadingViewProgressRefresh(float num)
        {
            TextProgress.text = $"{num * 100:F2}%";
        }

        private void OnLoadStateSwitch(LoadType type)
        {
            loadType = type;
            switch (type)
            {
                case LoadType.LoadAssetsConfig:
                    Debug.LogWarning("正在加载资源配置"); // MDEBUG:
                    break;

                case LoadType.LoadAssetsConfigComplete:
                    Debug.LogWarning("加载资源配置完成"); // MDEBUG:
                    break;

                case LoadType.CheckAssetsUpdate:
                    Debug.LogWarning("正在检查资源更新"); // MDEBUG:
                    break;

                case LoadType.DownloadHotAssets:
                    Debug.LogWarning("正在更新资源"); // MDEBUG:
                    break;

                case LoadType.DownloadHotAssetsComplete:
                    Debug.LogWarning("更新资源完成"); // MDEBUG:
                    break;

                case LoadType.UnzipAssets:
                    Debug.LogWarning("正在解压资源"); // MDEBUG:
                    break;

                case LoadType.UnzipAssetsComplete:
                    Debug.LogWarning("正在解压资源完成"); // MDEBUG:
                    break;

                case LoadType.LoadAssets:
                    Debug.LogWarning("正在加载资源"); // MDEBUG:
                    break;

                case LoadType.LoadAssetsComplete:
                    Debug.LogWarning("加载资源完成"); // MDEBUG:
                    break;
            }
        }
    }
}