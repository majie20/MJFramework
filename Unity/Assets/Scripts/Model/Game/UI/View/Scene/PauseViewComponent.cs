using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Normal,
        PrefabPath = "Assets/Res/Prefabs/UI/Scene/PauseView.prefab",
        UIMaskMode = (int)UIMaskMode.BlackTransparent,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class PauseViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private Button btnContinue;
        private Button btnMusic;
        private Button btnExit;

        public override void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();

            btnContinue = rc.Get<GameObject>("btnContinue").GetComponent<Button>();
            btnContinue.onClick.AddListener(OnContinue);

            btnMusic = rc.Get<GameObject>("btnMusic").GetComponent<Button>();
            btnMusic.onClick.AddListener(OnMusic);

            btnExit = rc.Get<GameObject>("btnExit").GetComponent<Button>();
            btnExit.onClick.AddListener(OnExit);

            RefreshMusicState();
        }

        private void OnContinue()
        {
            //GameManagerComponent.instance.ContinueGame();
            Game.Instance.EventSystem.Invoke<E_GameContinue>();
        }

        private void OnMusic()
        {
            MusicCtrl.instance.RefreshAudioSwitch();
            RefreshMusicState();
        }

        private void OnExit()
        {
            Game.Instance.EventSystem.Invoke<E_ExitLevel>();
            //GameManagerComponent.instance.ExitLevel();
        }

        private void RefreshMusicState()
        {
            if (MusicCtrl.instance.audioSwitch)
            {
                btnMusic.GetComponentInChildren<Text>().text = "音乐：开";
            }
            else
            {
                btnMusic.GetComponentInChildren<Text>().text = "音乐：关";
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