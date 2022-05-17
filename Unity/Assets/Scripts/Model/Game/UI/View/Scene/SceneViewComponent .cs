using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    [UIBaseData(UIViewType = (int)UIViewType.Normal,
        PrefabPath = "Assets/Res/Prefabs/UI/Scene/SceneView.prefab",
        UIMaskMode = (int)UIMaskMode.TransparentPenetrate,
        UILayer = (int)Model.UIViewLayer.Normal)]
    public class SceneViewComponent : UIBaseComponent, IOpen, IAwake
    {
        private Button btnPause;
        private Button btnUse;

        public override void Awake()
        {
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();

            btnPause = rc.Get<GameObject>("btnPause").GetComponent<Button>();
            btnPause.onClick.AddListener(OnPause);

            btnUse = rc.Get<GameObject>("btnUse").GetComponent<Button>();
            base.Awake();
        }

        private void OnPause()
        {
            //GameManagerComponent.instance.PauseGame();
            Game.Instance.EventSystem.Invoke<E_GamePause>();
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