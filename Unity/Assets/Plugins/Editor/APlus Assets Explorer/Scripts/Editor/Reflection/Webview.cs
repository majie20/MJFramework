//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

namespace APlus
{
    [SerializableAttribute]
    public sealed class CallbackWrapper
    {
        public object callback;

        public CallbackWrapper(object callback)
        {
            this.callback = callback;
        }

        public void Send(string message)
        {
            Webview.Callback(this.callback, message);
        }
    }

    [SerializableAttribute]
    public sealed class Webview  : ScriptableObject
    {
        private delegate void InitWebViewDelegate(
                object instance,
                object view,
                int x,
                int y,
                int width,
                int height,
                bool handles
        );

        public ScriptableObject web;
        private static Func<ScriptableObject> _Create;

        /// relection methods
        ///
        private static Action<object, bool> _AllowRightClickMenu;
        private static Action<object> _Back;
        private static Func<object, string, ScriptableObject, bool> _DefineScriptObject;
        private static Action<object, string> _ExecuteJavascript;
        private static Action<object> _Forward;
        private static Func<object, bool> _HasApplicationFocus;
        private static Action<object> _Hide;
        private static InitWebViewDelegate _InitWebView;
        private static Action<object, string> _LoadFile;
        private static Action<object, string> _LoadURL;
        private static Action<object> _OnDestory;
        private static Action<object> _Reload;
        private static Action<object, string> _SendOnEvent;
        private static Action<object, bool> _SetApplicationFocus;
        private static Action<object, ScriptableObject> _SetDelegateObject;
        private static Action<object, bool> _SetFocus;
        private static Action<object, object> _SetHostView;
        private static Action<object, Rect> _SetSizeAndPosition;
        private static Action<object> _Show;
        private static Action<object> _ShowDevTools;
        private static Action<object> _ToggleMaximinze;

        /// public static methods
        ///
        public static Func<object, object> GetView;
        public static Action<object, string> Callback;

        /// constructor
        ///
        public void Init()
        {
            web = _Create();
            web.hideFlags = HideFlags.HideAndDontSave;
        }

        static Webview()
        {
            var editorAssembly = typeof(Editor).Assembly;
            var webViewType = editorAssembly.GetType("UnityEditor.WebView");
            MethodsMapping(webViewType);

            var v8CallBackCsharp = editorAssembly.GetType("UnityEditor.WebViewV8CallbackCSharp");
            ReflectionUtils.RegisterMethod(v8CallBackCsharp, "Callback", ref Callback);
        }

        private static void MethodsMapping(Type webViewType)
        {
            _Create = () => { return ScriptableObject.CreateInstance(webViewType); };

            var parent = typeof(EditorWindow).GetField("m_Parent", ReflectionUtils.BIND_FLAGS);
            GetView = (obj) =>
            {
                return parent.GetValue(obj);
            };

            ReflectionUtils.RegisterMethod(webViewType, "Back", ref _Back);
            ReflectionUtils.RegisterMethod<string, ScriptableObject, bool>(webViewType, "DefineScriptObject", ref _DefineScriptObject);
            ReflectionUtils.RegisterMethod<string>(webViewType, "ExecuteJavascript", ref _ExecuteJavascript);
            ReflectionUtils.RegisterMethod(webViewType, "Forward", ref _Forward);



            var initMethod = webViewType.GetMethod("InitWebView");
            _InitWebView = (obj, view, x, y, width, height, handles) =>
            {
                initMethod.Invoke(obj, new[] { view, x, y, width, height, handles });
            };

            ReflectionUtils.RegisterMethod<string>(webViewType, "LoadFile", ref _LoadFile);
            ReflectionUtils.RegisterMethod<string>(webViewType, "LoadURL", ref _LoadURL);
            ReflectionUtils.RegisterMethod(webViewType, "Reload", ref _Reload);
            ReflectionUtils.RegisterMethod<string>(webViewType, "SendOnEvent", ref _SendOnEvent);
            ReflectionUtils.RegisterMethod<ScriptableObject>(webViewType, "SetDelegateObject", ref _SetDelegateObject);
            ReflectionUtils.RegisterMethod<bool>(webViewType, "SetFocus", ref _SetFocus);
            ReflectionUtils.RegisterMethod<object>(webViewType, "SetHostView", ref _SetHostView);

            var setRectMethod = webViewType.GetMethod("SetSizeAndPosition");
            _SetSizeAndPosition = (obj, rect) =>
            {
                setRectMethod.Invoke(obj, new object[] { (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height });
            };

            ReflectionUtils.RegisterMethod(webViewType, "Show", ref _Show);
            ReflectionUtils.RegisterMethod(webViewType, "Hide", ref _Hide);
            // ReflectionUtils.RegisterMethod(webViewType, "OnDestory", ref _OnDestory);
            ReflectionUtils.RegisterMethod(webViewType, "ShowDevTools", ref _ShowDevTools);

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
            ReflectionUtils.RegisterMethod<bool>(webViewType, "HasApplicationFocus", ref _HasApplicationFocus);
            ReflectionUtils.RegisterMethod(webViewType, "ToggleMaximize", ref _ToggleMaximinze);
            ReflectionUtils.RegisterMethod<bool>(webViewType, "SetApplicationFocus", ref _SetApplicationFocus);
            ReflectionUtils.RegisterMethod<bool>(webViewType, "AllowRightClickMenu", ref _AllowRightClickMenu);
#endif
        }

        /// Initialize the position of webview
        ///
        public void InitWebView(object view, Rect rect, bool handles)
        {
            if(web == null)
            {
                Init();
            }
            
            _InitWebView(web, view, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, handles);
        }

        public void Back()
        {
            _Back(web);
        }

        public void DefineScriptObject(string className, ScriptableObject sobject)
        {
            _DefineScriptObject(web, className, sobject);
        }

        public void ExecuteJavascript(string script)
        {
            if(web != null)
            {
                _ExecuteJavascript(web, script);
            }
        }

        public void Forward()
        {
            _Forward(web);
        }

        public void Hide()
        {
            _Hide(web);
        }

        public void LoadFile(string file)
        {
            _LoadFile(web, file);
        }

        /// Open Url
        ///    
        public void LoadURL(string url)
        {
            _LoadURL(web, url);
        }

        public void OnDestory()
        {
            if(web != null)
            {
                SetHostView(null);
                UnityEngine.Object.DestroyImmediate(web);
            }
        }

        /// reload webpage
        ///
        public void Reload()
        {
            _Reload(web);
        }

        public void SendOnEvent(string s)
        {
            _SendOnEvent(web, s);
        }

        public void SetDelegateObject(ScriptableObject sobject)
        {
            _SetDelegateObject(web, sobject);
        }

        public void SetFocus(bool focus)
        {
            _SetFocus(web, focus);
        }

        public void SetHostView(object view)
        {
            _SetHostView(web, view);
        }

        /// set position of webview
        ///
        public void SetSizeAndPosition(Rect rect)
        {
            _SetSizeAndPosition(this.web, rect);
        }

        public void Show()
        {
            _Show(web);
        }

        public void ShowDevTools()
        {
            _ShowDevTools(web);
        }

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
        public void ToggleMaximize()
        {
            _ToggleMaximinze(web);
        }

        public void SetApplicationFocus(bool focus)
        {
            _SetApplicationFocus(web, focus);
        }

        public bool HasApplicationFocus()
        {
            return _HasApplicationFocus(web);
        }

        public void AllowRightClickMenu(bool allow)
        {
            _AllowRightClickMenu(web, allow);
        }

#endif
    }
}
#endif

