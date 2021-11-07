using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class ComponentView : MonoBehaviour
    {
        public bool isHotfix { set; get; }
        public Dictionary<object, Type> dic = new Dictionary<object, Type>();
    }
}