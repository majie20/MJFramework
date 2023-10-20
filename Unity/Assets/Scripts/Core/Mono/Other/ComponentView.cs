using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class ComponentView : MonoBehaviour
    {
        public Dictionary<Component, Type> dic = new Dictionary<Component, Type>();
    }
}