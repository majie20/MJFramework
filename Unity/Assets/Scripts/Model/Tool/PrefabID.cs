using System;
using System.Globalization;
using UnityEngine;

public class PrefabID : MonoBehaviour
{
    [HideInInspector]
    public string ID = System.Guid.NewGuid().ToString();

    private void Awake()
    {
        Debug.Log(ID);
    }

}