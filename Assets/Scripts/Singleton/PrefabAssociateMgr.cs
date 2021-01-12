using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;


public class PrefabJsonData
{
    public string name;
    public string guid;
    public string abName;
    public List<PrefabCreateData> datas = new List<PrefabCreateData>();
}

[Serializable]
public class PrefabCreateData
{
    public string name;
    public string tag;
    public string layer;
    public string parentPath;
    public bool isDisplay;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

namespace Game.Singleton
{
    public class PrefabAssociateMgr : Singleton<PrefabAssociateMgr>
    {
        private Dictionary<string, List<PrefabJsonData>> dataDic;

        public override void Init()
        {
            base.Init();
            dataDic = new Dictionary<string, List<PrefabJsonData>>();
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
                            var pjDatas = new List<PrefabJsonData>();
                            for (int j = 0; j < jsonDatas.Count(); j++)
                            {
                                var jsonObj = jsonDatas[j];
                                var pjData = new PrefabJsonData();
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
                                    pcData.position = new Vector3(float.Parse(jsonSubObj["positionX"].ToString()), float.Parse(jsonSubObj["positionY"].ToString()), float.Parse(jsonSubObj["positionZ"].ToString()));
                                    pcData.rotation = new Vector3(float.Parse(jsonSubObj["rotationX"].ToString()), float.Parse(jsonSubObj["rotationY"].ToString()), float.Parse(jsonSubObj["rotationZ"].ToString()));
                                    pcData.scale = new Vector3(float.Parse(jsonSubObj["scaleX"].ToString()), float.Parse(jsonSubObj["scaleY"].ToString()), float.Parse(jsonSubObj["scaleZ"].ToString()));
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

        public IEnumerable<PrefabJsonData> GetPrefabJsonDatasByName(string name)
        {
            return dataDic[name];
        }
    }
}
