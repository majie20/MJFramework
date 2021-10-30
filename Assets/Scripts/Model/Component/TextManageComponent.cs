using System.Collections.Generic;

namespace MGame.Model
{
    [LifeCycle]
    public class TextManageComponent : Component, IAwake
    {
        private Dictionary<string, UnityEngine.TextAsset> textDataDic;

        public void Awake()
        {
            textDataDic = new Dictionary<string, UnityEngine.TextAsset>();

            Game.Instance.EventSystem.AddListener<AssetBundleLoadComplete>(OnAssetBundleLoadComplete, this);
        }

        public override void Dispose()
        {
            base.Dispose();

            textDataDic = null;

            Game.Instance.EventSystem.RemoveListener<AssetBundleLoadComplete>(this);
        }

        public UnityEngine.TextAsset GetTextAssetByName(string name)
        {
            return textDataDic[name];
        }

        private void OnAssetBundleLoadComplete()
        {
            var collectors = Game.Instance.Scene.GetComponent<ABComponent>().GetAllTextDataCollector();

            foreach (var collector in collectors)
            {
                for (int i = 0; i < collector.data.Count; i++)
                {
                    var data = collector.data[i];
                    if (!textDataDic.ContainsKey(data.key))
                    {
                        textDataDic.Add(data.key, data.gameObject as UnityEngine.TextAsset);
                    }
                }
            }

            Game.Instance.EventSystem.Invoke2(EventType.GameLoadComplete, EventType.TextDataLoadComplete);
        }
    }
}