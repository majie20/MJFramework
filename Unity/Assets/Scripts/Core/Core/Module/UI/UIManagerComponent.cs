using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class UIManagerComponent : Component, IAwake
    {
        private static int SORT_ORDER_INIT    = -32000;
        private static int SORT_ORDER_SPACING = 2000;

        private Dictionary<Type, UIBaseComponent> _uiComponentDic;
        private Stack<Type>                       _uiStack;
        private Stack<Type>                       _tempUIStack;

        private int    _sortOrder;
        public  Canvas Canvas;

        public void Awake()
        {
            _uiComponentDic = new Dictionary<Type, UIBaseComponent>(20);
            _uiStack = new Stack<Type>(5);
            _tempUIStack = new Stack<Type>();
            _sortOrder = SORT_ORDER_INIT;

            Canvas = this.Entity.Transform.GetComponent<Canvas>();
        }

        public override void Dispose()
        {
            Canvas = null;
            _uiComponentDic = null;
            _uiStack = null;
            _tempUIStack = null;
            base.Dispose();
        }

        public void CloseUpperMostView()
        {
            if (_uiStack.Count > 0)
            {
                while (true)
                {
                    var type = _uiStack.Peek();
                    var attr = UIHelper.GetUIBaseDataAttribute(type);
                    var uiViewType = (UIViewType)attr.UIViewType;
                    CloseUIView(type, attr, false);

                    if (uiViewType != UIViewType.Tips && uiViewType != UIViewType.None && attr.IsOperateMask)
                    {
                        break;
                    }
                }
            }
        }

        private UIBaseComponent CreateView(Type type, UIBaseDataAttribute attr)
        {
            if (_uiComponentDic.TryGetValue(type, out var component))
            {
                component.IsRuning = true;
                component.Entity.AwakeCalled = true;
                component.Entity.Called = false;

#if ILRuntime
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    ILRuntime.CLR.Method.IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.AddLifecycle"];

                    using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                    {
                        ctx.PushObject(component);
                        ctx.Invoke();
                    }
                }
                else
                {
                    Game.Instance.LifecycleSystem.Add(component);
                }
#else
                Game.Instance.LifecycleSystem.Add(component);
#endif
            }
            else
            {
#if ILRuntime
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    ILRuntime.CLR.Method.IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.CreateComponent3"];

                    using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                    {
                        ctx.PushObject(type);
                        ctx.PushObject(ObjectHelper.CreateUIEntity<Entity>(this.Entity, attr.PrefabPath));
                        ctx.PushBool(true);
                        ctx.Invoke();
                        component = ctx.ReadObject<UIBaseComponent>(0);
                    }
                }
                else
                {
                    component = ObjectHelper.CreateComponent(type, ObjectHelper.CreateUIEntity<Entity>(this.Entity, attr.PrefabPath)) as UIBaseComponent;
                }
#else
                component = ObjectHelper.CreateComponent(type, ObjectHelper.CreateUIEntity<Entity>(this.Entity, attr.PrefabPath)) as UIBaseComponent;
#endif
                component.IsLoadComplete = false;
                component.IsOpen = false;
                _uiComponentDic.Add(type, component);
            }

            component.CreateView(attr).Forget();

            return component;
        }

        private void PushView(Type type)
        {
            _uiComponentDic[type].SetSortingOrder(_sortOrder);
            _sortOrder += SORT_ORDER_SPACING;
            _uiStack.Push(type);
        }

        private void PopView()
        {
            _sortOrder -= SORT_ORDER_SPACING;
            _uiStack.Pop();
        }

        public void Clear()
        {
            while (_uiStack.Count > 0)
            {
                var type = _uiStack.Peek();
                CloseUIView(type, UIHelper.GetUIBaseDataAttribute(type), false);
            }

            _sortOrder = SORT_ORDER_INIT;
        }

        public void SetUIMask(bool isHide)
        {
            _tempUIStack.Clear();

            while (_uiStack.Count > 0)
            {
                var type = _uiStack.Peek();
                var attr = UIHelper.GetUIBaseDataAttribute(type);

                if (attr.IsOperateMask)
                {
                    if (isHide)
                    {
                        _uiComponentDic[type].SetMaskMode(UIMaskMode.None);
                    }
                    else
                    {
                        _uiComponentDic[type].SetMaskMode((UIMaskMode)attr.UIMaskMode);
                    }

                    while (_tempUIStack.Count > 0)
                    {
                        _uiStack.Push(_tempUIStack.Pop());
                    }

                    return;
                }

                _tempUIStack.Push(_uiStack.Pop());
            }

            while (_tempUIStack.Count > 0)
            {
                _uiStack.Push(_tempUIStack.Pop());
            }
        }

        public UIBaseComponent OpenUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack)
        {
            CloseUIView(type, attr, isCloseBack, true);

            var view = CreateView(type, attr);

            if (attr.IsOperateMask)
            {
                view.SetMaskMode((UIMaskMode)attr.UIMaskMode);
            }
            else
            {
                view.SetMaskMode((UIMaskMode)1);
            }

            PushView(type);

            return view;
        }

        public void CloseUIView(Type type)
        {
            UIBaseComponent component = _uiComponentDic[type];

            component.Close();
            component.IsRuning = false;
            component.Entity.DisposeTimer();
#if ILRuntime
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                ILRuntime.CLR.Method.IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.RemoveLifecycle"];

                using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                {
                    ctx.PushObject(component);
                    ctx.Invoke();
                }
            }
            else
            {
                Game.Instance.LifecycleSystem.Remove(component);
            }
