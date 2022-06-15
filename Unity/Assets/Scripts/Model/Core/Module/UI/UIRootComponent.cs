using UnityEngine;

namespace Model
{
    public class UIRootComponent : Component, IAwake
    {
        public static string UIROOT_PATH = $"{FileValue.RES_PATH}Prefabs/UI/UIRoot";

        public Camera UICamera { private set; get; }

        public void Awake()
        {
            UICamera = this.Entity.Transform.Find("UICamera").GetComponent<Camera>();

            ObjectHelper.CreateComponent<UI2DRootComponent>(ObjectHelper.CreateEntity(Entity, Entity.Transform.Find(UI2DRootComponent.GAME_OBJECT_NAME).gameObject), false);

            Game.Instance.EventSystem.AddListener<E_SetMainCameraShow, bool>(this, OnSetMainCameraShow);
        }

        public override void Dispose()
        {
            UICamera = null;
            base.Dispose();
        }

        private void OnSetMainCameraShow(bool b)
        {
            if (b)
            {
                if (UICamera.clearFlags != CameraClearFlags.Depth)
                {
                    this.UICamera.clearFlags = CameraClearFlags.Depth;
                }
            }
            else
            {
                if (UICamera.clearFlags == CameraClearFlags.Depth)
                {
                    this.UICamera.clearFlags = CameraClearFlags.SolidColor;
                }
            }
        }
    }
}