using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Normal,
        PrefabPath = "Assets/Res/Prefabs/UI/Start/StartView.prefab",
        UIMaskMode = (int)UIMaskMode.BlackTransparentClick,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class StartViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private Button btnPlay;
        private Button btnMusic;
        private Button btnRole;

        public override void Awake()
        {
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            btnPlay = rc.Get<GameObject>("btnPlay").GetComponent<Button>();
            btnPlay.onClick.AddListener(OnPlay);

            btnMusic = rc.Get<GameObject>("btnMusic").GetComponent<Button>();
            btnMusic.onClick.AddListener(OnMusic);
            RefreshMusicState();
            base.Awake();
        }

        private void OnPlay()
        {
            ObjectHelper.OpenUIView<SelectViewComponent>();
        }

        private void OnMusic()
        {
            MusicCtrl.instance.RefreshAudioSwitch();
            RefreshMusicState();
        }

        private void RefreshMusicState()
        {
            NLog.Log.Debug("==========<这里有两段测试开关背景音乐代码");
            if (MusicCtrl.instance.audioSwitch)
            {
                btnMusic.GetComponent<Image>().sprite = Game.Instance.Scene.GetComponent<SpriteComponent>().LoadSprite("MUSIC-ON", "S_Start");
                Game.Instance.EventSystem.Invoke<E_SetMusicValue,float>(1);
            }
            else
            {
                btnMusic.GetComponent<Image>().sprite = Game.Instance.Scene.GetComponent<SpriteComponent>().LoadSprite("MUSIC-OFF", "S_Start");
                Game.Instance.EventSystem.Invoke<E_SetMusicValue, float>(0);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Open()
        {
            OnOpen();
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}