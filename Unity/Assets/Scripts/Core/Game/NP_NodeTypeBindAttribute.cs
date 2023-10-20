using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NP_NodeTypeBindAttribute : BaseAttribute
    {
        public Type Type;

        public NP_NodeTypeBindAttribute(Type type)
        {
            this.Type = type;
        }
    }
}