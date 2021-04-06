using System.Collections.Generic;
using UnityEngine;

namespace MGame.Model
{
    public class BodyConstructor : MonoBehaviour
    {
        private string originName;
        private Dictionary<GameObject, string> subObjDic;

        /// <summary>
        /// 组合
        /// </summary>
        /// <param name="name"></param>
        public void Assemble(string name)
        {
            originName = name;
            subObjDic = new Dictionary<GameObject, string>();
            var pcDataList = Game.Instance.Scene.GetComponent<PrefabAssociateComponent>().GetPrefabCreateDataListByName(originName);
            foreach (var pcData in pcDataList)
            {
                Splicing(pcData);
            }
        }

        /// <summary>
        /// 拼接身体的物件
        /// </summary>
        public void Splicing(PrefabCreateData pcData)
        {
            var parentPath = pcData.parentPath;
            var subName = Game.Instance.Scene.GetComponent<PrefabAssociateComponent>().GetPrefabPlaceDataByName(pcData.guid).name;
            var subObj = Game.Instance.ObjectPool.GetGameObjByName(subName, true);

            subObj.transform.SetParent(string.IsNullOrEmpty(parentPath) ? transform : transform.Find(parentPath));
            subObj.name = pcData.name;
            subObj.tag = pcData.tag;
            subObj.layer = LayerMask.NameToLayer(pcData.layer);
            subObj.transform.localPosition = pcData.localPosition;
            subObj.transform.localEulerAngles = pcData.localEulerAngles;
            subObj.transform.localScale = pcData.localScale;
            subObj.SetActive(pcData.isDisplay);
            subObj.GetComponent<BodyConstructor>()?.Assemble(subName);

            AddIn(subName, subObj);
        }

        /// <summary>
        /// 加入身体里，成为它的一部分
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void AddIn(string name, GameObject obj)
        {
            if (!IsInTheBody(obj))
            {
                subObjDic.Add(obj, name);
            }
        }

        /// <summary>
        /// 打碎
        /// </summary>
        public void Smash()
        {
            foreach (var key in subObjDic.Keys)
            {
                Separate(key);
            }

            subObjDic = null;
            Game.Instance.ObjectPool.RecycleGameObj(originName, gameObject);
        }

        /// <summary>
        /// 分离
        /// </summary>
        public void Separate(GameObject obj)
        {
            obj.GetComponent<BodyConstructor>()?.Smash();
            Game.Instance.ObjectPool.RecycleGameObj(subObjDic[obj], obj);
        }

        /// <summary>
        /// 是否是该物体的一部分
        /// </summary>
        public bool IsInTheBody(GameObject obj)
        {
            return subObjDic.ContainsKey(obj);
        }
    }
}