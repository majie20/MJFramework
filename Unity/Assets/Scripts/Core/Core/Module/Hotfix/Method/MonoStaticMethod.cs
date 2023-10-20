#if HybridCLR
using System;
using System.Reflection;

namespace Model
{
    public class MonoStaticMethod : IStaticMethod
    {
        private readonly MethodInfo methodInfo;

        private readonly object[] param;

        public MonoStaticMethod(Type type, string methodName)
        {
            this.methodInfo = type.GetMethod(methodName);
            this.param = new object[this.methodInfo.GetParameters().Length];
        }

        public void Run()
        {
            this.methodInfo.Invoke(null, param);
        }

        public void Run(object a)
        {
            this.param[0] = a;
            this.methodInfo.Invoke(null, param);
        }

        public void Run(object a, object b)
        {
            this.param[0] = a;
            this.param[1] = b;
            this.methodInfo.Invoke(null, param);
        }

        public void Run(object a, object b, object c)
        {
            this.param[0] = a;
            this.param[1] = b;
            this.param[2] = c;
            this.methodInfo.Invoke(null, param);
        }
    }
}
#endif