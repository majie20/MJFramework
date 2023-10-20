using UnityEngine;

namespace Model
{
    public class UIRootComponent : Component, IAwake
    {
        public Camera        UICamera;
        public AudioListener AudioListener;

        public void Awake()
        {
            Game.Instance.GAddComponent(this);
            var uiCamera_obj = this.Entity.Transform.Find("UICamera");
            AudioListener = uiCamera_obj.GetComponent<AudioListener>();
            UICamera = uiCamera_obj.GetComponent<Camera>();

            ObjectHelper.CreateComponent<UI2DRootComponent>(ObjectHelper.CreateEntity<Entity>(Entity, Entity.Transform.Find(UI2DRootComponent.GAME_OBJECT_NAME).gameObject), false);

            Game.Instance.EventSystem.AddListener<E_SetMainCameraShow, bool>(this, OnSetMainCameraShow);
        }

        public override void Dispose()
        {
            Game.Instance.GRemoveComponent(this);
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