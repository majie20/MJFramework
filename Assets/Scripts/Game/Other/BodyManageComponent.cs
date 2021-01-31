using System.Collections.Generic;
using UnityEngine;

namespace MGame
{
    public class BodyManageComponent : MonoBehaviour
    {
        private string originName;
        private Dictionary<GameObject, string> subObjDic;
        private List<GameObject> subObjs;

        public void Assemble(string name)
        {
            originName = name;
            subObjs = new List<GameObject>();
            subObjDic = new Dictionary<GameObject, string>();
            var pcDatas = PrefabAssociateMgr.Instance.GetPrefabJsonDatasByName(name);
            foreach (var pcData in pcDatas)
            {
                var subName = PrefabAssociateMgr.Instance.GetPrefabAssociateDataByName(pcData.guid).name;
                GameObject subObj = PoolMgr.Instance.GetGameObjByName(subName);
                var parentPath = pcData.parentPath;
                if (string.IsNullOrEmpty(parentPath))
                    subObj.transform.SetParent(transform);
                else
                    subObj.transform.SetParent(transform.Find(parentPath));
                subObj.name = pcData.name;
                subObj.tag = pcData.tag;
                subObj.layer = LayerMask.NameToLayer(pcData.layer);
                subObj.transform.localPosition = pcData.localPosition;
                subObj.transform.localEulerAngles = pcData.localEulerAngles;
                subObj.transform.localScale = pcData.localScale;
                subObj.SetActive(pcData.isDisplay);
                subObj.GetComponent<BodyManageComponent>()?.Assemble(subName);

                AddChild(subName, subObj);
            }
        }

        public void AddChild(string name, GameObject obj)
        {
            if (!subObjDic.ContainsKey(obj))
            {
                subObjDic.Add(obj, name);
            }

            subObjs.Add(obj);
        }

        public void Dismemberment()
        {
            for (int i = subObjs.Count - 1; i >= 0; i--)
            {
                var subObj = subObjs[i];
                subObj.GetComponent<BodyManageComponent>()?.Dismemberment();
                PoolMgr.Instance.RecycleGameObj(subObjDic[subObj], subObj);
            }

            subObjDic = null;
            subObjs = null;
            PoolMgr.Instance.RecycleGameObj(originName, gameObject);
        }
    }
}