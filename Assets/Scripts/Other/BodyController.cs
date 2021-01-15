using System.Collections;
using System.Collections.Generic;
using Game.Singleton;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    void Start()
    {
        var pcDatas = PrefabAssociateMgr.Instance.GetPrefabJsonDatasByName("Cube");
        foreach (var pcData in pcDatas)
        {
            var obj = ABMgr.Instance.GetPrefabByName(PrefabAssociateMgr.Instance.GetPrefabAssociateDataByName(pcData.guid).name);
            GameObject instance;
            var parentPath = pcData.parentPath;
            if (string.IsNullOrEmpty(parentPath))
                instance = UnityEngine.Object.Instantiate(obj, transform);
            else
                instance = UnityEngine.Object.Instantiate(obj, transform.Find(parentPath));
            instance.name = pcData.name;
            instance.tag = pcData.tag;
            instance.layer = LayerMask.NameToLayer(pcData.layer);
            instance.transform.localPosition = pcData.localPosition;
            instance.transform.localEulerAngles = pcData.localEulerAngles;
            instance.transform.localScale = pcData.localScale;
            instance.SetActive(pcData.isDisplay);
        }
    }
}
