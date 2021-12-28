using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class UIManagerComponent : Component, IAwake
    {
        private static int SORT_ORDER_INIT = 1000;
        private static int SORT_ORDER_SPACING = 1000;
        public static string GAME_OBJECT_NAME = "2DRoot";

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
        private Type maskType;

        public void Awake()
        {
            uiComponentDic = new Dictionary<Type, UIBaseComponent>(20);
            uiStack = new Stack<Type>(15);
            tempUIStack = new Stack<Type>(15);
            sortOrder = SORT_ORDER_INIT;

            UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            var canvas = this.Entity.Transform.GetComponent<Canvas>();
            canvas.worldCamera = UICamera;

            maskType = typeof(UIBlackMaskComponent);

            Game.Instance.EventSystem.AddListener<CloseUIViewEvent>(OnCloseUIViewEvent, this);

            UIBlackMaskComponent = ObjectHelper.OpenUIView<UIBlackMaskComponent>();
        }

        public override void Dispose()
        {
            Game.Instance.EventSystem.RemoveListener<CloseUIViewEvent>(this);
            uiComponentDic = null;
            uiStack = null;
            tempUIStack = null;
            UICamera = null;
            Entity = null;
        }

        private void OnCloseUIViewEvent()
        {
            if (uiStack.Count > 0)
            {
                CloseUIView(uiStack.Peek(), false);
            }
        }

        private UIBaseComponent CreateView(Type type, UIBaseDataAttribute attr)
        {
            if (!uiComponentDic.ContainsKey(type))
            {
                var component = ObjectHelper.CreateComponent(type, ObjectHelper.CreatEntity(this.Entity, null, attr.PrefabPath, true), true) as UIBaseComponent;
                RectTransform rect = component.Entity.Transform.GetComponent<RectTransform>();
                rect.localPosition = Vector3.zero;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                uiComponentDic.Add(type, component);
            }

            return uiComponentDic[type];
        }

        private void PushView(Type type)
        {
            uiComponentDic[type].Canvas.sortingOrder = sortOrder;
            sortOrder += SORT_ORDER_SPACING;
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
                CloseUIView(type, attr, isCloseBack, true);
                SetUIMask(attr);
                var newView = CreateView(type, attr);
                PushView(type);
                return newView;
            }

            return null;
        }

        public void CloseUIView(Type type, bool isCloseBack)
        {
            CloseUIView(type, type.GetCustomAttribute<UIBaseDataAttribute>(), isCloseBack, false);
        }

        private void SortUIView()
        {
            CloseUIView(maskType, false);
            if (uiStack.Count > 0)
            {
                var tempType = uiStack.Peek();
                PopView();
                SetUIMask(tempType.GetCustomAttribute<UIBaseDataAttribute>());
                PushView(tempType);
            }
        }

        private void SetUIMask(UIBaseDataAttribute attr)
        {
            if (attr.UIViewType != UIViewType.Tips && attr.UIViewType != UIViewType.None)
            {
                CloseUIView(maskType, false);
                UIBlackMaskComponent.SetMaskMode(attr.UIMaskMode);
                PushView(maskType);
            }
        }

        private void CloseUIView(Type type, UIBaseDataAttribute attr, bool isCloseBack, bool isOpen)
        {
            if (attr != null)
            {
                if (uiStack.Contains(type))
                {
                    if (attr.UIViewType == UIViewType.Normal)
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

                                SortUIView();
                                if (uiStack.Count > 0)
                                {
                                    UIBaseComponent component = uiComponentDic[uiStack.Peek()];
                                    if (!(isOpen && component.Canvas.enabled))
                                    {
                                        component.Enable();
                                    }
                                    else if (isOpen && component.IsEnable)
                                    {
                                        component.Disable();
                                    }
                                }
                                break;
                            }

                            var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                            if (tempAttr.UIViewType == UIViewType.Normal)
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

                                        CloseUIView(tempType, tempAttr2, false, false);
                                    }

                                    while (tempUIStack.Count > 0)
                                    {
                                        PushView(tempUIStack.Pop());
                                    }

                                    SortUIView();
                                    break;
                                }

                                var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                                if (tempAttr.UIViewType == UIViewType.Normal)
                                {
                                    SortUIView();
                                    break;
                                }

                                if (tempAttr.UIViewType == UIViewType.Tips)
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
                            while (true)
                            {
                                var tempType = uiStack.Peek();
                                if (tempType == type)
                                {
                                    PopView();
                                    SortUIView();
                                    break;
                                }

                                var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                                if (tempAttr.UIViewType == UIViewType.Normal)
                                {
                                    SortUIView();
                                    break;
                                }

                                CloseUIView(tempType, tempAttr, false, false);
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
                                CloseUIView(tempType, tempAttr, false, false);
                            }
                            else
                            {
                                PopView();
                                tempUIStack.Push(tempType);
                            }
                        }
                    }

                    uiComponentDic[type].Close();
                }
                else
                {
                    if (attr.UIViewType == UIViewType.Normal)
                    {
                        while (uiStack.Count > 0)
                        {
                            var tempType = uiStack.Peek();
                            var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                            if (tempAttr.UIViewType == UIViewType.Normal)
                            {
                                UIBaseComponent component = uiComponentDic[tempType];
                                if (isOpen && component.IsEnable)
                                {
                                    component.Disable();
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
}