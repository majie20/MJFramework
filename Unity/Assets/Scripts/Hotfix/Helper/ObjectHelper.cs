using System;

namespace Hotfix
{
    public class ObjectHelper
    {
        public const string MODEL_NAMESPACE = "Model";

        #region CreateComponent

        public static Model.Component CreateComponent(Type type, Model.Entity entity, bool isFromPool = true)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.ObjectHelper.CreateComponent(type, entity, isFromPool);
            }

            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);
            IAwake iAwake = component as IAwake;
            iAwake?.Awake();

#if ILRuntime
            Game.Instance.LifecycleSystem.Add(component);
#elif HybridCLR
            Model.Game.Instance.LifecycleSystem.Add(component);
#endif

            return component;
        }

        public static Model.Component CreateComponent<A>(Type type, Model.Entity entity, A a, bool isFromPool = true)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.ObjectHelper.CreateComponent(type, entity, a, isFromPool);
            }

            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);

            IAwake<A> iAwake = component as IAwake<A>;
            iAwake?.Awake(a);

#if ILRuntime
            Game.Instance.LifecycleSystem.Add(component);
#elif HybridCLR
            Model.Game.Instance.LifecycleSystem.Add(component);
#endif

            return component;
        }

        public static Model.Component CreateComponent<A, B>(Type type, Model.Entity entity, A a, B b, bool isFromPool = true)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.ObjectHelper.CreateComponent(type, entity, a, b, isFromPool);
            }

            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);

            IAwake<A, B> iAwake = component as IAwake<A, B>;
            iAwake?.Awake(a, b);

#if ILRuntime
            Game.Instance.LifecycleSystem.Add(component);
#elif HybridCLR
            Model.Game.Instance.LifecycleSystem.Add(component);
#endif

            return component;
        }

        public static Model.Component CreateComponent<A, B, C>(Type type, Model.Entity entity, A a, B b, C c, bool isFromPool = true)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.ObjectHelper.CreateComponent(type, entity, a, b, c, isFromPool);
            }

            Model.Component component = Model.ObjectHelper._CreateComponent(type, entity, isFromPool);

            IAwake<A, B, C> iAwake = component as IAwake<A, B, C>;
            iAwake?.Awake(a, b, c);

#if ILRuntime
            Game.Instance.LifecycleSystem.Add(component);
#elif HybridCLR
            Model.Game.Instance.LifecycleSystem.Add(component);
#endif

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
            if (type.Namespace == MODEL_NAMESPACE)
            {
                Model.ObjectHelper.RemoveComponent(type, entity);

                return;
            }

            Model.Component component = Model.ObjectHelper._RemoveComponent(type, entity);

            if (component != null)
            {
#if ILRuntime
                Game.Instance.LifecycleSystem.Remove(component);
#elif HybridCLR
            Model.Game.Instance.LifecycleSystem.Remove(component);
#endif
            }
        }

        public static void RemoveComponent<T>(Model.Entity entity)
        {
            RemoveComponent(typeof(T), entity);
        }

        #endregion RemoveComponent

#if ILRuntime

        #region Lifecycle

        public static void AddLifecycle(Model.Component component)
        {
            Game.Instance.LifecycleSystem.Add(component);
        }

        public static void RemoveLifecycle(Model.Component component)
        {
            Game.Instance.LifecycleSystem.Remove(component);
        }

        #endregion Lifecycle

#endif
    }
}