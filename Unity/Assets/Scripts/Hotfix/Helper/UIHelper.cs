using System;

namespace Hotfix
{
    public class UIHelper
    {
        public const string MODEL_NAMESPACE = "Model";

        #region OpenUIView

        public static Model.UIBaseComponent OpenUIView(Type type, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.UIHelper.OpenUIView(type, isCloseBack);
            }

            var component = Model.UIHelper._OpenUIView(type, isCloseBack);

            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen iOpen = component as IOpen;
                iOpen?.Open();

                if (component.IsLoadComplete)
                {
                    component.RefreshEnable();
                    component.OnLoadComplete().Forget();
                }
            }

            return component;
        }

        public static Model.UIBaseComponent OpenUIView<A>(Type type, A a, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.UIHelper.OpenUIView(type, a, isCloseBack);
            }

            var component = Model.UIHelper._OpenUIView(type, isCloseBack);

            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A> iOpen = component as IOpen<A>;
                iOpen?.Open(a);

                if (component.IsLoadComplete)
                {
                    component.RefreshEnable();
                    component.OnLoadComplete().Forget();
                }
            }

            return component;
        }

        public static Model.UIBaseComponent OpenUIView<A, B>(Type type, A a, B b, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.UIHelper.OpenUIView(type, a, b, isCloseBack);
            }

            var component = Model.UIHelper._OpenUIView(type, isCloseBack);

            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B> iOpen = component as IOpen<A, B>;
                iOpen?.Open(a, b);

                if (component.IsLoadComplete)
                {
                    component.RefreshEnable();
                    component.OnLoadComplete().Forget();
                }
            }

            return component;
        }

        public static Model.UIBaseComponent OpenUIView<A, B, C>(Type type, A a, B b, C c, bool isCloseBack = false)
        {
            if (type.Namespace == MODEL_NAMESPACE)
            {
                return Model.UIHelper.OpenUIView(type, a, b, c, isCloseBack);
            }

            var component = Model.UIHelper._OpenUIView(type, isCloseBack);

            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B, C> iOpen = component as IOpen<A, B, C>;
                iOpen?.Open(a, b, c);

                if (component.IsLoadComplete)
                {
                    component.RefreshEnable();
                    component.OnLoadComplete().Forget();
                }
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