using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    [LifeCycle]
    [UIBaseData(UIViewType = UIViewType.None, PrefabPath = "Assets/Res/Prefabs/UIBlackMask", UIMaskMode = UIMaskMode.None)]
    public class UIBlackMaskComponent : UIBaseComponent, IOpen
    {
        private Button btnSelf;

        public override void Awake()
        {
            AddComponent();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            btnSelf = rc.Get<GameObject>("Image").GetComponent<Button>();

            btnSelf.onClick.AddListener(OnBtnSelfClick);
        }

        public override void Dispose()
        {
            Entity = null;
        }

        public void Open()
        {
            OnOpen();
        }

        protected override void OnOpen()
        {
        }

        protected override void OnClose()
        {
        }

        public override void Enable()
        {
        }

        public override void Disable()
        {
        }

        private void OnBtnSelfClick()
        {
            Game.Instance.EventSystem.Invoke<CloseUIViewEvent>();
        }

        public void SetMaskMode(UIMaskMode mode)
        {
            if (mode == UIMaskMode.Transparent)
            {
                SetMaskMode(0, false, true);
            }
            else if (mode == UIMaskMode.TransparentClick)
            {
                SetMaskMode(0, true, true);
            }
            else if (mode == UIMaskMode.TransparentPenetrate)
            {
                SetMaskMode(0, false, false);
            }
            else if (mode == UIMaskMode.BlackTransparent)
            {
                SetMaskMode(1, false, true);
            }
            else if (mode == UIMaskMode.BlackTransparentClick)
            {
                SetMaskMode(1, true, true);
            }
            else if (mode == UIMaskMode.BlackTransparentPenetrate)
            {
                SetMaskMode(1, false, false);
            }
        }

        public void SetMaskMode(float alpha, bool interactable, bool blocksRaycasts)
        {
            this.CanvasGroup.alpha = alpha;
            this.CanvasGroup.interactable = interactable;
            this.CanvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}