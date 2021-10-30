using System;

namespace MGame.Hotfix
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

        public static T CreateComponent<T>(Entity entity, bool isFromPool = true) where T : Component
        {
            Type type = typeof(T);

            T component;
            if (isFromPool)
            {
                component = (T)Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (T)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component);

            return component;
        }

        public static T CreateComponent<T, A>(Entity entity, A a, bool isFromPool = true) where T : Component
        {
            Type type = typeof(T);

            T component;
            if (isFromPool)
            {
                component = (T)Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (T)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component, a);

            return component;
        }

        public static T CreateComponent<T, A, B>(Entity entity, A a, B b, bool isFromPool = true) where T : Component
        {
            Type type = typeof(T);

            T component;
            if (isFromPool)
            {
                component = (T)Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (T)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component, a, b);

            return component;
        }

        public static T CreateComponent<T, A, B, C>(Entity entity, A a, B b, C c, bool isFromPool = true) where T : Component
        {
            Type type = typeof(T);

            T component;
            if (isFromPool)
            {
                component = (T)Game.Instance.ObjectPool.HatchComponent(type);
            }
            else
            {
                component = (T)Activator.CreateInstance(type);
            }

            entity.AddComponent(component);
            component.Entity = entity;
            Game.Instance.LifecycleSystem.Add(component);
            Game.Instance.LifecycleSystem.Awake(component, a, b, c);

            return component;
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