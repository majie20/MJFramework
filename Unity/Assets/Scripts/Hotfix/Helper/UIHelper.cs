using Cysharp.Threading.Tasks;
using System;

namespace Hotfix
{
    public class UIHelper
    {
        public const string MODEL_NAMESPACE = "Model";

        #region OpenUIView

        public static async UniTask<Model.UIBaseComponent> OpenUIView(Type type, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return await Model.UIHelper.OpenUIView(type, isCloseBack);
            }

            var component = await Model.UIHelper._OpenUIView(type, isCloseBack);

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
                return await Model.UIHelper.OpenUIView(type, a, isCloseBack);
            }

            var component = await Model.UIHelper._OpenUIView(type, isCloseBack);

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
                return await Model.UIHelper.OpenUIView(type, a, b, isCloseBack);
            }

            var component = await Model.UIHelper._OpenUIView(type, isCloseBack);

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
                return await Model.UIHelper.OpenUIView(type, a, b, c, isCloseBack);
            }

            var component = await Model.UIHelper._OpenUIView(type, isCloseBack);

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
            Model.UIHelper.CloseUIView(type, isCloseBack);
        }

        public static void CloseUIView<T>(bool isCloseBack = false)
        {
            CloseUIView(typeof(T), isCloseBack);
        }

        #endregion CloseUIView
    }
}