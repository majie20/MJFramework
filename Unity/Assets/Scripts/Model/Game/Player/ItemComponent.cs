using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Model
{
    public class ItemComponent : MObjectComponent, IAwake
    {
        public void Awake()
        {
            Type = ObjectType.Item;
        }
    }
}
