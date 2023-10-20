#if ILRuntime
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif

namespace Model
{   
    public class IAsyncStateMachineAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Runtime.CompilerServices.IAsyncStateMachine);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : System.Runtime.CompilerServices.IAsyncStateMachine, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mMoveNext_0 = new CrossBindingMethodInfo("MoveNext");
            CrossBindingMethodInfo<System.Runtime.CompilerServices.IAsyncStateMachine> mSetStateMachine_1 = new CrossBindingMethodInfo<System.Runtime.CompilerServices.IAsyncStateMachine>("SetStateMachine");

            bool isInvokingToString;
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public void MoveNext()
            {
                mMoveNext_0.Invoke(this.instance);
            }

            public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
            {
                mSetStateMachine_1.Invoke(this.instance, stateMachine);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    if (!isInvokingToString)
                    {
                        isInvokingToString = true;
                        string res = instance.ToString();
                        isInvokingToString = false;
                        return res;
                    }
                    else
                        return instance.Type.FullName;
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}
#endif

