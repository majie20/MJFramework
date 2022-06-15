using UnityEngine;

namespace Model
{
    public class CameraCtrlComponent : Component, IAwake
    {
        private Camera _mainCamera;

        public void Awake()
        {
            this._mainCamera = this.Entity.GameObject.GetComponent<Camera>();
            this._mainCamera.enabled = Game.Instance.Scene.GetComponent<GamePlayDataComponent>().IsShowMainCamera;

            Game.Instance.EventSystem.AddListener<E_SetMainCameraShow, bool>(this, OnSetMainCameraShow);
        }

        private void OnSetMainCameraShow(bool b)
        {
            if (this._mainCamera.enabled != b)
            {
                this._mainCamera.enabled = b;
            }
        }
    }
}