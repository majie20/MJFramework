using System;

namespace Hotfix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseAttribute : Attribute
    {
        public Type AttributeType { get; }

        public BaseAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }
}