namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = UIViewType.Normal, PrefabPath = "Assets/Res/Prefabs/NetTest", UIMaskMode = UIMaskMode.BlackTransparent)]
    public class NetTestComponent : UIBaseComponent, IOpen
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Open()
        {
            base.Open();
        }

        public override void OnOpen()
        {
            base.OnOpen();
        }

        public override void Close()
        {
            base.Close();
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        protected override void OnUIEnable()
        {
        }

        protected override void OnUIDisable()
        {
        }

        protected override void OnUIDestroy()
        {
        }
    }
}