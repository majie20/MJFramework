using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class GameManagerComponent : Component, IAwake, IUpdateSystem
    {
        private bool gameState;
        private CharacterComponent Player;
        private LevelCtrlComponent LevelCtrlComponent;

        public void Awake()
        {
            Game.Instance.EventSystem.AddListener<E_OpenLevel, int>(this, OnOpenLevel);
            Game.Instance.EventSystem.AddListener<E_ExitLevel>(this, OnExitLevel);
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

        private void OnOpenLevel(int level)
        {
            UniTask.Void(async () =>
            {
                this.LevelCtrlComponent = ObjectHelper.CreateComponent<LevelCtrlComponent>(
                    await ObjectHelper.CreateEntity(Game.Instance.Scene, null,
                        $"Assets/Res/Prefabs/Level/Level{level}.prefab", true));
                var player = await ObjectHelper.CreateEntity(Game.Instance.Scene, null,
                    "Assets/Res/Prefabs/Model/Character/KoalaModel.prefab", true, typeof(CharacterComponent), typeof(CharacterMovementComponent), typeof(CharacterCtrlComponent), typeof(CharacterColliderComponent));
                player.GameObject.name = "player";
                this.Player = player.GetComponent<CharacterComponent>();

                ObjectHelper.OpenUIView<SceneViewComponent>();
                ObjectHelper.OpenUIView<VirtualJoystickViewComponent>();

                gameState = true;
                Game.Instance.EventSystem.Invoke<E_LevelLoadFinish>();
            });
        }

        private void OnExitLevel()
        {
            ObjectHelper.RemoveEntity(this.Player.Entity);
            ObjectHelper.RemoveEntity(this.LevelCtrlComponent.Entity);
            this.Player = null;
            this.LevelCtrlComponent = null;
            Time.timeScale = 1;
            ObjectHelper.CloseUIView<PauseViewComponent>();
            ObjectHelper.CloseUIView<SceneViewComponent>();
            ObjectHelper.CloseUIView<VirtualJoystickViewComponent>();
        }

        private void OnGamePause()
        {
            gameState = false;
            Time.timeScale = 0;
            ObjectHelper.OpenUIView<PauseViewComponent>();
        }

        private void OnGameContinue()
        {
            gameState = true;
            Time.timeScale = 1;
            ObjectHelper.CloseUIView<PauseViewComponent>();
        }
    }
}