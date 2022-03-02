using UnityEngine;

namespace Hotfix
{
    [Model.LifeCycle]
    [Model.UIBaseData(UIViewType = (int)Model.UIViewType.Normal, PrefabPath = "Assets/Res/BuildAB/Prefabs/NetTest", UIMaskMode = (int)Model.UIMaskMode.BlackTransparentClick)]
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