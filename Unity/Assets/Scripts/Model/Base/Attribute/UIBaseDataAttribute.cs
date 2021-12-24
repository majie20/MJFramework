using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIBaseDataAttribute : BaseAttribute
    {
        public UIViewType Type;
        public string PrefabPath;
    }
}