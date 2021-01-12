using System.Collections;
using System.Collections.Generic;
using Game.Singleton;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    void Start()
    {
        var pjDatas = PrefabAssociateMgr.Instance.GetPrefabJsonDatasByName("Cube");
        foreach (var pjData in pjDatas)
        {
            var obj = ABMgr.Instance.GetPrefabByName(pjData.name);
            for (int i = 0; i < pjData.datas.Count; i++)
            {
                GameObject instance;
                var pcdata = pjData.datas[i];
                var parentPath = pcdata.parentPath;
                if (string.IsNullOrEmpty(parentPath))
                    instance = UnityEngine.Object.Instantiate(obj, transform);
                else
                {
                    instance = UnityEngine.Object.Instantiate(obj, transform.Find(parentPath));
                }
                instance.name = pcdata.name;
                instance.tag = pcdata.tag;
                instance.layer = LayerMask.NameToLayer(pcdata.layer);
                instance.transform.localPosition = pcdata.position;
                instance.transform.localEulerAngles = pcdata.rotation;
                instance.transform.localScale = pcdata.scale;
                instance.SetActive(pcdata.isDisplay);
            }
        }
    }
}
