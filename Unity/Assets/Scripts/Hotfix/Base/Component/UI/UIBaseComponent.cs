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
            _canvas.enabled = true;
        }

        protected override void OnClose()
        {
            _OnClose();
        }

        public override void Enable()
        {
            _canvas.enabled = true;
            IsEnable = true;
        }

        public override void Disable()
        {
            _canvas.enabled = false;
            IsEnable = false;
        }
    }
}