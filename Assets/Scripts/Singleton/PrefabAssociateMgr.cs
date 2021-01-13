using Game.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PrefabAssociateData
{
    public string name;
    public string guid;
    public string abName;
    public List<PrefabCreateData> datas = new List<PrefabCreateData>();
}

public class PrefabCreateData
{
    public string name;
    public string tag;
    public string layer;
    public string parentPath;
    public bool isDisplay;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;
}

namespace Game.Singleton
{
    public class PrefabAssociateMgr : Singleton<PrefabAssociateMgr>
    {
        private Dictionary<string, List<PrefabAssociateData>> dataDic;

        public override void Init()
        {
            base.Init();
            dataDic = new Dictionary<string, List<PrefabAssociateData>>();
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
                    if (!dataDic.ContainsKey(collector.data[i].key))
                    {
                        using (var sr = new StringReader(collector.Get<TextAsset>(collector.data[i].key).text))
                        {
                            var reader = new JsonTextReader(sr);
                            var jsonDatas = JToken.ReadFrom(reader);
                            var pjDatas = new List<PrefabAssociateData>();
                            for (int j = 0; j < jsonDatas.Count(); j++)
                            {
                                var jsonObj = jsonDatas[j];
                                var pjData = new PrefabAssociateData();
                                pjData.name = jsonObj["name"].ToString();
                                pjData.guid = jsonObj["guid"].ToString();
                                pjData.abName = jsonObj["abName"].ToString();
                                var jsonSubDatas = jsonObj["datas"];
                                for (int k = 0; k < jsonSubDatas.Count(); k++)
                                {
                                    if (pjData.datas == null)
                                        pjData.datas = new List<PrefabCreateData>();
                                    var jsonSubObj = jsonSubDatas[k];
                                    var pcData = new PrefabCreateData();
                                    pcData.name = jsonSubObj["name"].ToString();
                                    pcData.tag = jsonSubObj["tag"].ToString();
                                    pcData.layer = jsonSubObj["layer"].ToString();
                                    pcData.parentPath = jsonSubObj["parentPath"].ToString();
                                    pcData.isDisplay = bool.Parse(jsonSubObj["isDisplay"].ToString());
                                    pcData.localPosition = new Vector3(float.Parse(jsonSubObj["localPositionX"].ToString()), float.Parse(jsonSubObj["localPositionY"].ToString()), float.Parse(jsonSubObj["localPositionZ"].ToString()));
                                    pcData.localEulerAngles = new Vector3(float.Parse(jsonSubObj["localEulerAnglesX"].ToString()), float.Parse(jsonSubObj["localEulerAnglesY"].ToString()), float.Parse(jsonSubObj["localEulerAnglesZ"].ToString()));
                                    pcData.localScale = new Vector3(float.Parse(jsonSubObj["localScaleX"].ToString()), float.Parse(jsonSubObj["localScaleY"].ToString()), float.Parse(jsonSubObj["localScaleZ"].ToString()));
                                    pjData.datas.Add(pcData);
                                }
                                pjDatas.Add(pjData);
                            }
                            dataDic.Add(collector.data[i].key, pjDatas);
                        }
                    }
                }
            }

            UnityEngine.Object.Instantiate(ABMgr.Instance.GetPrefabByName("Cube"));
        }

        public IEnumerable<PrefabAssociateData> GetPrefabJsonDatasByName(string name)
        {
            return dataDic[name];
        }
    }
}