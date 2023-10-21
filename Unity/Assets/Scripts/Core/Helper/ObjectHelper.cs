using System;
using System.Reflection;
using UnityEngine;

namespace Model
{
    public class ObjectHelper
    {
        #region CreateComponent

        public static Component _CreateComponent(Type type, Entity entity, bool isFromPool = true)
        {
            Component component;

            if (isFromPool)
            {
                component = Game.Instance.Scene.GetComponent<ComponentPoolComponent>().HatchComponent(type);
            }
            else
            {
#if ILRuntime
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    component = Game.Instance.Hotfix.AppDomain.Instantiate<Component>(type.FullName);
                }
                else
                {
                    component = (Component)Activator.CreateInstance(type);
                }
#else
                component = (Component)Activator.CreateInstance(type);
#endif
            }

            entity.AddComponent(component);
            component.Entity = entity;
            component.IsRuning = true;
            component.AddComponentParent();

            return component;
        }

        public static Component CreateComponent(Type type, Entity entity, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake iAwake = component as IAwake;
            iAwake?.Awake();

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Component CreateComponent<A>(Type type, Entity entity, A a, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake<A> iAwake = component as IAwake<A>;
            iAwake?.Awake(a);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Component CreateComponent<A, B>(Type type, Entity entity, A a, B b, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake<A, B> iAwake = component as IAwake<A, B>;
            iAwake?.Awake(a, b);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Component CreateComponent<A, B, C>(Type type, Entity entity, A a, B b, C c, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake<A, B, C> iAwake = component as IAwake<A, B, C>;
            iAwake?.Awake(a, b, c);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static T CreateComponent<T>(Entity entity, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, isFromPool);
        }

        public static T CreateComponent<T, A>(Entity entity, A a, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, a, isFromPool);
        }

        public static T CreateComponent<T, A, B>(Entity entity, A a, B b, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, a, b, isFromPool);
        }

        public static T CreateComponent<T, A, B, C>(Entity entity, A a, B b, C c, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, a, b, c, isFromPool);
        }

        public static void CreateComponents(Entity entity, params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
#if ILRuntime
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    ILRuntime.CLR.Method.IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.CreateComponent3"];

                    using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                    {
                        ctx.PushObject(type);
                        ctx.PushObject(entity);
                        ctx.PushBool(true);
                        ctx.Invoke();
                    }
                }
                else
                {
                    CreateComponent(type, entity);
                }
#else
                CreateComponent(type, entity);
#endif
            }
        }

        #endregion CreateComponent

        #region RemoveComponent

        public static Component _RemoveComponent(Type type, Entity entity)
        {
            Component component = entity.GetComponent(type);

            if (component != null)
            {
                component.Dispose();
                component.IsRuning = false;
                entity.RemoveComponent(type);
                Game.Instance.Scene.GetComponent<ComponentPoolComponent>().RecycleComponent(component);

                return component;
            }

            return null;
        }

        public static void RemoveComponent(Type type, Entity entity)
        {
            Component component = _RemoveComponent(type, entity);

            if (component != null)
            {
                Game.Instance.LifecycleSystem.Remove(component);
            }
        }

        public static void RemoveComponent<T>(Entity entity)
        {
            RemoveComponent(typeof(T), entity);
        }

        #endregion RemoveComponent

        #region CreateEntity

        public static T CreateUIEntity<T>(Entity eParent, string sign) where T : Entity
        {
            var entity = Game.Instance.Scene.GetComponent<EntityPoolComponent>().HatchEntity<T>();
            entity.Sign = sign;
            entity.SetParent(eParent);
            entity.AwakeCalled = true;
            entity.Called = false;

            return entity;
        }

        public static T CreateEntity<T>(Entity eParent, Transform parent = null, string sign = "OrdinaryGameObject", bool isFromAB = false, bool isParentCanNull = false)
            where T : Entity
        {
            var entity = Game.Instance.Scene.GetComponent<EntityPoolComponent>().HatchEntity<T>();

            entity.GameObject = Game.Instance.Scene.GetComponent<GameObjPoolComponent>()
               .HatchGameObjBySign(sign, (!isParentCanNull && parent == null) ? eParent.Transform : parent, isFromAB);
            entity.Sign = sign;
            entity.Transform = entity.GameObject.transform;
            entity.SetParent(eParent);
            entity.AddComponentView();
            entity.AwakeCalled = true;
            entity.Called = false;

            if (entity.Transform.TryGetComponent<EntityIdHandle>(out var idHandle))
            {
                idHandle.Guid = entity.Guid;
            }

            return entity;
        }

        public static T CreateEntity<T>(Entity eParent, GameObject obj) where T : Entity
        {
            var entity = Game.Instance.Scene.GetComponent<EntityPoolComponent>().HatchEntity<T>();
            entity.GameObject = obj;
            entity.Sign = GameObjPoolComponent.None_GameObject;
            entity.Transform = entity.GameObject.transform;
            entity.SetParent(eParent);
            entity.AddComponentView();
            entity.AwakeCalled = true;
            entity.Called = false;

            if (entity.Transform.TryGetComponent<EntityIdHandle>(out var idHandle))
            {
                idHandle.Guid = entity.Guid;
            }

            return entity;
        }

        #endregion CreateEntity

        #region RemoveEntity

        public static void RemoveEntity(Entity entity)
        {
            var idHandle = entity.Transform.GetComponent<EntityIdHandle>();

            if (idHandle != null)
            {
                idHandle.Guid = 0;
            }

            entity.Dispose();
            Game.Instance.Scene.GetComponent<EntityPoolComponent>().RecycleEntity(entity);
        }

        #endregion RemoveEntity

        public static FunctionSortAttribute GetFunctionSortAttribute(Type type, string methodName)
        {
#if ILRuntime
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                var attrs = type.GetMethod(methodName).GetCustomAttributes(typeof(FunctionSortAttribute), false);

                if (attrs.Length > 0)
                {
                    if (attrs[0] is FunctionSortAttribute attr)
                    {
                        return attr;
                    }
                }
            }
            else
            {
                return type.GetMethod(methodName).GetCustomAttribute<FunctionSortAttribute>();
            }

            return null;
#else
            return type.GetMethod(methodName).GetCustomAttribute<FunctionSortAttribute>();
#endif
        }
    }
}