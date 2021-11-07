using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if ILRuntime

using System.IO;

#else
using System;
using System.Reflection;
#endif

namespace Model
{
    public sealed class Hotfix
    {
#if ILRuntime
        private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get; }

        private MemoryStream dllStream;
        private MemoryStream pdbStream;
#else
        private Assembly assembly;
#endif

        private List<Type> hotfixTypes;

        private IStaticMethod start;
        private bool isRuning;

        public bool IsRuning
        {
            private set
            {
                isRuning = value;
            }
            get
            {
                return isRuning;
            }
        }

        public Action<float> GameUpdate;
        public Action GameLateUpdate;
        public Action GameApplicationQuit;

        public Hotfix()
        {
            IsRuning = false;
        }

        public void Dispose()
        {
#if ILRuntime
            dllStream?.Close();
            pdbStream?.Close();
            dllStream = null;
            pdbStream = null;
#else

#endif
        }

        public void GotoHotfix()
        {
#if ILRuntime
            ILHelper.InitILRuntime(this.appDomain);
#endif
            this.start.Run();

            IsRuning = true;
        }

        public List<Type> GetHotfixTypes()
        {
            return this.hotfixTypes;
        }

        public void LoadHotfixAssembly()
        {
            var component = Game.Instance.Scene.GetComponent<TextManageComponent>();
            byte[] assBytes = component.GetTextAssetByName("Hotfix.dll").bytes;
            byte[] pdbBytes = component.GetTextAssetByName("Hotfix.pdb").bytes;

#if ILRuntime
            Debug.Log($"当前使用的是ILRuntime模式");
            this.appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            this.dllStream = new MemoryStream(assBytes);
            this.pdbStream = new MemoryStream(pdbBytes);
            this.appDomain.LoadAssembly(this.dllStream, this.pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

            appDomain.DebugService.StartDebugService(55000);
#endif

            this.start = new ILStaticMethod(this.appDomain, "Hotfix.Init", "Start", 0);

            this.hotfixTypes = this.appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
#else
            Debug.Log($"当前使用的是Mono模式");

            this.assembly = Assembly.Load(assBytes, pdbBytes);

            Type hotfixInit = this.assembly.GetType("Hotfix.Init");
            this.start = new MonoStaticMethod(hotfixInit, "Start");

            this.hotfixTypes = this.assembly.GetTypes().ToList();
#endif
        }
    }
}