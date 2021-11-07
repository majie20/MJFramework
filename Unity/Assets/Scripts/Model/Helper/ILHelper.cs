using ILRuntime.Runtime.Intepreter;

namespace Model
{
    public static class ILHelper
    {
        public static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            // 注册重定向函数

            // 注册委托

            appdomain.DelegateManager.RegisterMethodDelegate<float>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String>();

            appdomain.DelegateManager.RegisterDelegateConvertor<EventDelegateParams>((action) =>
            {
                return new EventDelegateParams((a) =>
                {
                    ((System.Action<object[]>)action)(a);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((action) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((System.Action)action)();
                });
            });
            //appdomain.DelegateManager.RegisterDelegateConvertor<GameUpdate>((action) =>
            //{
            //    return new GameUpdate((a) =>
            //    {
            //        ((System.Action<float>)action)(a);
            //    });
            //});
            //appdomain.DelegateManager.RegisterDelegateConvertor<GameLateUpdate>((action) =>
            //{
            //    return new GameLateUpdate(() => { ((System.Action)action)(); });
            //});
            //appdomain.DelegateManager.RegisterDelegateConvertor<GameApplicationQuit>((action) =>
            //{
            //    return new GameApplicationQuit(() => { ((System.Action)action)(); });
            //});

            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);

            // 注册适配器

            appdomain.RegisterCrossBindingAdaptor(new IDisposableAdapter());

            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);

            //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            //appdomain.RegisterCrossBindingAdaptor(new EventDelegateParams());
            //domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            //domain.RegisterCrossBindingAdaptor(new TestClassBaseAdapter());
            //domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        }
    }
}