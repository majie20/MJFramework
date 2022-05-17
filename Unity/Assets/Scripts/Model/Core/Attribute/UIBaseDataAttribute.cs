using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIBaseDataAttribute : BaseAttribute
    {
        public int UIViewType;
        public string PrefabPath;
        public int UIMaskMode;
        public int UILayer;
        public bool IsOperateMask = true;
    }
}