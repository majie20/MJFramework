using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentOfAttribute : BaseAttribute
    {
        public Type[] Types;

        public ComponentOfAttribute(params Type[] types)
        {
            this.Types = types;
        }
    }
}