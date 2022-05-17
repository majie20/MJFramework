using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [UIBaseData(UIViewType = (int)UIViewType.None,
        PrefabPath = "Assets/Res/Prefabs/UI/UIBlackMask.prefab",
        UIMaskMode = (int)UIMaskMode.None,
        UILayer = (int)UIViewLayer.None,
        IsOperateMask = false)]
    public class UIBlackMaskComponent : UIBaseComponent, IOpen, IAwake
    {
        private Button btnSelf;

        public override void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            btnSelf = rc.Get<GameObject>("Image").GetComponent<Button>();

            btnSelf.onClick.AddListener(OnBtnSelfClick);

            Game.Instance.EventSystem.AddListener<E_SetMaskModeEvent, int>(this, OnSetMaskMode);
            Game.Instance.EventSystem.AddListener<E_OnCloseUIBlackMaskEvent>(this, OnClose);
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
            Game.Instance.EventSystem.InvokeAsync<E_CloseUIViewEvent>();
        }

        public void OnSetMaskMode(int mode)
        {
            var v = (UIMaskMode)mode;
            if (v == UIMaskMode.Transparent)
            {
                SetMaskMode(0, false, true);
            }
            else if (v == UIMaskMode.TransparentClick)
            {
                SetMaskMode(0, true, true);
            }
            else if (v == UIMaskMode.TransparentPenetrate)
            {
                SetMaskMode(0, false, false);
            }
            else if (v == UIMaskMode.BlackTransparent)
            {
                SetMaskMode(1, false, true);
            }
            else if (v == UIMaskMode.BlackTransparentClick)
            {
                SetMaskMode(1, true, true);
            }
            else if (v == UIMaskMode.BlackTransparentPenetrate)
            {
                SetMaskMode(1, false, false);
            }
        }

        public void SetMaskMode(float alpha, bool interactable, bool blocksRaycasts)
        {
            this.canvasGroup.alpha = alpha;
            this.canvasGroup.interactable = interactable;
            this.canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}