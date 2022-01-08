using System;
using UnityEngine;

namespace Hotfix
{
    public class ObjectHelper
    {
        #region CreateComponent

        public static Model.Component CreateComponent(Type type, Model.Entity entity, bool isFromPool = true)
        {
            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);

            IAwake iAwake = component as IAwake;
            iAwake?.Awake();

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Model.Component CreateComponent<A>(Type type, Model.Entity entity, A a, bool isFromPool = true)
        {
            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);

            IAwake<A> iAwake = component as IAwake<A>;
            iAwake?.Awake(a);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Model.Component CreateComponent<A, B>(Type type, Model.Entity entity, A a, B b, bool isFromPool = true)
        {
            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);

            IAwake<A, B> iAwake = component as IAwake<A, B>;
            iAwake?.Awake(a, b);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Model.Component CreateComponent<A, B, C>(Type type, Model.Entity entity, A a, B b, C c, bool isFromPool = true)
        {
            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);

            IAwake<A, B, C> iAwake = component as IAwake<A, B, C>;
            iAwake?.Awake(a, b, c);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static T CreateComponent<T>(Model.Entity entity, bool isFromPool = true) where T : Model.Component
        {
            return (T)CreateComponent(typeof(T), entity, isFromPool);
        }

        public static T CreateComponent<T, A>(Model.Entity entity, A a, bool isFromPool = true) where T : Model.Component
        {
            return (T)CreateComponent(typeof(T), entity, a, isFromPool);
        }

        public static T CreateComponent<T, A, B>(Model.Entity entity, A a, B b, bool isFromPool = true) where T : Model.Component
        {
            return (T)CreateComponent(typeof(T), entity, a, b, isFromPool);
        }

        public static T CreateComponent<T, A, B, C>(Model.Entity entity, A a, B b, C c, bool isFromPool = true) where T : Model.Component
        {
            return (T)CreateComponent(typeof(T), entity, a, b, c, isFromPool);
        }

        #endregion CreateComponent

        #region RemoveComponent

        public static void RemoveComponent(Type type, Model.Entity entity)
        {
            Model.Component component = entity.GetComponent(type);
            if (component != null)
            {
                component.Dispose();
                entity.RemoveComponent(type);

                Game.Instance.LifecycleSystem.Remove(component);
            }
        }

        public static void RemoveComponent<T>(Model.Entity entity)
        {
            RemoveComponent(typeof(T), entity);
        }

        #endregion RemoveComponent

        #region OpenUIView

        public static Model.UIBaseComponent OpenUIView(Type type, bool isCloseBack = false)
        {
            var component = Model.ObjectHelper._OpenUIView(type, isCloseBack);
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

        public static Model.UIBaseComponent OpenUIView<A>(Type type, A a, bool isCloseBack = false)
        {
            var component = Model.ObjectHelper._OpenUIView(type, isCloseBack);
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

        public static Model.UIBaseComponent OpenUIView<A, B>(Type type, A a, B b, bool isCloseBack = false)
        {
            var component = Model.ObjectHelper._OpenUIView(type, isCloseBack);
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

        public static Model.UIBaseComponent OpenUIView<A, B, C>(Type type, A a, B b, C c, bool isCloseBack = false)
        {
            var component = Model.ObjectHelper._OpenUIView(type, isCloseBack);
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

        public static T OpenUIView<T>(bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), isCloseBack);
        }

        public static T OpenUIView<T, A>(A a, bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), a, isCloseBack);
        }

        public static T OpenUIView<T, A, B>(A a, B b, bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), a, b, isCloseBack);
        }

        public static T OpenUIView<T, A, B, C>(A a, B b, C c, bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)OpenUIView(typeof(T), a, b, c, isCloseBack);
        }

        #endregion OpenUIView
    }
}