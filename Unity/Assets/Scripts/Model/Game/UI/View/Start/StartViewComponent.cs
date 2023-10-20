using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Normal, PrefabPath = "Assets/Res/UI/Prefab/Start/StartView.prefab", AtlasPath = "Assets/Res/UI/Texture/StaticSprite/S_Start",
        UIMaskMode = (int)UIMaskMode.BlackTransparentClick, UILayer = (int)Model.UIViewLayer.Normal, IsFullScreen = true, IsOperateMask = false)]
    public class StartViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private Button btnPlay;

        public override void Awake()
        {
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            btnPlay = rc.Get<GameObject>("btnPlay").GetComponent<Button>();
            btnPlay.onClick.AddListener(OnPlay);

            RefreshMusicState();
            base.Awake();
        }

        private void OnPlay()
        {
            Game.Instance.EventSystem.Invoke<E_OpenLevel, int>(1);

            //AsyncTimerHelper.TimeFrameHandle((() =>
            //{
            //    UIHelper.OpenUIView<MainViewComponent>().Forget();
            //    UIHelper.CloseUIView<MainViewComponent>();
            //}),1, -1);
        }

        private void RefreshMusicState()
        {
            //NLog.Log.Debug("==========<这里有两段测试开关背景音乐代码");
            //if (MusicCtrl.instance.audioSwitch)
            //{
            //    btnMusic.GetComponent<Image>().sprite = Game.Instance.Scene.GetComponent<SpriteComponent>().LoadSprite("MUSIC-ON", "S_Start");
            //    Game.Instance.EventSystem.Invoke<E_SetMusicValue,float>(1);
            //}
            //else
            //{
            //    btnMusic.GetComponent<Image>().sprite = Game.Instance.Scene.GetComponent<SpriteComponent>().LoadSprite("MUSIC-OFF", "S_Start");
            //    Game.Instance.EventSystem.Invoke<E_SetMusicValue, float>(0);
            //}
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Open()
        {
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}