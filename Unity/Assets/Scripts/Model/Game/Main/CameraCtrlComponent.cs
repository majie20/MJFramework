using UnityEngine;

namespace Model
{
    public class CameraCtrlComponent : Component, IAwake
    {
        private Camera _mainCamera;
        private int    _maskLayer;

        public void Awake()
        {
            var mainCamera = Entity.Transform.Find("Main Camera");

            this._mainCamera = mainCamera.GetComponent<Camera>();

            this._maskLayer = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "Environment", "Player", "Monster", "PlayerWeapon", "MonsterWeapon", "Area",
                "Trigger");

            OnSetMainCameraShow(Game.Instance.Scene.GetComponent<GamePlayDataComponent>().IsShowMainCamera);
            Game.Instance.EventSystem.AddListener<E_SetMainCameraShow, bool>(this, OnSetMainCameraShow);
        }

        private void OnSetMainCameraShow(bool b)
        {
            if (b)
            {
                _mainCamera.cullingMask = this._maskLayer;
            }
            else
            {
                _mainCamera.cullingMask = 0;
            }
        }
    }
}