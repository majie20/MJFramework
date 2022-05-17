using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

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
            CrossBindingMethodInfo mAddComponent_2 = new CrossBindingMethodInfo("AddComponent");
            CrossBindingMethodInfo mClose_3 = new CrossBindingMethodInfo("Close");
            CrossBindingMethodInfo mOnOpen_4 = new CrossBindingMethodInfo("OnOpen");
            CrossBindingMethodInfo mOnClose_5 = new CrossBindingMethodInfo("OnClose");
            CrossBindingMethodInfo mEnable_6 = new CrossBindingMethodInfo("Enable");
            CrossBindingMethodInfo mDisable_7 = new CrossBindingMethodInfo("Disable");

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

            protected override void AddComponent()
            {
                if (mAddComponent_2.CheckShouldInvokeBase(this.instance))
                    base.AddComponent();
                else
                    mAddComponent_2.Invoke(this.instance);
            }

            public override void Close()
            {
                if (mClose_3.CheckShouldInvokeBase(this.instance))
                    base.Close();
                else
                    mClose_3.Invoke(this.instance);
            }

            protected override void OnOpen()
            {
                if (mOnOpen_4.CheckShouldInvokeBase(this.instance))
                    base.OnOpen();
                else
                    mOnOpen_4.Invoke(this.instance);
            }

            protected override void OnClose()
            {
                if (mOnClose_5.CheckShouldInvokeBase(this.instance))
                    base.OnClose();
                else
                    mOnClose_5.Invoke(this.instance);
            }

            public override void Enable()
            {
                if (mEnable_6.CheckShouldInvokeBase(this.instance))
                    base.Enable();
                else
                    mEnable_6.Invoke(this.instance);
            }

            public override void Disable()
            {
                if (mDisable_7.CheckShouldInvokeBase(this.instance))
                    base.Disable();
                else
                    mDisable_7.Invoke(this.instance);
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

