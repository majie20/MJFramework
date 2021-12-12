using System;
using System.Collections.Generic;

namespace Model
{
    public class UIValue
    {
        public static Dictionary<Type, string> UIPathDic = new Dictionary<Type, string>();

        public static void Add()
        {
            UIPathDic.Add(typeof(HotComponent), "");
        }
    }
}