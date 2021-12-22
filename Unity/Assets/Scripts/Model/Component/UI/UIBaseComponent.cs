using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace Model
{
    [LifeCycle]
    public class UIBaseComponent : Component, IAwake
    {
        public void Awake()
        {            
            UIValue.Add();
        }
    }
}