using System;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class ComponentPoolComponent : Component, IAwake
    {
        private Dictionary<Type, Queue<Component>> componentDic;

        public void Awake()
        {
            componentDic = new Dictionary<Type, Queue<Component>>();
        }

        public override void Dispose()
        {
            componentDic = null;
            Entity = null;
        }

        public T HatchComponent<T>() where T : Component
        {
            return (T)HatchComponent(typeof(T));
        }

        public Component HatchComponent(Type type)
        {
            return (Component)_HatchComponent(type);
        }

        private object _HatchComponent(Type type)
        {
            if (componentDic.ContainsKey(type))
            {
                return componentDic[type].Dequeue();
            }

            object component = null;
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                component = ((ILRuntime.Reflection.ILRuntimeType)type).ILType.Instantiate();
            }
            else
            {
                component = Activator.CreateInstance(type);
            }

            return component;
        }

        public void RecycleComponent(Component component)
        {
            var type = component.GetType();
            Queue<Component> queue;
            if (componentDic.ContainsKey(type))
            {
                queue = componentDic[type];
            }
            else
            {
                queue = new Queue<Component>();
                componentDic.Add(type, queue);
            }

            queue.Enqueue(component);
        }

        public unsafe void RegisterILRuntimeCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            foreach (var i in typeof(ComponentPoolComponent).GetMethods())
            {
                if (i.Name == "HatchComponent" && !i.IsGenericMethodDefinition)
                {
                    appdomain.RegisterCLRMethodRedirection(i, JsonToObject);
                }
            }
        }

        public unsafe StackObject* JsonToObject(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            Type type = (Type)typeof(Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);
            var result_of_this_method = _HatchComponent(type);

            return ILIntepreter.PushObject(__ret, mStack, result_of_this_method);
        }
    }
}