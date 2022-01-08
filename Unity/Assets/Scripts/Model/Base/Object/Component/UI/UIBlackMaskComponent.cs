using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.None, PrefabPath = "Assets/Res/Prefabs/UIBlackMask", UIMaskMode = (int)UIMaskMode.None)]
    public class UIBlackMaskComponent : UIBaseComponent, IOpen, IAwake
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

        public void SetMaskMode(int mode)
        {
            if (mode == (int)UIMaskMode.Transparent)
            {
                SetMaskMode(0, false, true);
            }
            else if (mode == (int)UIMaskMode.TransparentClick)
            {
                SetMaskMode(0, true, true);
            }
            else if (mode == (int)UIMaskMode.TransparentPenetrate)
            {
                SetMaskMode(0, false, false);
            }
            else if (mode == (int)UIMaskMode.BlackTransparent)
            {
                SetMaskMode(1, false, true);
            }
            else if (mode == (int)UIMaskMode.BlackTransparentClick)
            {
                SetMaskMode(1, true, true);
            }
            else if (mode == (int)UIMaskMode.BlackTransparentPenetrate)
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