using Cysharp.Threading.Tasks;
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

            Game.Instance.LifecycleSystem.Add(component);

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

            Game.Instance.LifecycleSystem.Add(component);

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

            Game.Instance.LifecycleSystem.Add(component);

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
            if (type.Namespace == MODEL_NAMESPACE)
            {
                Model.ObjectHelper.RemoveComponent(type, entity);
                return;
            }
            Model.Component component = Model.ObjectHelper._RemoveComponent(type, entity);
            if (component != null)
            {
                Game.Instance.LifecycleSystem.Remove(component);
            }
        }

        public static void RemoveComponent<T>(Model.Entity entity)
        {
            RemoveComponent(typeof(T), entity);
        }

        #endregion RemoveComponent

        #region OpenUIView

        public static async UniTask<Model.UIBaseComponent> OpenUIView(Type type, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return await Model.ObjectHelper.OpenUIView(type, isCloseBack);
            }
            var component = await Model.ObjectHelper._OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen iOpen = component as IOpen;
                iOpen?.Open();
            }

            return component;
        }

        public static async UniTask<Model.UIBaseComponent> OpenUIView<A>(Type type, A a, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return await Model.ObjectHelper.OpenUIView(type, a, isCloseBack);
            }
            var component = await Model.ObjectHelper._OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A> iOpen = component as IOpen<A>;
                iOpen?.Open(a);
            }

            return component;
        }

        public static async UniTask<Model.UIBaseComponent> OpenUIView<A, B>(Type type, A a, B b, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return await Model.ObjectHelper.OpenUIView(type, a, b, isCloseBack);
            }
            var component = await Model.ObjectHelper._OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B> iOpen = component as IOpen<A, B>;
                iOpen?.Open(a, b);
            }

            return component;
        }

        public static async UniTask<Model.UIBaseComponent> OpenUIView<A, B, C>(Type type, A a, B b, C c, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return await Model.ObjectHelper.OpenUIView(type, a, b, c, isCloseBack);
            }
            var component = await Model.ObjectHelper._OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B, C> iOpen = component as IOpen<A, B, C>;
                iOpen?.Open(a, b, c);
            }

            return component;
        }

        public static async UniTask<T> OpenUIView<T>(bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A>(A a, bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A, B>(A a, B b, bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, b, isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A, B, C>(A a, B b, C c, bool isCloseBack = false) where T : Model.UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, b, c, isCloseBack);
        }

        #endregion OpenUIView

        #region CloseUIView

        public static void CloseUIView(Type type, bool isCloseBack = false)
        {
            Model.ObjectHelper.CloseUIView(type, isCloseBack);
        }

        public static void CloseUIView<T>(bool isCloseBack = false)
        {
            CloseUIView(typeof(T), isCloseBack);
        }

        #endregion CloseUIView

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
    }
}