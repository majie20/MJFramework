using Cysharp.Threading.Tasks;
using ILRuntime.CLR.Method;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Model
{
    public class UIManagerComponent : Component, IAwake
    {
        private static int SORT_ORDER_INIT = -32000;
        private static int SORT_ORDER_SPACING = 2000;

        private Dictionary<Type, UIBaseComponent> uiComponentDic;
        private Dictionary<Type, UniTask> asyncDic;
        private Stack<Type> uiStack;
        private Stack<Type> tempUIStack;

        private int sortOrder;
        private Canvas canvas;

        public void Awake()
        {
            uiComponentDic = new Dictionary<Type, UIBaseComponent>(20);
            asyncDic = new Dictionary<Type, UniTask>(5);
            uiStack = new Stack<Type>(15);
            tempUIStack = new Stack<Type>(15);
            sortOrder = SORT_ORDER_INIT;

            canvas = this.Entity.Transform.GetComponent<Canvas>();
        }

        public override void Dispose()
        {
            asyncDic = null;
            canvas = null;
            uiComponentDic = null;
            uiStack = null;
            tempUIStack = null;
            base.Dispose();
        }

        public async UniTask CloseUpperMostView()
        {
            if (uiStack.Count > 0)
            {
                while (true)
                {
                    var type = uiStack.Peek();
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
            if (uiComponentDic.ContainsKey(type))
            {
                component = uiComponentDic[type];
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
                uiComponentDic.Add(type, component);
            }

            return component;
        }

        private void PushView(Type type)
        {
            if (type == UIValue.MASK_TYPE)
            {
                Game.Instance.EventSystem.Invoke<E_SetUIBlackMaskSortingOrderEvent, int>(sortOrder);
            }
            else
            {
                uiComponentDic[type].SetSortingOrder(sortOrder);
            }
            sortOrder += SORT_ORDER_SPACING;
            uiStack.Push(type);
        }

        private void PopView()
        {
            sortOrder -= SORT_ORDER_SPACING;
            uiStack.Pop();
        }

        public async UniTask<UIBaseComponent> OpenUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack)
        {
            await CloseUIView(type, attr, isCloseBack, true);
            var cts = new CancellationTokenSource();
            var task = UniTask.Create(async () =>
            {
                if (attr.IsOperateMask)
                {
                    Game.Instance.EventSystem.Invoke<E_CloseUIBlackMaskEvent>();
                    SetAndPushUIMask(attr);
                }

                var view = await CreateView(type, attr);
                view.SetParentAndSortingLayer(canvas);
                PushView(type);
                return view;
            }).AttachExternalCancellation(cts.Token);

            if (!asyncDic.ContainsKey(type))
            {
                asyncDic.Add(type, task);
            }
            var newView = await task;
            asyncDic.Remove(type);

            return newView;
        }

        public async UniTask CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack)
        {
            await CloseUIView(type, attr, isCloseBack, false);
        }

        public void InsertUIMask()
        {
            if (uiStack.Count > 0)
            {
                var tempType = uiStack.Peek();
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
                Game.Instance.EventSystem.Invoke<E_SetUIBlackMaskParentEvent, Canvas>(canvas);
                PushView(UIValue.MASK_TYPE);
            }
        }

        private async UniTask CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack, bool isOpen)
        {
            if (asyncDic.TryGetValue(type, out UniTask uniTask))
            {
                var awaitor = uniTask.GetAwaiter();
                await UniTask.WaitWhile(() => awaitor.IsCompleted);
            }
            var uiViewType = (UIViewType)attr.UIViewType;
            if (uiStack.Contains(type))
            {
                if (uiViewType == UIViewType.Normal)
                {
                    tempUIStack.Clear();
                    while (true)
                    {
                        var tempType = uiStack.Peek();
                        if (tempType == type)
                        {
                            PopView();

                            while (tempUIStack.Count > 0)
                            {
                                PushView(tempUIStack.Pop());
                            }

                            var normal = (int)UIViewType.Normal;
                            while (uiStack.Count > 0)
                            {
                                var _tempType = uiStack.Peek();
                                var _tempAttr = UIValue.GetUIBaseDataAttribute(_tempType);
                                if (_tempAttr.UIViewType == normal)
                                {
                                    UIBaseComponent component = uiComponentDic[_tempType];
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
                                await CloseUIView(_tempType, _tempAttr, false, false);
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
                        tempUIStack.Clear();
                        while (true)
                        {
                            var tempType = uiStack.Peek();
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
                            var _uiViewType = (UIViewType)tempAttr.UIViewType;
                            if (_uiViewType == UIViewType.Normal)
                            {
                                break;
                            }

                            if (_uiViewType == UIViewType.Tips)
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
                        //关闭old的界面A与old的界面A和即将打开的界面A之间的pop界面
                        while (true)
                        {
                            var tempType = uiStack.Peek();
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
                    tempUIStack.Clear();
                    while (true)
                    {
                        var tempType = uiStack.Peek();
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

                if (type == UIValue.MASK_TYPE)
                {
                    Game.Instance.EventSystem.Invoke<E_OnCloseUIBlackMaskEvent>();
                }
                else
                {
                    UIBaseComponent component = uiComponentDic[type];
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
            else
            {
                if (uiViewType == UIViewType.Normal)
                {
                    while (uiStack.Count > 0)
                    {
                        var tempType = uiStack.Peek();
                        var tempAttr = UIValue.GetUIBaseDataAttribute(tempType);
                        if (tempAttr.UIViewType == (int)UIViewType.Normal)
                        {
                            UIBaseComponent component = uiComponentDic[tempType];
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