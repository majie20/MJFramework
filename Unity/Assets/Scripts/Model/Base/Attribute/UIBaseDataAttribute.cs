using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIBaseDataAttribute : BaseAttribute
    {
        public UIViewType UIViewType;
        public string PrefabPath;
        public UIMaskMode UIMaskMode;
    }
}