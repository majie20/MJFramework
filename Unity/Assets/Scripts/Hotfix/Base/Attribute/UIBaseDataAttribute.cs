using System;

namespace Hotfix
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIBaseDataAttribute : BaseAttribute
    {
        public UIViewType UIViewType;
        public string PrefabPath;
        public UIMaskMode UIMaskMode;
    }
}