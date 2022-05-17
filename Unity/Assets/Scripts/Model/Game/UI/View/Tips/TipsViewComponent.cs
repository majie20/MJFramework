using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Pop,
        PrefabPath = "Assets/Res/Prefabs/UI/Tips/TipsView.prefab",
        UIMaskMode = (int)UIMaskMode.BlackTransparentClick,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class TipsViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private Text textTitle;
        private Text textHint;
        private Button btnConfirm;
        private Button btnCancel;

        public override void Awake()
        {
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();

            textTitle = rc.Get<GameObject>("textTitle").GetComponent<Text>();
            textHint = rc.Get<GameObject>("textHint").GetComponent<Text>();
            btnConfirm = rc.Get<GameObject>("btnConfirm").GetComponent<Button>();
            btnCancel = rc.Get<GameObject>("btnCancel").GetComponent<Button>();
            base.Awake();
        }

        public void SetInfo(string sTitle, string sHint, string sConfirm = null, string sCancel = null, UnityAction confirmFunc = null, UnityAction cancelFunc = null)
        {
            textTitle.text = sTitle;
            textHint.text = sHint;

            btnCancel.gameObject.SetActive(sCancel != null);
            btnConfirm.gameObject.SetActive(sConfirm != null);
            if (sConfirm != null)
            {
                btnConfirm.GetComponentInChildren<Text>().text = sConfirm;
            }

            if (sCancel != null)
            {
                btnCancel.GetComponentInChildren<Text>().text = sCancel;
            }

            btnConfirm.onClick.RemoveAllListeners();
            if (confirmFunc != null)
            {
                btnConfirm.onClick.AddListener(confirmFunc);
            }

            btnCancel.onClick.RemoveAllListeners();
            if (cancelFunc != null)
            {
                btnCancel.onClick.AddListener(cancelFunc);
            }
            else
            {
                btnCancel.onClick.AddListener(() =>
                {
                    ObjectHelper.CloseUIView<TipsViewComponent>();
                });
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Open()
        {
            OnOpen();
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}