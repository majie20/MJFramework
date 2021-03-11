using System.Collections.Generic;

namespace MGame.Model
{
    public class TextManageComponent : Component
    {
        private Dictionary<string, UnityEngine.TextAsset> textDataDic;

        public TextManageComponent()
        {
        }

        public override Component Init()
        {
            base.Init();

            textDataDic = new Dictionary<string, UnityEngine.TextAsset>();

            Game.Instance.EventSystem.Add<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();

            textDataDic = null;

            Game.Instance.EventSystem.Remove<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
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

            Game.Instance.EventSystem.Run<TextDataLoadComplete>();
        }
    }
}