using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    [LifeCycle]
    [UIBaseData(UIViewType = Model.UIViewType.None, PrefabPath = "Assets/Res/Prefabs/UIBlackMask", UIMaskMode = Model.UIMaskMode.None)]
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

        public override void Open()
        {
        }

        public override void OnOpen()
        {
        }

        public override void Close()
        {
        }

        public override void OnClose()
        {
        }

        private void OnBtnSelfClick()
        {
        }

        public void SetMaskMode(Model.UIMaskMode mode)
        {
            if (mode == Model.UIMaskMode.Transparent)
            {
                SetMaskMode(0, false, true);
            }
            else if (mode == Model.UIMaskMode.TransparentClick)
            {
                SetMaskMode(0, true, true);
            }
            else if (mode == Model.UIMaskMode.TransparentPenetrate)
            {
                SetMaskMode(0, false, false);
            }
            else if (mode == Model.UIMaskMode.BlackTransparent)
            {
                SetMaskMode(1, false, true);
            }
            else if (mode == Model.UIMaskMode.BlackTransparentClick)
            {
                SetMaskMode(1, true, true);
            }
            else if (mode == Model.UIMaskMode.BlackTransparentPenetrate)
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