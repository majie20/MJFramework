using System;

namespace Hotfix
{
    public class ObjectHelper
    {
        public static Component CreateComponent(Type type, Entity entity, bool isFromPool = true)
        {
            Component component;
            if (isFromPool)
            {
                component = Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (Component)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component);

            return component;
        }

        public static Component CreateComponent<A>(Type type, Entity entity, A a, bool isFromPool = true)
        {
            Component component;
            if (isFromPool)
            {
                component = Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (Component)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component, a);

            return component;
        }

        public static Component CreateComponent<A, B>(Type type, Entity entity, A a, B b, bool isFromPool = true)
        {
            Component component;
            if (isFromPool)
            {
                component = Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (Component)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component, a, b);

            return component;
        }

        public static Component CreateComponent<A, B, C>(Type type, Entity entity, A a, B b, C c, bool isFromPool = true)
        {
            Component component;
            if (isFromPool)
            {
                component = Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (Component)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component, a, b, c);

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

        public static Entity CreatEntity(Entity parent, string sign = "OrdinaryGameObject", bool isFromAB = false)
        {
            Entity entity = Game.Instance.ObjectPool.HatchEntity();
            entity.GameObject = Game.Instance.ObjectPool.HatchGameObjByName(sign, isFromAB);
            entity.Sign = sign;

            entity.Transform = entity.GameObject.transform;
            entity.Transform.SetParent(parent.Transform);

            entity.AddComponentView();

            return entity;
        }
    }
}