using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Normal,
        PrefabPath = "Assets/Res/Prefabs/UI/SelectLevel/SelectView.prefab",
        UIMaskMode = (int)UIMaskMode.BlackTransparentClick,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class SelectViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private VerticalScrollExtend scroll;
        private Button btnReturn;

        public override void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            scroll = this.Entity.GameObject.GetComponent<VerticalScrollExtend>();
            scroll.AddScrollListener(100, RefreshBox);

            btnReturn = rc.Get<GameObject>("btnReturn").GetComponent<Button>();
            btnReturn.onClick.AddListener(OnReturn);
        }

        private void RefreshBox(int idx, GameObject box)
        {
            box.GetComponentInChildren<Text>().text = (idx + 1).ToString();
            box.transform.Find("imgLock").gameObject.SetActive(0 < idx);
            var btnLevel = box.GetComponentInChildren<Button>();
            btnLevel.onClick.RemoveAllListeners();
            btnLevel.onClick.AddListener(() =>
            {
                if (idx <= 0)
                {
                    Game.Instance.EventSystem.Invoke<E_OpenLevel, int>(idx + 1);
                    //GameManagerComponent.instance.CreateLevel(idx + 1);
                }
                else if (idx == 1)
                {
                    UniTask.Void(async () =>
                    {
                        var tipsView = await ObjectHelper.OpenUIView<TipsViewComponent>();
                        string sTitle = "观看广告";
                        string sHint = "观看广告提前解锁此关";

                        tipsView.SetInfo(sTitle, sHint, "好的", "看你**", PlayGuangGao);
                    });
                }
                else
                {
                    NLog.Log.Debug("请先解锁第" + idx + "关");
                }
            });
        }

        private void OnReturn()
        {
            ObjectHelper.CloseUIView<SelectViewComponent>();
        }

        private void PlayGuangGao()
        {
            NLog.Log.Error("播放广告");
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