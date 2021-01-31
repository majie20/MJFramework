using Game.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MGame
{
    public class PrefabAssociateData
    {
        public string name;
        public string guid;
        public string abName;
    }

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

    public class PrefabAssociateMgr : Singleton<PrefabAssociateMgr>
    {
        private Dictionary<string, PrefabAssociateData> PrefabAssociateDic;
        private Dictionary<string, List<PrefabCreateData>> PrefabCreateDic;

        public override void Init()
        {
            base.Init();
            PrefabAssociateDic = new Dictionary<string, PrefabAssociateData>();
            PrefabCreateDic = new Dictionary<string, List<PrefabCreateData>>();
            EventSystem.Instance.Add<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
        }

        public override void Dispose()
        {
            base.Dispose();
            EventSystem.Instance.Remove<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
        }

        private void OnAssetBundleLoadComplete()
        {
            var collectors = ABMgr.Instance.GetAllPrefabJsonDataCollector();
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
                            var paData = new PrefabAssociateData
                            {
                                name = jsonObj["name"].ToString(),
                                guid = jsonObj["guid"].ToString(),
                                abName = jsonObj["abName"].ToString()
                            };
                            if (!PrefabAssociateDic.ContainsKey(paData.guid))
                            {
                                PrefabAssociateDic.Add(paData.guid, paData);
                            }

                            var jsonSubDatas = jsonObj["datas"];
                            for (int k = 0; k < jsonSubDatas.Count(); k++)
                            {
                                var jsonSubObj = jsonSubDatas[k];
                                pcDatas.Add(new PrefabCreateData
                                {
                                    index = int.Parse(jsonSubObj["index"].ToString()),
                                    name = jsonSubObj["name"].ToString(),
                                    guid = paData.guid,
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
                        PrefabCreateDic.Add(collector.data[i].key, pcDatas);
                    }
                }
            }

            EventSystem.Instance.Run<PrefabAssociateDataLoadComplete>();
        }

        public IEnumerable<PrefabCreateData> GetPrefabJsonDatasByName(string name)
        {
            return PrefabCreateDic[name];
        }

        public PrefabAssociateData GetPrefabAssociateDataByName(string guid)
        {
            return PrefabAssociateDic[guid];
        }
    }
}