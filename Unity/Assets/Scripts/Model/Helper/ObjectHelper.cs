using System;
using UnityEngine;

namespace Model
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

        public static Entity CreatEntity(Entity eParent, Transform parent = null, string sign = "OrdinaryGameObject", bool isFromAB = false)
        {
            Entity entity = Game.Instance.ObjectPool.HatchEntity();
            entity.GameObject = Game.Instance.ObjectPool.HatchGameObjByName(sign, isFromAB);
            entity.Sign = sign;

            entity.Transform = entity.GameObject.transform;
            entity.Transform.SetParent(parent == null ? eParent.Transform : parent);
            entity.SetParent(eParent);

            entity.AddComponentView();

            return entity;
        }

        public static UIBaseComponent OpenUIView(Type type, bool isCloseBack = false)
        {
            var component = Game.Instance.Scene.GetChild(UIManagerComponent.UIROOT_PATH).GetComponent<UIManagerComponent>().OpenUIView(type, isCloseBack);
            if (component == null)
            {
                Debug.LogError($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen iOpen = component as IOpen;

                iOpen?.Open();
            }

            return component;
        }

        public static UIBaseComponent OpenUIView<A>(Type type, A a, bool isCloseBack = false)
        {
            var component = Game.Instance.Scene.GetChild(UIManagerComponent.UIROOT_PATH).GetComponent<UIManagerComponent>().OpenUIView(type, isCloseBack);
            if (component == null)
            {
                Debug.LogError($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A> iOpen = component as IOpen<A>;

                iOpen?.Open(a);
            }

            return component;
        }

        public static UIBaseComponent OpenUIView<A, B>(Type type, A a, B b, bool isCloseBack = false)
        {
            var component = Game.Instance.Scene.GetChild(UIManagerComponent.UIROOT_PATH).GetComponent<UIManagerComponent>().OpenUIView(type, isCloseBack);
            if (component == null)
            {
                Debug.LogError($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B> iOpen = component as IOpen<A, B>;

                iOpen?.Open(a, b);
            }

            return component;
        }

        public static UIBaseComponent OpenUIView<A, B, C>(Type type, A a, B b, C c, bool isCloseBack = false)
        {
            var component = Game.Instance.Scene.GetChild(UIManagerComponent.UIROOT_PATH).GetComponent<UIManagerComponent>().OpenUIView(type, isCloseBack);
            if (component == null)
            {
                Debug.LogError($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B, C> iOpen = component as IOpen<A, B, C>;

                iOpen?.Open(a, b, c);
            }

            return component;
        }

        public static T OpenUIView<T>(bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), isCloseBack);
        }

        public static T OpenUIView<T, A>(A a, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), a, isCloseBack);
        }

        public static T OpenUIView<T, A, B>(A a, B b, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), a, b, isCloseBack);
        }

        public static T OpenUIView<T, A, B, C>(A a, B b, C c, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), a, b, c, isCloseBack);
        }

        public static void CloseUIView(Type type, bool isCloseBack = false)
        {
            Game.Instance.Scene.GetChild(UIManagerComponent.UIROOT_PATH).GetComponent<UIManagerComponent>().CloseUIView(type, isCloseBack);
        }

        public static void CloseUIView<T>(bool isCloseBack = false)
        {
            CloseUIView(typeof(T), isCloseBack);
        }
    }
}