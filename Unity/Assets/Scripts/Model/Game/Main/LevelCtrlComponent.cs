using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class LevelCtrlComponent : Component, IAwake, IUpdateSystem
    {
        public void Awake()
        {
            Game.Instance.EventSystem.AddListener<E_GamePause>(this, OnGamePause);
            Game.Instance.EventSystem.AddListener<E_GameContinue>(this, OnGameContinue);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {
        }

        private void OnGamePause()
        {
            Time.timeScale = 0;
            ObjectHelper.OpenUIView<PauseViewComponent>();
        }

        private void OnGameContinue()
        {
            Time.timeScale = 1;
            ObjectHelper.CloseUIView<PauseViewComponent>();
        }
    }
}