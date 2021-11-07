using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    [LifeCycle]
    public class NetTestComponent : Component, IAwake, IUpdateSystem
    {
        private Transform content;
        private GameObject item;
        private InputField inputField;
        private Button button;
        public string message;

        public void Awake()
        {
            ReferenceCollector collector = Entity.GameObject.GetComponent<ReferenceCollector>();
            content = collector.Get<GameObject>("Content").transform;
            item = collector.Get<GameObject>("Item");
            inputField = collector.Get<GameObject>("InputField").GetComponent<InputField>();
            button = collector.Get<GameObject>("Button").GetComponent<Button>();

            button.onClick.AddListener(OnButtonClick);

            Model.Game.Instance.EventSystem.AddListener<Model.NetTestSend, string>(OnNetTestSend, this);
        }

        public void OnUpdate(float tick)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var obj = GameObject.Instantiate(item, content);
                obj.SetActive(true);
                var textName = obj.transform.Find("TextName").GetComponent<Text>();
                var textContent = obj.transform.Find("TextContent").GetComponent<Text>();
                var ipEndPoint = Model.Game.Instance.Scene.GetComponent<Model.NetComponent>().IpEndPoint;
                textName.text = $"{ipEndPoint.Address}:{ipEndPoint.Port}";
                textContent.text = message;
                message = null;
                LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            }
        }

        private void OnButtonClick()
        {
            if (inputField.text.Length > 0)
            {
                Debug.Log("OnButtonClick"); // MDEBUG:
                Model.Game.Instance.Scene.GetComponent<Model.NetComponent>().Send(inputField.text);
            }
        }

        private void OnNetTestSend(string message)
        {
            this.message = message;
        }
    }
}