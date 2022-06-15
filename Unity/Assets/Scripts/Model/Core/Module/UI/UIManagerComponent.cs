using Cysharp.Threading.Tasks;
using ILRuntime.CLR.Method;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Model
{
    public class UIManagerComponent : Component, IAwake<UIBlackMaskComponent>
    {
        private static int SORT_ORDER_INIT = -32000;
        private static int SORT_ORDER_SPACING = 2000;

        private Dictionary<Type, UIBaseComponent> _uiComponentDic;
        private Dictionary<Type, UniTask> _asyncDic;
        private Stack<Type> _uiStack;

        private int _sortOrder;
        private Canvas _canvas;

        public void Awake(UIBlackMaskComponent component)
        {
            _uiComponentDic = new Dictionary<Type, UIBaseComponent>(20);
            _asyncDic = new Dictionary<Type, UniTask>(5);
            _uiStack = new Stack<Type>(15);
            _sortOrder = SORT_ORDER_INIT;

            _canvas = this.Entity.Transform.GetComponent<Canvas>();

            _uiComponentDic.Add(component.GetType(), component);
        }

        public override void Dispose()
        {
            _asyncDic = null;
            _canvas = null;
            _uiComponentDic = null;
            _uiStack = null;
            base.Dispose();
        }

        public async UniTask CloseUpperMostView()
        {
            if (_uiStack.Count > 0)
            {
                while (true)
                {
                    var type = _uiStack.Peek();
                    var attr = UIValue.GetUIBaseDataAttribute(type);
                    var uiViewType = (UIViewType)attr.UIViewType;
                    await CloseUIView(type, attr, false);
                    if (uiViewType != UIViewType.Tips && uiViewType != UIViewType.None && attr.IsOperateMask)
                    {
                        break;
                    }
                }
            }
        }

        private async UniTask<UIBaseComponent> CreateView(Type type, UIBaseDataAttribute attr)
        {
            UIBaseComponent component;
            if (_uiComponentDic.ContainsKey(type))
            {
                component = _uiComponentDic[type];
                component.IsRuning = true;
                component.awakeCalled = true;
                component.called = false;
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.AddLifecycle"];

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
            }
            else
            {
                await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadUIAtlas(attr.PrefabPath);
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.CreateComponent3"];

                    using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                    {
                        ctx.PushObject(type);
                        ctx.PushObject(await ObjectHelper.CreateEntity(this.Entity, null, attr.PrefabPath, true));
                        ctx.PushBool(true);
                        ctx.Invoke();
                        component = ctx.ReadObject<UIBaseComponent>(0);
                    }
                }
                else
                {
                    component = ObjectHelper.CreateComponent(type, await ObjectHelper.CreateEntity(this.Entity, null, attr.PrefabPath, true)) as UIBaseComponent;
                }
                RectTransform rect = component.Entity.Transform.GetComponent<RectTransform>();
                rect.localPosition = Vector3.zero;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                _uiComponentDic.Add(type, component);
            }

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
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }

            while (_uiStack.Count > 0)
            {
                var type = _uiStack.Peek();
                PopView();
                CloseUIView(type);
            }

            _sortOrder = SORT_ORDER_INIT;
        }

        public async UniTask<UIBaseComponent> OpenUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack)
        {
            await CloseUIView(type, attr, isCloseBack, true);
            var task = UniTask.Create(async () =>
            {
                if (attr.IsOperateMask)
                {
                    SetAndPushUIMask(attr);
                }

                var view = await CreateView(type, attr);
                view.SetParentAndSortingLayer(_canvas);
                PushView(type);
                return view;
            }).AttachExternalCancellation(CancellationToken);

            if (!_asyncDic.ContainsKey(type))
            {
                _asyncDic.Add(type, task);
            }
            var newView = await task;
            _asyncDic.Remove(type);

            return newView;
        }

        public async UniTask CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack)
        {
            await CloseUIView(type, attr, isCloseBack, false);
        }

        public void InsertUIMask()
        {
            if (_uiStack.Count > 0)
            {
                var tempType = _uiStack.Peek();
                var _attr = UIValue.GetUIBaseDataAttribute(tempType);
                if (_attr.IsOperateMask)
                {
                    PopView();
                    SetAndPushUIMask(_attr);
                    PushView(tempType);
                }
            }
        }

        private void SetAndPushUIMask(UIBaseDataAttribute attr)
        {
            var type = (UIViewType)attr.UIViewType;
            if (type != UIViewType.Tips && type != UIViewType.None)
            {
                Game.Instance.EventSystem.Invoke<E_SetMaskModeEvent, int>(attr.UIMaskMode);
                var component = OpenUIView(UIValue.MASK_TYPE, UIValue.GetUIBaseDataAttribute(UIValue.MASK_TYPE), false).GetAwaiter().GetResult();
                component.Enable();
            }
        }

        public void CloseUIView(Type type)
        {
            UIBaseComponent component = _uiComponentDic[type];
            if (type == UIValue.MASK_TYPE)
            {
                component.Disable();
            }
            else
            {
                component.Close();
                component.IsRuning = false;
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.RemoveLifecycle"];

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
            }
        }

        private async UniTask CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack, bool isOpen)
        {
            if (_asyncDic.TryGetValue(type, out UniTask uniTask))
            {
                var awaitor = uniTask.GetAwaiter();
                await UniTask.WaitWhile(() => awaitor.IsCompleted);
            }
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
                                var tempAttr1 = UIValue.GetUIBaseDataAttribute(tempType1);
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
                                await CloseUIView(tempType1, tempAttr1, false, false);
                            }

                            break;
                        }

                        var tempAttr = UIValue.GetUIBaseDataAttribute(tempType);
                        if (tempAttr.UIViewType == (int)UIViewType.Normal)
                        {
                            PopView();
                            tempUIStack.Push(tempType);
                        }
                        else
                        {
                            await CloseUIView(tempType, tempAttr, false, false);
                        }
                    }
                }
                else if (uiViewType == UIViewType.Pop)
                {
                    if (isCloseBack)
                    {
                        //关闭old的界面A前面的pop界面
                        Stack<Type> tempUIStack = new Stack<Type>();
                        while (true)
                        {
                            var tempType = _uiStack.Peek();
                            if (tempType == type)
                            {
                                PopView();
                                while (true)
                                {
                                    var tempAttr2 = UIValue.GetUIBaseDataAttribute(tempType);
                                    if (tempAttr2.UIViewType == (int)UIViewType.Normal)
                                    {
                                        break;
                                    }

                                    await CloseUIView(tempType, tempAttr2, false, false);
                                }

                                while (tempUIStack.Count > 0)
                                {
                                    PushView(tempUIStack.Pop());
                                }

                                break;
                            }

                            var tempAttr = UIValue.GetUIBaseDataAttribute(tempType);
                            var uiViewType1 = (UIViewType)tempAttr.UIViewType;
                            if (uiViewType1 == UIViewType.Normal)
                            {
                                break;
                            }

                            if (uiViewType1 == UIViewType.Tips)
                            {
                                await CloseUIView(tempType, tempAttr, false, false);
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
                        //关闭old的界面A和old的界面A与即将打开的界面A之间的pop界面
                        while (true)
                        {
                            var tempType = _uiStack.Peek();
                            if (tempType == type)
                            {
                                PopView();
                                break;
                            }

                            var tempAttr = UIValue.GetUIBaseDataAttribute(tempType);
                            if (tempAttr.UIViewType == (int)UIViewType.Normal)
                            {
                                break;
                            }

                            await CloseUIView(tempType, tempAttr, false, false);
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

                        var tempAttr = UIValue.GetUIBaseDataAttribute(tempType);
                        if (tempAttr.UIViewType == (int)UIViewType.Tips)
                        {
                            await CloseUIView(tempType, tempAttr, false, false);
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
                        var tempAttr = UIValue.GetUIBaseDataAttribute(tempType);
                        if (tempAttr.UIViewType == (int)UIViewType.Normal)
                        {
                            UIBaseComponent component = _uiComponentDic[tempType];
                            if (isOpen && component.IsEnable)
                            {
                                component.Disable();
                            }
                            break;
                        }
                        await CloseUIView(tempType, tempAttr, false, false);
                    }
                }
            }
        }
    }
}