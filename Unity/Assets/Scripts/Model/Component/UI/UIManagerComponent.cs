using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class UIManagerComponent : Component, IAwake
    {
        private static int SORT_ORDER_SPACING = 10000;
        public static string UIROOT_PATH = "Assets/Res/Prefabs/UIRoot";

        private Dictionary<Type, UIBaseComponent> uiComponentDic;
        private Stack<Type> uiStack;
        private Stack<Type> tempUIStack;
        private Camera uiCamera;

        public Camera UICamera
        {
            private set
            {
                uiCamera = value;
            }
            get
            {
                return uiCamera;
            }
        }

        private UIBlackMaskComponent uiBlackMaskComponent;

        public UIBlackMaskComponent UIBlackMaskComponent
        {
            set
            {
                uiBlackMaskComponent = value;
            }
            get
            {
                return uiBlackMaskComponent;
            }
        }

        private int sortOrder;

        public void Awake()
        {
            uiComponentDic = new Dictionary<Type, UIBaseComponent>(20);
            uiStack = new Stack<Type>(15);
            tempUIStack = new Stack<Type>(15);
            sortOrder = 0;

            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            UICamera = rc.Get<GameObject>("UICamera").GetComponent<Camera>();

            UIBlackMaskComponent = ObjectHelper.OpenUIView<UIBlackMaskComponent>();
        }

        public override void Dispose()
        {
            uiComponentDic = null;
            uiStack = null;
            tempUIStack = null;
            UICamera = null;
            Entity = null;
        }

        private UIBaseComponent CreateView(Type type, UIBaseDataAttribute attr)
        {
            if (!uiComponentDic.ContainsKey(type))
            {
                var component = ObjectHelper.CreateComponent(type, ObjectHelper.CreatEntity(this.Entity, attr.PrefabPath, true), true) as UIBaseComponent;
                uiComponentDic.Add(type, component);
            }

            return uiComponentDic[type];
        }

        private void PushView(Type type)
        {
            sortOrder += SORT_ORDER_SPACING;
            uiComponentDic[type].Canvas.sortingOrder = sortOrder;
            uiStack.Push(type);
        }

        private void PopView()
        {
            sortOrder -= SORT_ORDER_SPACING;
            uiStack.Pop();
        }

        public UIBaseComponent OpenUIView(Type type, bool isCloseBack)
        {
            var attr = type.GetCustomAttribute<UIBaseDataAttribute>();
            if (attr != null)
            {
                CloseUIView(type, attr, isCloseBack);
                if (attr.UIViewType != UIViewType.Tips && attr.UIViewType != UIViewType.None)
                {
                    var tempType = typeof(UIBlackMaskComponent);
                    CloseUIView(tempType, isCloseBack);
                    UIBlackMaskComponent.SetMaskMode(attr.UIMaskMode);
                    PushView(tempType);
                }

                var newView = CreateView(type, attr);
                PushView(type);
                return newView;
            }

            return default;
        }

        public void CloseUIView(Type type, bool isCloseBack)
        {
            CloseUIView(type, type.GetCustomAttribute<UIBaseDataAttribute>(), isCloseBack);
        }

        public void CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack)
        {
            if (attr != null && uiStack.Contains(type))
            {
                if (attr.UIViewType == UIViewType.Normal)
                {
                    while (true)
                    {
                        var tempType = uiStack.Peek();
                        if (tempType == type)
                        {
                            PopView();
                            break;
                        }
                        var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                        CloseUIView(tempType, tempAttr, false);
                    }
                }
                else if (attr.UIViewType == UIViewType.Pop)
                {
                    if (isCloseBack)
                    {
                        tempUIStack.Clear();
                        while (true)
                        {
                            var tempType = uiStack.Peek();
                            if (tempType == type)
                            {
                                PopView();
                                while (true)
                                {
                                    var tempAttr2 = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                                    if (tempAttr2.UIViewType == UIViewType.Normal)
                                    {
                                        break;
                                    }

                                    CloseUIView(tempType, tempAttr2, false);
                                }

                                while (tempUIStack.Count > 0)
                                {
                                    PushView(tempUIStack.Pop());
                                }
                                break;
                            }
                            var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                            if (tempAttr.UIViewType == UIViewType.Normal)
                            {
                                break;
                            }
                            if (tempAttr.UIViewType == UIViewType.Tips)
                            {
                                CloseUIView(tempType, tempAttr, false);
                            }
                            else
                            {
                                tempUIStack.Push(tempType);
                            }
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            var tempType = uiStack.Peek();
                            if (tempType == type)
                            {
                                PopView();
                                break;
                            }
                            var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                            if (tempAttr.UIViewType == UIViewType.Normal)
                            {
                                break;
                            }
                            CloseUIView(tempType, tempAttr, false);
                        }
                    }
                }
                else if (attr.UIViewType == UIViewType.Tips || attr.UIViewType == UIViewType.None)
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
                        var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                        if (tempAttr.UIViewType == UIViewType.Tips)
                        {
                            CloseUIView(tempType, tempAttr, false);
                        }
                        else
                        {
                            tempUIStack.Push(tempType);
                        }
                    }
                }
                uiComponentDic[type].Close();
            }
        }
    }
}