using cfg;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    /// <summary>
    /// 商店界面
    /// </summary>
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Normal,
        PrefabPath = "Assets/Res/Prefabs/UI/Store/StoreView.prefab",
        UIMaskMode = (int)UIMaskMode.BlackTransparentClick,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class StoreViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private Button btnClose;
        private GameObject Content;
        private GameObject Item;

        public void Awake()
        {
            base.Awake();
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            btnClose = rc.Get<GameObject>("btnClose").GetComponent<Button>();
            btnClose.onClick.AddListener(() =>
            {
                ObjectHelper.CloseUIView<StoreViewComponent>();
            });
            Content = rc.Get<GameObject>("Content");
            Item = rc.Get<GameObject>("Item");
            Item.SetActive(false);
            InitScreen();
        }

        public void Open()
        {
            OnOpen();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void InitScreen()
        {
            for (int i = 0; i < Content.transform.childCount; i++)
            {
                Content.transform.GetChild(i).gameObject.SetActive(false);
            }
            roleData data = Game.Instance.Scene.GetComponent<GameConfigDataComponent>().JsonTables.roleData;
            if (data != null && data.DataList.Count > 0)
            {
                for (var i = 0; i < data.DataList.Count; i++)
                {
                    role role = data.DataList[i];
                    GameObject item = null;
                    if (i + 1 >= Content.transform.childCount)
                    {
                        item = GameObject.Instantiate(Item);
                        item.transform.SetParent(Content.transform);
                    }
                    else
                    {
                        item = Content.transform.GetChild(i + 1).gameObject;
                    }

                    StoreItem StoreItem = new StoreItem(item, role);
                    item.SetActive(true);
                    item.name = role.Id.ToString();
                }
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}