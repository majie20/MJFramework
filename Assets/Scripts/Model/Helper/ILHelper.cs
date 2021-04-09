using ILRuntime.Runtime.Intepreter;
using System.Collections.Generic;

namespace MGame.Model
{
    public static class ILHelper
    {
        public static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            // 注册重定向函数

            // 注册委托
            appdomain.DelegateManager.RegisterMethodDelegate<List<object>>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();

            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);

            // 注册适配器

            //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            //domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            //domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            //domain.RegisterCrossBindingAdaptor(new TestClassBaseAdapter());
            //domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        }
    }
}