using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class GameManagerComponent : Component, IAwake, IUpdateSystem
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

        public void OnUpdate(float tick)
        {
        }

        public async UniTask Run(LoadSceneData data)
        {
            await LoadHelper.LoadScene(data);
        }

        private async UniTask OpenLevelCall()
        {
            await ObjectHelper.OpenUIView<SceneViewComponent>();
            await ObjectHelper.OpenUIView<VirtualJoystickViewComponent>();

            var gameRoot = Game.Instance.Scene.GetChild("GameRoot");

            ObjectHelper.CreateComponent<LevelCtrlComponent>(ObjectHelper.CreateEntity(gameRoot, GameObject.Find("GameRoot/Main Camera")));

            ObjectHelper.SetMainCamera();

            var player = await ObjectHelper.CreateEntity(gameRoot, null,
               "Assets/Res/Prefabs/Model/Character/KoalaModel.prefab", true, true, false,
               typeof(CharacterMovementComponent), typeof(PlayerComponent), typeof(CharacterColliderComponent), typeof(CharacterJumpComponent));

            Game.Instance.EventSystem.Invoke<E_SetMainCameraFollowTarget, Transform>(player.Transform);
        }

        private void OnOpenLevel(int level)
        {
            _curLevelCode = level;
            UniTask.Void(async () =>
            {
                LoadSceneData data = new LoadSceneData()
                {
                    ScenePath = $"Assets/Res/Scenes/Level/Level{_curLevelCode}.unity",
                    SettingsPath = $"Assets/Res/Config/ScriptableObject/AssetReference/Level{_curLevelCode}_ARS.asset",
                    Call = OpenLevelCall
                };
                await ObjectHelper.OpenUIView<LoadingViewComponent, LoadUseType, LoadSceneData>(LoadUseType.Normal, data);
            });
        }

        private void OnExitLevel()
        {
            Time.timeScale = 1;

            UniTask.Void(async () =>
            {
                LoadSceneData data = new LoadSceneData()
                {
                    ScenePath = $"Assets/Res/Scenes/Main.unity",
                    Call = ExitLevelCall
                };
                await ObjectHelper.OpenUIView<LoadingViewComponent, LoadUseType, LoadSceneData>(LoadUseType.Normal, data);
            });
        }

        private async UniTask ExitLevelCall()
        {
            ObjectHelper.SetMainCamera();

            await ObjectHelper.OpenUIView<SelectViewComponent>();
        }
    }
}