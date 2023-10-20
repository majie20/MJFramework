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
    public class UIBaseComponentAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Model.UIBaseComponent);
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

        public class Adapter : Model.UIBaseComponent, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mAwake_0 = new CrossBindingMethodInfo("Awake");
            CrossBindingMethodInfo mDispose_1 = new CrossBindingMethodInfo("Dispose");
            CrossBindingMethodInfo mClose_2 = new CrossBindingMethodInfo("Close");
            CrossBindingMethodInfo mOnClose_3 = new CrossBindingMethodInfo("OnClose");
            CrossBindingMethodInfo mOnEnable_4 = new CrossBindingMethodInfo("OnEnable");
            CrossBindingMethodInfo mOnDisable_5 = new CrossBindingMethodInfo("OnDisable");

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

            public override void Awake()
            {
                if (mAwake_0.CheckShouldInvokeBase(this.instance))
                    base.Awake();
                else
                    mAwake_0.Invoke(this.instance);
            }

            public override void Dispose()
            {
                if (mDispose_1.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_1.Invoke(this.instance);
            }

            public override void Close()
            {
                if (mClose_2.CheckShouldInvokeBase(this.instance))
                    base.Close();
                else
                    mClose_2.Invoke(this.instance);
            }

            protected override void OnClose()
            {
                if (mOnClose_3.CheckShouldInvokeBase(this.instance))
                    base.OnClose();
                else
                    mOnClose_3.Invoke(this.instance);
            }

            protected override void OnEnable()
            {
                if (mOnEnable_4.CheckShouldInvokeBase(this.instance))
                    base.OnEnable();
                else
                    mOnEnable_4.Invoke(this.instance);
            }

            protected override void OnDisable()
            {
                if (mOnDisable_5.CheckShouldInvokeBase(this.instance))
                    base.OnDisable();
                else
                    mOnDisable_5.Invoke(this.instance);
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

