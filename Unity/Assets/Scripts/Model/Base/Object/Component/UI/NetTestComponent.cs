using UnityEngine;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = UIViewType.Normal, PrefabPath = "Assets/Res/Prefabs/NetTest", UIMaskMode = UIMaskMode.BlackTransparentClick)]
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