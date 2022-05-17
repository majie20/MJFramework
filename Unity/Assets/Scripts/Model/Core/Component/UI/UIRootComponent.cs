using UnityEngine;

namespace Model
{
    public class UIRootComponent : Component, IAwake
    {
        public static string UIROOT_PATH = $"{FileValue.RES_PATH}Prefabs/UI/UIRoot";

        private Camera uiCamera;

        public Camera UICamera
        {
            private set
            {
                uiCamera = value;
            }
            get
            {
                return uiCamera;
            }
        }

        public void Awake()
        {
            UICamera = this.Entity.Transform.Find("UICamera").GetComponent<Camera>();

            ObjectHelper.CreateComponent<UI2DRootComponent>(ObjectHelper.CreateEntity(Entity, Entity.Transform.Find(UI2DRootComponent.GAME_OBJECT_NAME).gameObject), false);
        }

        public override void Dispose()
        {
            UICamera = null;
            base.Dispose();
        }
    }
}