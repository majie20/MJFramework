namespace Hotfix
{
    [Model.LifeCycle]
    [Model.UIBaseData(UIViewType = (int)Model.UIViewType.Normal,
        PrefabPath = "Assets/Res/Prefabs/UI/Scene/PauseView.prefab",
        UIMaskMode = (int)Model.UIMaskMode.BlackTransparentClick,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class NetTestComponent : Model.UIBaseComponent, IOpen, IAwake
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Open()
        {
            OnOpen();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }
    }
}