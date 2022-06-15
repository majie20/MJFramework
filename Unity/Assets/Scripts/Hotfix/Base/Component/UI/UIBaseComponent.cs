namespace Hotfix
{
    public class UIBaseComponent : Model.UIBaseComponent
    {
        public override void Awake()
        {
            _Awake();
        }

        public override void Dispose()
        {
            _Dispose();
        }

        protected override void OnOpen()
        {
            Canvas.enabled = true;
        }

        protected override void OnClose()
        {
            _OnClose();
        }

        public override void Enable()
        {
            Canvas.enabled = true;
            IsEnable = true;
        }

        public override void Disable()
        {
            Canvas.enabled = false;
            IsEnable = false;
        }
    }
}