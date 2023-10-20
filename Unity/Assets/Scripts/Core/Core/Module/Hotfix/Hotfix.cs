using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Model
{
    public sealed class Hotfix
    {
#if ILRuntime
        private ILRuntime.Runtime.Enviorment.AppDomain appDomain;

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain
        {
            private set { appDomain = value; }
            get { return appDomain; }
        }

        private System.IO.MemoryStream dllStream;
        private System.IO.MemoryStream pdbStream;

        private Dictionary<string, ILRuntime.CLR.Method.IMethod> methodDic;

        public Dictionary<string, ILRuntime.CLR.Method.IMethod> MethodDic
        {
            private set { methodDic = value; }
            get { return methodDic; }
        }
#elif HybridCLR
        private System.Reflection.Assembly assembly;
#endif

        private List<Type> hotfixTypes;

        private IStaticMethod start;
        private bool          isRuning;

        public bool IsRuning
        {
            private set { isRuning = value; }
            get { return isRuning; }
        }
#if ILRuntime
        public Action<float> GameUpdate;
        public Action<float> GameFixedUpdate;
        public Action        GameLateUpdate;
#endif
        public Action GameApplicationQuit;

        public Hotfix()
        {
            IsRuning = false;
#if ILRuntime
            MethodDic = new Dictionary<string, ILRuntime.CLR.Method.IMethod>();
#endif
        }

        public void Dispose()
        {
#if ILRuntime
            MethodDic = null;
            dllStream?.Close();
            pdbStream?.Close();
            dllStream = null;
            pdbStream = null;
#endif
            IsRuning = false;
        }

        public void GotoHotfix()
        {
#if ILRuntime
            ILHelper.InitILRuntime(this.AppDomain);
#endif

            this.start?.Run();

            IsRuning = true;
        }

        public List<Type> GetHotfixTypes()
        {
            return this.hotfixTypes;
        }

        public async UniTask LoadHotfixAssembly()
        {
            var component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            byte[] assBytes = (await component.LoadAsync<UnityEngine.TextAsset>($"{ConstData.CODE_DIR_PATH}Hotfix.dll.bytes")).bytes;
            byte[] pdbBytes = (await component.LoadAsync<UnityEngine.TextAsset>($"{ConstData.CODE_DIR_PATH}Hotfix.pdb.bytes")).bytes;

#if ILRuntime
            NLog.Log.Debug($"当前使用的是ILRuntime模式");
            this.AppDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            this.dllStream = new System.IO.MemoryStream(assBytes);
            this.pdbStream = new System.IO.MemoryStream(pdbBytes);
            this.AppDomain.LoadAssembly(this.dllStream, this.pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

            AppDomain.DebugService.StartDebugService(56000);
#endif

            this.start = new ILStaticMethod(this.AppDomain, "Hotfix.Init", "Start", 0);

            this.hotfixTypes = this.AppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();

            AddMethod();
#elif HybridCLR
            NLog.Log.Debug($"当前使用的是HybridCLR模式");

            this.assembly = System.Reflection.Assembly.Load(assBytes, pdbBytes);

            Type hotfixInit = this.assembly.GetType("Hotfix.Init");

            this.start = new MonoStaticMethod(hotfixInit, "Start");

            this.hotfixTypes = this.assembly.GetTypes().ToList();
#endif
        }

#if ILRuntime
        public void AddMethod()
        {
            MethodDic.Add("Hotfix.ObjectHelper.CreateComponent3", AppDomain.LoadedTypes["Hotfix.ObjectHelper"].GetMethod("CreateComponent", 3));
            MethodDic.Add("Hotfix.ObjectHelper.RemoveComponent2", AppDomain.LoadedTypes["Hotfix.ObjectHelper"].GetMethod("RemoveComponent", 2));
            MethodDic.Add("Hotfix.ObjectHelper.AddLifecycle", AppDomain.LoadedTypes["Hotfix.ObjectHelper"].GetMethod("AddLifecycle", 1));
            MethodDic.Add("Hotfix.ObjectHelper.RemoveLifecycle", AppDomain.LoadedTypes["Hotfix.ObjectHelper"].GetMethod("RemoveLifecycle", 1));
        }
#endif
    }
}