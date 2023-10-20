using Cysharp.Threading.Tasks;
using System;
using System.Reflection;

namespace Model
{
    public class UIHelper
    {
        #region OpenUIView

        public static async UniTask<UIBaseComponent> _OpenUIView(Type type, bool isCloseBack = false)
        {
            var component = await Game.Instance.GGetComponent<UI2DRootComponent>().OpenUIView(type, isCloseBack);
            component.Canvas.enabled = true;
            component.IsEnable = true;

            return component;
        }

        public static async UniTask<UIBaseComponent> OpenUIView(Type type, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);

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

        public static async UniTask<UIBaseComponent> OpenUIView<A>(Type type, A a, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);

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

        public static async UniTask<UIBaseComponent> OpenUIView<A, B>(Type type, A a, B b, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);

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

        public static async UniTask<UIBaseComponent> OpenUIView<A, B, C>(Type type, A a, B b, C c, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);

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

        public static async UniTask<T> OpenUIView<T>(bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A>(A a, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A, B>(A a, B b, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, b, isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A, B, C>(A a, B b, C c, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, b, c, isCloseBack);
        }

        #endregion OpenUIView

        #region CloseUIView

        public static void CloseUIView(Type type, bool isCloseBack = false)
        {
            Game.Instance.GGetComponent<UI2DRootComponent>().CloseUIView(type, isCloseBack);
        }

        public static void CloseUIView<T>(bool isCloseBack = false)
        {
            CloseUIView(typeof(T), isCloseBack);
        }

        #endregion CloseUIView

        public static UIBaseDataAttribute GetUIBaseDataAttribute(Type type)
        {
#if ILRuntime
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                var attrs = type.GetCustomAttributes(typeof(UIBaseDataAttribute), false);

                if (attrs.Length > 0)
                {
                    if (attrs[0] is UIBaseDataAttribute attr)
                    {
                        return attr;
                    }
                }
            }
            else
            {
                return type.GetCustomAttribute<UIBaseDataAttribute>();
            }

            return null;
#else
            return type.GetCustomAttribute<UIBaseDataAttribute>();
#endif
        }
    }
}