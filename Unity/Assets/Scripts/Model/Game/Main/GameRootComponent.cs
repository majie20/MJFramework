using UnityEngine;

namespace Model
{
    public class GameRootComponent : Component, IAwake
    {
        public Entity Player;

        public void Awake()
        {
            Game.Instance.GAddComponent(this);

            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();

            //var vcamList = rc.Get<GameObject>("VcamList");
            //var mainCamera = rc.Get<GameObject>("Main Camera");
            //ObjectHelper.CreateComponent<VCameraCtrlComponent>(ObjectHelper.CreateEntity<Entity>(Entity, vcamList));
            //ObjectHelper.CreateComponent<CameraCtrlComponent>(ObjectHelper.CreateEntity<Entity>(Entity, mainCamera));

            var camera = ObjectHelper.CreateEntity<Entity>(Entity, rc.Get<GameObject>("MainCamera"));
            ObjectHelper.CreateComponent<VCameraCtrlComponent>(camera);
            ObjectHelper.CreateComponent<CameraCtrlComponent>(camera);
        }

        public override void Dispose()
        {
            Game.Instance.GRemoveComponent(this);

            base.Dispose();
        }
    }
}