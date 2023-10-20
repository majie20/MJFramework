using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class GameManagerComponent : Component, IAwake
    {
        private int _curLevelCode;

        public void Awake()
        {
            Game.Instance.EventSystem.AddListener<E_OpenLevel, int>(this, OnOpenLevel);
            Game.Instance.EventSystem.AddListener<E_ExitLevel>(this, OnExitLevel);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public async UniTask Run(LoadSceneData data)
        {
            await LoadHelper.LoadScene(data);
        }

        private async UniTask OpenLevelCall()
        {
            await Game.Instance.Scene.GetComponent<UnitDataComponent>().LoadPlayerReferenceRes("Slime01");
            await UIHelper.OpenUIView<VirtualJoystickViewComponent>();

            var gameRoot = Game.Instance.GGetComponent<GameRootComponent>().Entity;

            ObjectHelper.CreateComponent<LevelCtrlComponent>(gameRoot);
        }

        private async UniTask ExitLevelCall()
        {
        }

        private void OnOpenLevel(int level)
        {
            _curLevelCode = level;

            UniTask.Void(async () =>
            {
                LoadSceneData data = new LoadSceneData()
                {
                    ScenePath = $"{ConstData.ENVIRONMENT_SCENE_PATH}Level{_curLevelCode}.unity",
                    SettingsPath = new List<string>() {$"{ConstData.ENVIRONMENT_LEVEL_PATH}Level{_curLevelCode}/Level{_curLevelCode}_ARS.asset"},
                    Call = OpenLevelCall
                };
                await UIHelper.OpenUIView<LoadingViewComponent, LoadUseType, LoadSceneData>(LoadUseType.Normal, data);
            });
        }

        private void OnExitLevel()
        {
            Time.timeScale = 1;

            UniTask.Void(async () =>
            {
                LoadSceneData data = new LoadSceneData() {ScenePath = ConstData.MAIN_SCENE, Call = ExitLevelCall};
                await UIHelper.OpenUIView<LoadingViewComponent, LoadUseType, LoadSceneData>(LoadUseType.Normal, data);
            });
        }
    }
}