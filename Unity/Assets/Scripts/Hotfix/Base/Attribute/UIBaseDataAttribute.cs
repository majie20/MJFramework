using System;

namespace Hotfix
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIBaseDataAttribute : BaseAttribute
    {
        public Model.UIViewType UIViewType;
        public string PrefabPath;
        public Model.UIMaskMode UIMaskMode;
    }
}