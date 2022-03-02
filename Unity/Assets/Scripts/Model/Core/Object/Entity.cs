using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public class Entity : IDisposable
    {
        protected Dictionary<Type, Component> componentDic;
        protected Dictionary<long, Entity> childDic;
        protected ComponentView componentView;

        private Entity parent;

        public Entity Parent
        {
            set
            {
                parent = value;
            }
            get
            {
                return parent;
            }
        }

        private GameObject gameObject;

        public GameObject GameObject
        {
            set
            {
                gameObject = value;
            }
            get
            {
                return gameObject;
            }
        }

        private Transform transform;

        public Transform Transform
        {
            set
            {
                transform = value;
            }
            get
            {
                return transform;
            }
        }

        private string sign;

        public string Sign
        {
            set
            {
                sign = value;
            }
            get
            {
                return sign;
            }
        }

        private long _guid;

        public long Guid
        {
            private set
            {
                _guid = value;
            }
            get
            {
                return _guid;
            }
        }

        public Entity()
        {
            componentDic = new Dictionary<Type, Component>();
            childDic = new Dictionary<long, Entity>();
            Guid = GuidHelper.GuidToLongID();
        }

        public virtual void Dispose()
        {
            if (childDic.Count > 0)
            {
                foreach (var child in childDic.Values)
                {
                    child.Dispose();
                }
                childDic.Clear();
            }

            if (componentDic.Count > 0)
            {
                var types = this.componentDic.Keys.ToArray();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] is ILRuntime.Reflection.ILRuntimeType)
                    {
                        IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.RemoveComponent2"];
                        using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                        {
                            ctx.PushObject(types[i]);
                            ctx.PushObject(this);
                            ctx.Invoke();
                        }
                    }
                    else
                    {
                        ObjectHelper.RemoveComponent(types[i], this);
                    }
                }
                componentDic.Clear();
            }

            componentView = null;

            if (GameObject != null)
            {
                if (Sign != GameObjPoolComponent.None_GameObject)
                {
                    Game.Instance.ObjectPool.RecycleGameObj(Sign, GameObject);
                }
                Transform = null;
                GameObject = null;
            }
        }

        public void AddComponentView()
        {
            var component = GameObject.GetComponent<Model.ComponentView>();
            componentView = component == null ? GameObject.AddComponent<Model.ComponentView>() : component;
            componentView.isHotfix = false;
        }

        private void AddToComponentView(Component component)
        {
            var type = component.GetType();
            if (type.FullName == "Model.ComponentAdapter+Adapter")
            {
                componentView.dic.Add(component, (component as ComponentAdapter.Adapter)?.ILInstance.Type.ReflectionType);
            }
            else
            {
                componentView.dic.Add(component, type);
            }
        }

        private void RemoveToComponentView(Component component)
        {
            componentView.dic.Remove(component);
        }

        public void SetParent(Entity entity)
        {
            Parent = entity;
            entity.AddChild(this);
        }

        public void AddChild(Entity child)
        {
            childDic.Add(child.Guid, child);
        }

        public Entity GetChild(long guid)
        {
            if (childDic.ContainsKey(guid))
            {
                return childDic[guid];
            }

            return null;
        }

        public bool HasComponent(Type type)
        {
            if (componentDic.ContainsKey(type))
            {
                return true;
            }

            return false;
        }

        public bool HasComponent<T>() where T : Component
        {
            return HasComponent(typeof(T));
        }

        #region 添加组件

        public Component AddComponent(Component component)
        {
            Type type = component.GetType();
            if (type.FullName == "Model.ComponentAdapter+Adapter")
            {
                type = (component as ComponentAdapter.Adapter)?.ILInstance.Type.ReflectionType;
            }
            if (componentDic.ContainsKey(type))
            {
                return componentDic[type];
            }

            AddToComponentView(component);
            componentDic.Add(type, component);

            return component;
        }

        #endregion 添加组件

        #region 获取组件

        public T GetComponent<T>() where T : Component
        {
            return (T)GetComponent(typeof(T));
        }

        public Component GetComponent(Type type)
        {
            if (componentDic.TryGetValue(type, out Component component))
            {
                return component;
            }

            return null;
        }

        public IEnumerable<Component> GetComponents()
        {
            return componentDic.Values;
        }

        public Component[] GetComponentsToArray()
        {
            return componentDic.Values.ToArray();
        }

        #endregion 获取组件

        #region 删除组件

        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Type type)
        {
            if (componentDic.ContainsKey(type))
            {
                var component = componentDic[type];
                RemoveToComponentView(component);
                componentDic.Remove(type);

                Game.Instance.ObjectPool.RecycleComponent(component);
            }
        }

        public void RemoveComponent(Component component)
        {
            Type type = component.GetType();
            if (type.FullName == "Model.ComponentAdapter+Adapter")
            {
                type = (component as ComponentAdapter.Adapter)?.ILInstance.Type.ReflectionType;
            }
            if (componentDic.ContainsKey(type))
            {
                RemoveToComponentView(component);
                componentDic.Remove(type);

                Game.Instance.ObjectPool.RecycleComponent(component);
            }
        }

        #endregion 删除组件

        public static unsafe void RegisterILRuntimeCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            foreach (var i in typeof(Entity).GetMethods())
            {
                if (i.Name == "GetComponent" && i.IsGenericMethodDefinition)
                {
                    appdomain.RegisterCLRMethodRedirection(i, CLRGetComponent);
                }
                else if (i.Name == "RemoveComponent" && i.IsGenericMethodDefinition)
                {
                    appdomain.RegisterCLRMethodRedirection(i, CLRRemoveComponent);
                }
                else if (i.Name == "HasComponent" && i.IsGenericMethodDefinition)
                {
                    appdomain.RegisterCLRMethodRedirection(i, CLRHasComponent);
                }
            }
        }

        public static unsafe StackObject* CLRGetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            Entity instance = StackObject.ToObject(ptr, __domain, __mStack) as Entity;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.GetComponent(type.TypeForCLR);
                }
                else
                {
                    res = (instance.GetComponent(type.ReflectionType) as ComponentAdapter.Adapter)?.ILInstance;
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

        public static unsafe StackObject* CLRRemoveComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            Entity instance = StackObject.ToObject(ptr, __domain, __mStack) as Entity;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    instance.RemoveComponent(type.TypeForCLR);
                }
                else
                {
                    instance.RemoveComponent(type.ReflectionType);
                }
            }

            return __esp;
        }

        public static unsafe StackObject* CLRHasComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            Entity instance = StackObject.ToObject(ptr, __domain, __mStack) as Entity;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.HasComponent(type.TypeForCLR);
                }
                else
                {
                    res = instance.HasComponent(type.ReflectionType);
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }
    }
}