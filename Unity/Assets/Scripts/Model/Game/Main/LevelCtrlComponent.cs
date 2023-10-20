using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class LevelCtrlComponent : Component, IAwake
    {
        public void Awake()
        {
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();

            var environmentCollector = rc.Get<GameObject>("Environment").GetComponent<EnvironmentManager>();

            GameRootComponent gameRootComponent = Entity.GetComponent<GameRootComponent>();
            GameConfigDataComponent gameConfigDataComponent = Game.Instance.Scene.GetComponent<GameConfigDataComponent>();
            var code = Game.Instance.Scene.GetComponent<GamePlayDataComponent>().GetGameData<int>(GameDataKey.CUR_PLAYER_TYPE_CODE);
            var cfg = gameConfigDataComponent.JsonTables.TbRole.Get(code);

            var player = Game.Instance.Scene.GetComponent<UnitDataComponent>().CreateUnit(cfg.TypeCode, Entity, null, environmentCollector.BornPoint);
            player.EventSystem.Invoke<E_UnitDataInitialize, int>(code);
            gameRootComponent.Player = player;

            Game.Instance.EventSystem.Invoke<E_SetMainCameraFollowTarget, Transform>(player.Transform);

            for (int i = environmentCollector.Areas.Length - 1; i >= 0; i--)
            {
                var area = environmentCollector.Areas[i];
                ObjectHelper.CreateComponent<AreaManagerComponent>(ObjectHelper.CreateEntity<EventEntity>(Entity, area.gameObject));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}