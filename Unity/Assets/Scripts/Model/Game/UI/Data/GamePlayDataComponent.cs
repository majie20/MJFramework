namespace Model
{
    public class GamePlayDataComponent : Component, IAwake
    {
        public bool IsShowMainCamera = true;

        public void Awake()
        {
            Game.Instance.EventSystem.AddListener<E_SetMainCameraShow, bool>(this, OnSetMainCameraShow);
        }

        private void OnSetMainCameraShow(bool b)
        {
            IsShowMainCamera = b;
        }
    }
}