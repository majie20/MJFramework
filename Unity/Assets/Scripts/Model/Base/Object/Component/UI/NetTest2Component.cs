using UnityEngine;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Normal, PrefabPath = "Assets/Res/Prefabs/NetTest 2", UIMaskMode = (int)UIMaskMode.BlackTransparentClick)]
    public class NetTest2Component : UIBaseComponent, IOpen, IAwake
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

        protected override void OnOpen()
        {
            base.OnOpen();
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