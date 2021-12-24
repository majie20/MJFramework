using System;
using System.Collections.Generic;
using System.Reflection;

namespace Model
{
    [LifeCycle]
    public class UIManagerComponent : Component, IAwake
    {
        private Dictionary<Type, UIBaseComponent> uiComponentDic;

        private Stack<Type> uiStack;

        public void Awake()
        {
            uiComponentDic = new Dictionary<Type, UIBaseComponent>(10);
            uiStack = new Stack<Type>(20);
        }

        public UIBaseComponent OpenUIView(Type type)
        {
            var attr = type.GetCustomAttribute<UIBaseDataAttribute>();
            if (attr != null)
            {
                CloseUIView(type, attr);
                if (attr.Type == UIViewType.Normal)
                {
                    while (true)
                    {
                        var tempAttr = uiStack.Peek().GetCustomAttribute<UIBaseDataAttribute>();
                        if (tempAttr.Type != UIViewType.Normal)
                        {
                        }
                        else
                        {
                            break;
                        }

                        if (uiStack.Count == 0)
                        {
                            break;
                        }
                    }

                    if (uiStack.Contains(type))
                    {

                    }

                    var component = CreateView(type, attr);
                    uiStack.Push(type);
                }
                else if (attr.Type == UIViewType.Pop)
                {

                }
                else if (attr.Type == UIViewType.Tips)
                {

                }
            }



            return default;
        }

        public void CloseUIView(Type type, UIBaseDataAttribute attr)
        {
            if (attr != null && uiStack.Contains(type))
            {
                Stack<Type> tempStack = new Stack<Type>();

                if (attr.Type == UIViewType.Normal)
                {
                    while (true)
                    {
                        var tempType = uiStack.Peek();
                        if (tempType == type)
                        {
                            uiStack.Pop();
                            break;
                        }

                        var tempAttr = tempType.GetCustomAttribute<UIBaseDataAttribute>();
                        CloseUIView(tempType, tempAttr);
                    }
                }
                else if (attr.Type == UIViewType.Pop)
                {
                }
                else if (attr.Type == UIViewType.Tips)
                {

                }
            }

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
    }
}