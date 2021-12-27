using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class UIRootComponent : Component, IAwake
    {
        public static string UIROOT_PATH = "Assets/Res/Prefabs/UIRoot";
        public void Awake()
        {
            var transform = this.Entity.Transform;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }

        public override void Dispose()
        {
            Entity = null;
        }
    }
}