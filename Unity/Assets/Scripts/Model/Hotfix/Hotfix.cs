using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using ILRuntime.CLR.Method;


namespace Model
{
    public sealed class Hotfix
    {
        private ILRuntime.Runtime.Enviorment.AppDomain appDomain;

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain
        {
            private set
            {
                appDomain = value;
            }
            get
            {
                return appDomain;
            }
        }

        private MemoryStream dllStream;
        private MemoryStream pdbStream;

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

        private Dictionary<string, IMethod> methodDic;

        public Dictionary<string, IMethod> MethodDic
        {
            private set
            {
                methodDic = value;
            }
            get
            {
                return methodDic;
            }
        }

        public Hotfix()
        {
            IsRuning = false;
            MethodDic = new Dictionary<string, IMethod>();
        }

        public void Dispose()
        {
            MethodDic = null;
            dllStream?.Close();
            pdbStream?.Close();
            dllStream = null;
            pdbStream = null;
            IsRuning = false;
        }

        public void GotoHotfix()
        {
            ILHelper.InitILRuntime(this.AppDomain);

            this.start.Run();

            IsRuning = true;
        }

        public List<Type> GetHotfixTypes()
        {
            return this.hotfixTypes;
        }

        public void LoadHotfixAssembly()
        {
            var component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            byte[] assBytes = component.Load<UnityEngine.TextAsset>($"{FileValue.BUILD_AB_RES_PATH}Text/Hotfix.dll").bytes;
            byte[] pdbBytes = component.Load<UnityEngine.TextAsset>($"{FileValue.BUILD_AB_RES_PATH}Text/Hotfix.pdb").bytes;


            Debug.Log($"当前使用的是ILRuntime模式");
            this.AppDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            this.dllStream = new MemoryStream(assBytes);
            this.pdbStream = new MemoryStream(pdbBytes);
            this.AppDomain.LoadAssembly(this.dllStream, this.pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

            AppDomain.DebugService.StartDebugService(56000);
#endif

            this.start = new ILStaticMethod(this.AppDomain, "Hotfix.Init", "Start", 0);

            this.hotfixTypes = this.AppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();

            AddMethod();
        }

        public void AddMethod()
        {
            MethodDic.Add("Hotfix.ObjectHelper.CreateComponent3", AppDomain.LoadedTypes["Hotfix.ObjectHelper"].GetMethod("CreateComponent", 3));
            MethodDic.Add("Hotfix.ObjectHelper.RemoveComponent2", AppDomain.LoadedTypes["Hotfix.ObjectHelper"].GetMethod("RemoveComponent", 2));
        }
    }
}