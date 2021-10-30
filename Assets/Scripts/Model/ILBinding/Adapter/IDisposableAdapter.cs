using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace MGame.Model
{
    public class IDisposableAdapter : CrossBindingAdaptor
    {
        //定义访问方法的方法信息
        private static CrossBindingMethodInfo mDispose_0 = new CrossBindingMethodInfo("Dispose");

        public override Type BaseCLRType
        {
            get
            {
                return typeof(IDisposable); //这里是你想继承的类型
            }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adapter); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain,
            ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : IDisposable, CrossBindingAdaptorType
        {
            private ILTypeInstance instance;
            private ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            //必须要提供一个无参数的构造函数
            public Adapter()
            {
            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }

            //下面将所有虚函数都重载一遍，并中转到热更内

            public void Dispose()
            {
                mDispose_0.Invoke(this.instance);
            }
        }
    }
}