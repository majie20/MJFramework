using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MGame.Model
{
    /// <summary>
    /// 预制体位置数据
    /// </summary>
    public class PrefabPlaceData
    {
        public string name;
        public string guid;
        public string abName;
    }

    /// <summary>
    /// 预制体创建所需数据
    /// </summary>
    public class PrefabCreateData
    {
        public int index;
        public string name;
        public string guid;
        public string tag;
        public string layer;
        public string parentPath;
        public bool isDisplay;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale;
    }

    public class PrefabAssociateComponent : Component
    {
        /// <summary>
        /// Guid、预制体位置数据（在AB包中的位置）
        /// </summary>
        private Dictionary<string, PrefabPlaceData> prefabPlaceDic;

        /// <summary>
        /// 预制体名、预制体创建所需子物体的数据
        /// </summary>
        private Dictionary<string, List<PrefabCreateData>> prefabCreateDic;

        public PrefabAssociateComponent()
        {
        }

        public override Component Init()
        {
            base.Init();

            prefabPlaceDic = new Dictionary<string, PrefabPlaceData>();
            prefabCreateDic = new Dictionary<string, List<PrefabCreateData>>();

            Game.Instance.EventSystem.AddListener<AssetBundleLoadComplete>(OnAssetBundleLoadComplete, this);

            return this;
        }

        public override void Dispose()
        {
            base.Dispose();

            prefabPlaceDic = null;
            prefabCreateDic = null;

            Game.Instance.EventSystem.RemoveListener<AssetBundleLoadComplete>(this);
        }

        private void OnAssetBundleLoadComplete()
        {
            var collectors = Game.Instance.Scene.GetComponent<ABComponent>().GetAllPrefabJsonDataCollector();
            foreach (var collector in collectors)
            {
                for (int i = 0; i < collector.data.Count; i++)
                {
                    using (var sr = new StringReader(collector.Get<TextAsset>(collector.data[i].key).text))
                    {
                        var reader = new JsonTextReader(sr);
                        var jsonDatas = JToken.ReadFrom(reader);
                        var pcDatas = new List<PrefabCreateData>();
                        for (int j = 0; j < jsonDatas.Count(); j++)
                        {
                            var jsonObj = jsonDatas[j];
                            var ppData = new PrefabPlaceData
                            {
                                name = jsonObj["name"].ToString(),
                                guid = jsonObj["guid"].ToString(),
                                abName = jsonObj["abName"].ToString()
                            };
                            if (!prefabPlaceDic.ContainsKey(ppData.guid))
                            {
                                prefabPlaceDic.Add(ppData.guid, ppData);
                            }

                            var jsonSubDatas = jsonObj["datas"];
                            for (int k = 0; k < jsonSubDatas.Count(); k++)
                            {
                                var jsonSubObj = jsonSubDatas[k];
                                pcDatas.Add(new PrefabCreateData
                                {
                                    index = int.Parse(jsonSubObj["index"].ToString()),
                                    name = jsonSubObj["name"].ToString(),
                                    guid = ppData.guid,
                                    tag = jsonSubObj["tag"].ToString(),
                                    layer = jsonSubObj["layer"].ToString(),
                                    parentPath = jsonSubObj["parentPath"].ToString(),
                                    isDisplay = bool.Parse(jsonSubObj["isDisplay"].ToString()),
                                    localPosition = new Vector3(float.Parse(jsonSubObj["localPositionX"].ToString()),
                                        float.Parse(jsonSubObj["localPositionY"].ToString()),
                                        float.Parse(jsonSubObj["localPositionZ"].ToString())),
                                    localEulerAngles =
                                        new Vector3(float.Parse(jsonSubObj["localEulerAnglesX"].ToString()),
                                            float.Parse(jsonSubObj["localEulerAnglesY"].ToString()),
                                            float.Parse(jsonSubObj["localEulerAnglesZ"].ToString())),
                                    localScale = new Vector3(float.Parse(jsonSubObj["localScaleX"].ToString()),
                                        float.Parse(jsonSubObj["localScaleY"].ToString()),
                                        float.Parse(jsonSubObj["localScaleZ"].ToString()))
                                });
                            }
                        }
                        pcDatas.Sort(((d1, d2) =>
                        {
                            if (d1.index < d2.index) return -1;
                            return 1;
                        }));

                        prefabCreateDic.Add(collector.data[i].key, pcDatas);
                    }
                }
            }

            Game.Instance.EventSystem.Invoke<PrefabAssociateDataLoadComplete>();
        }

        public List<PrefabCreateData> GetPrefabCreateDataListByName(string name)
        {
            return prefabCreateDic[name];
        }

        public PrefabPlaceData GetPrefabPlaceDataByName(string guid)
        {
            return prefabPlaceDic[guid];
        }
    }
}