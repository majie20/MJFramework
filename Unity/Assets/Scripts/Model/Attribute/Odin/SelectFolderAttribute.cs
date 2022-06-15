using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SelectFolderAttribute : BaseAttribute
    {
        public Type TargetType;

        public SelectFolderAttribute(Type type)
        {
            this.TargetType = type;
        }
    }
}