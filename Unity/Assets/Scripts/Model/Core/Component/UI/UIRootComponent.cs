using UnityEngine;

namespace Model
{
    public class UIRootComponent : Component, IAwake
    {
        public static string UIROOT_PATH = $"{FileValue.NO_BUILD_AB_RES_PATH}Prefabs/UI/UIRoot";

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

            Game.Instance.AddComponent(this);
        }

        public override void Dispose()
        {
            Entity = null;
            UICamera = null;
        }
    }
}