#else
            Game.Instance.LifecycleSystem.Remove(component);
#endif
        }

        public void CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack)
        {
            CloseUIView(type, attr, isCloseBack, false);
        }

        private void CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack, bool isOpen)
        {
            var uiViewType = (UIViewType)attr.UIViewType;

            if (_uiStack.Contains(type))
            {
                if (uiViewType == UIViewType.Normal)
                {
                    Stack<Type> tempUIStack = new Stack<Type>();

                    while (true)
                    {
                        var tempType = _uiStack.Peek();

                        if (tempType == type)
                        {
                            PopView();

                            while (tempUIStack.Count > 0)
                            {
                                PushView(tempUIStack.Pop());
                            }

                            var normal = (int)UIViewType.Normal;

                            while (_uiStack.Count > 0)
                            {
                                var tempType1 = _uiStack.Peek();
                                var tempAttr1 = UIHelper.GetUIBaseDataAttribute(tempType1);

                                if (tempAttr1.UIViewType == normal)
                                {
                                    UIBaseComponent component = _uiComponentDic[tempType1];

                                    if (!(isOpen || component.IsEnable))
                                    {
                                        component.Enable();
                                    }
                                    else if (isOpen && component.IsEnable)
                                    {
                                        component.Disable();
                                    }

                                    break;
                                }

                                CloseUIView(tempType1, tempAttr1, false, false);
                            }

                            break;
                        }

                        var tempAttr = UIHelper.GetUIBaseDataAttribute(tempType);

                        if (tempAttr.UIViewType == (int)UIViewType.Normal)
                        {
                            PopView();
                            tempUIStack.Push(tempType);
                        }
                        else
                        {
                            CloseUIView(tempType, tempAttr, false, false);
                        }
                    }
                }
                else if (uiViewType == UIViewType.Pop)
                {
                    if (isCloseBack)
                    {
                        //关闭旧的界面A下面的pop界面
                        Stack<Type> tempUIStack = new Stack<Type>();

                        while (true)
                        {
                            var tempType = _uiStack.Peek();

                            if (tempType == type)
                            {
                                PopView();

                                while (true)
                                {
                                    var tempAttr2 = UIHelper.GetUIBaseDataAttribute(tempType);

                                    if (tempAttr2.UIViewType == (int)UIViewType.Normal)
                                    {
                                        break;
                                    }

                                    CloseUIView(tempType, tempAttr2, false, false);
                                }

                                while (tempUIStack.Count > 0)
                                {
                                    PushView(tempUIStack.Pop());
                                }

                                break;
                            }

                            var tempAttr = UIHelper.GetUIBaseDataAttribute(tempType);
                            var uiViewType1 = (UIViewType)tempAttr.UIViewType;

                            if (uiViewType1 == UIViewType.Normal)
                            {
                                break;
                            }

                            if (uiViewType1 == UIViewType.Tips)
                            {
                                CloseUIView(tempType, tempAttr, false, false);
                            }
                            else
                            {
                                PopView();
                                tempUIStack.Push(tempType);
                            }
                        }
                    }
                    else
                    {
                        //关闭旧的界面A和旧的界面A与即将打开的新的界面A之间的pop界面
                        while (true)
                        {
                            var tempType = _uiStack.Peek();

                            if (tempType == type)
                            {
                                PopView();

                                break;
                            }

                            var tempAttr = UIHelper.GetUIBaseDataAttribute(tempType);

                            if (tempAttr.UIViewType == (int)UIViewType.Normal)
                            {
                                break;
                            }

                            CloseUIView(tempType, tempAttr, false, false);
                        }
                    }
                }
                else if (uiViewType == UIViewType.Tips || uiViewType == UIViewType.None)
                {
                    Stack<Type> tempUIStack = new Stack<Type>();

                    while (true)
                    {
                        var tempType = _uiStack.Peek();

                        if (tempType == type)
                        {
                            PopView();

                            while (tempUIStack.Count > 0)
                            {
                                PushView(tempUIStack.Pop());
                            }

                            break;
                        }

                        var tempAttr = UIHelper.GetUIBaseDataAttribute(tempType);

                        if (tempAttr.UIViewType == (int)UIViewType.Tips)
                        {
                            CloseUIView(tempType, tempAttr, false, false);
                        }
                        else
                        {
                            PopView();
                            tempUIStack.Push(tempType);
                        }
                    }
                }

                CloseUIView(type);
            }
            else
            {
                if (uiViewType == UIViewType.Normal)
                {
                    while (_uiStack.Count > 0)
                    {
                        var tempType = _uiStack.Peek();
                        var tempAttr = UIHelper.GetUIBaseDataAttribute(tempType);

                        if (tempAttr.UIViewType == (int)UIViewType.Normal)
                        {
                            UIBaseComponent component = _uiComponentDic[tempType];

                            if (isOpen && component.IsEnable)
                            {
                                component.Disable();
                            }
                            else if (!(isOpen && component.IsEnable))
                            {
                                component.Enable();
                            }

                            break;
                        }

                        CloseUIView(tempType, tempAttr, false, false);
                    }
                }
            }
        }
    }
}