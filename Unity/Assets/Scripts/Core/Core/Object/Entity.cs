#if ILRuntime
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
#endif

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Model
{
    public class Entity : IDisposable
    {
        protected Dictionary<Type, Component> _componentDic;
        protected Dictionary<long, Entity>    _childDic;
        protected ComponentView               _componentView;

        private Entity _parent;

        public Entity Parent
        {
            protected set { _parent = value; }
            get { return _parent; }
        }

        private GameObject _gameObject;

        public GameObject GameObject
        {
            set { _gameObject = value; }
            get { return _gameObject; }
        }

        private Transform _transform;

        public Transform Transform
        {
            set { _transform = value; }
            get { return _transform; }
        }

        private string _sign;

        public string Sign
        {
            set { _sign = value; }
            get { return _sign; }
        }

        private long _guid;

        public long Guid
        {
            set { _guid = value; }
            get { return _guid; }
        }

        private bool _isDispose;

        public bool IsDispose
        {
            protected set { _isDispose = value; }
            get { return _isDispose; }
        }

        private EventSystem _eventSystem;

        public EventSystem EventSystem
        {
            set { _eventSystem = value; }
            get { return _eventSystem; }
        }

        #region Task

        public    bool                    AwakeCalled = false;
        public    bool                    Called      = false;
        protected CancellationTokenSource _cancellationTokenSource;

        public CancellationToken CancellationToken
        {
            get
            {
                if (_cancellationTokenSource is null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }

                if (!AwakeCalled)
                {
                    PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
                }

                return _cancellationTokenSource.Token;
            }
        }

        private class AwakeMonitor : IPlayerLoopItem
        {
            private readonly Entity trigger;

            public AwakeMonitor(Entity trigger)
            {
                this.trigger = trigger;
            }

            public bool MoveNext()
            {
                if (trigger.Called) return false;

                if (trigger == null)
                {
                    trigger.Dispose();

                    return false;
                }

                return true;
            }
        }

        #endregion Task

        public Entity()
        {
            _componentDic = new Dictionary<Type, Component>();
            _childDic = new Dictionary<long, Entity>();
        }

        public virtual void Dispose()
        {
            IsDispose = true;

            DisposeTimer();

            Parent.RemoveChild(Guid);

            if (_childDic.Count > 0)
            {
                foreach (var child in _childDic.Values)
                {
                    child.Dispose();
                }

                _childDic.Clear();
            }

            if (_componentDic.Count > 0)
            {
                var types = this._componentDic.Keys.ToArray();

                for (int i = 0; i < types.Length; i++)
                {
#if ILRuntime
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
#else
                    ObjectHelper.RemoveComponent(types[i], this);
#endif
                }

                _componentDic.Clear();
            }

            _componentView = null;
            Parent = null;

            if (GameObject != null)
            {
                if (Sign != GameObjPoolComponent.None_GameObject)
                {
                    Game.Instance.Scene.GetComponent<GameObjPoolComponent>().RecycleGameObj(Sign, GameObject);
                }

                Transform = null;
                GameObject = null;
            }

            IsDispose = false;
        }

        public void AddComponentView()
        {
            var component = GameObject.GetComponent<Model.ComponentView>();
            _componentView = component == null ? GameObject.AddComponent<Model.ComponentView>() : component;
        }

        private void AddToComponentView(Component component)
        {
#if ILRuntime
            if (component is ComponentAdapter.Adapter componentAdapter)
            {
                _componentView.dic.Add(component, componentAdapter.ILInstance.Type.ReflectionType);
            }
            else
            {
                var type = component.GetType();
                _componentView.dic.Add(component, type);
            }
#else
            var type = component.GetType();
            _componentView.dic.Add(component, type);
#endif
        }

        private void RemoveToComponentView(Component component)
        {
            _componentView.dic.Remove(component);
        }

        public void SetParent(Entity entity)
        {
            this.Parent = entity;
            this.Parent.AddChild(this);
        }

        public void AddChild(Entity child)
        {
            if (!_childDic.ContainsKey(child.Guid))
            {
                _childDic.Add(child.Guid, child);
            }
        }

        public Entity GetChild(long guid)
        {
            if (_childDic.TryGetValue(guid, out var entity))
            {
                return entity;
            }

            return null;
        }

        public void DisposeTimer()
        {
            AwakeCalled = false;
            Called = true;

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public void RemoveChild(long guid)
        {
            if (!IsDispose)
            {
                _childDic.Remove(guid);
            }
        }

        public bool HasComponent(Type type)
        {
            if (_componentDic.ContainsKey(type))
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

        public void AddComponent(Component component)
        {
            Type type = component.GetType();
#if ILRuntime
            if (component is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType croos)
            {
                type = croos.ILInstance.Type.ReflectionType;
            }
#endif
            if (!_componentDic.ContainsKey(type))
            {
                AddToComponentView(component);
                _componentDic.Add(type, component);
            }
        }

        #endregion 添加组件

        #region 获取组件

        public T GetComponent<T>() where T : Component
        {
            return (T)GetComponent(typeof(T));
        }

        public Component GetComponent(Type type)
        {
            if (_componentDic.TryGetValue(type, out Component component))
            {
                return component;
            }

            return null;
        }

        public IEnumerable<Component> GetComponents()
        {
            return _componentDic.Values;
        }

        public Component[] GetComponentsToArray()
        {
            return _componentDic.Values.ToArray();
        }

        #endregion 获取组件

        #region 删除组件

        public void RemoveComponent(Type type)
        {
            if (_componentDic.TryGetValue(type, out var component))
            {
                RemoveToComponentView(component);
                _componentDic.Remove(type);
            }
        }

        #endregion 删除组件

#if ILRuntime
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
#endif
    }
}