using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NP_NodeTypeBindAttribute : BaseAttribute
    {
        public Type Type;
    }
}