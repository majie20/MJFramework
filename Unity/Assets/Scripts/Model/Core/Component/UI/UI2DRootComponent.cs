using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class UI2DRootComponent : Component, IAwake
    {
        public static string GAME_OBJECT_NAME = "2DRoot";
        private Dictionary<UIViewLayer, UIManagerComponent> components;

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

        private Stack<UIViewLayer> layerStack;

        public void Awake()
        {
            components = new Dictionary<UIViewLayer, UIManagerComponent>();
            layerStack = new Stack<UIViewLayer>();

            Game.Instance.AddComponent(this);

            CreateUIBlackMask();

            foreach (var v in UIValue.LayerNames)
            {
                var component = ObjectHelper.CreateComponent<UIManagerComponent>(ObjectHelper.CreateEntity(Entity, Entity.Transform.Find(v.Value).gameObject), false);
                components.Add(v.Key, component);
            }

            Game.Instance.EventSystem.AddListenerAsync<E_CloseUIViewEvent>(this, OnCloseUIViewEvent);
            Game.Instance.EventSystem.AddListener<E_SetUIBlackMaskParentEvent, Canvas>(this, OnSetUIBlackMaskParent);
            Game.Instance.EventSystem.AddListener<E_SetUIBlackMaskSortingOrderEvent, int>(this, OnSetUIBlackMaskSortingOrder);
            Game.Instance.EventSystem.AddListener<E_CloseUIBlackMaskEvent>(this, OnCloseUIBlackMask);
        }

        public override void Dispose()
        {
            this.UIBlackMaskComponent.Dispose();
            this.UIBlackMaskComponent = null;

            components = null;

            Game.Instance.RemoveComponent(this);

            base.Dispose();
        }

        private async UniTask OnCloseUIViewEvent()
        {
            if (layerStack.Count == 0)
            {
                return;
            }
            var layer = layerStack.Peek();
            await components[layer].CloseUpperMostView();
            await components[layer].CloseUIView(UIValue.MASK_TYPE, UIValue.MASK_TYPE_ATTR, false);
            layerStack.Pop();
            if (layerStack.Count == 0)
            {
                return;
            }
            layer = layerStack.Peek();
            components[layer].InsertUIMask();
        }

        private void OnSetUIBlackMaskParent(Canvas canvas)
        {
            this.UIBlackMaskComponent.SetParentAndSortingLayer(canvas);
        }

        private void OnSetUIBlackMaskSortingOrder(int order)
        {
            UIBlackMaskComponent.SetSortingOrder(order);
        }

        private void OnCloseUIBlackMask()
        {
            if (layerStack.Count == 0)
            {
                return;
            }
            var layer = layerStack.Peek();
            if (layer != UIViewLayer.None)
            {
                components[layer].CloseUIView(UIValue.MASK_TYPE, UIValue.MASK_TYPE_ATTR, false).GetAwaiter().GetResult();
            }
        }

        public async UniTask<UIBaseComponent> OpenUIView(Type type, bool isCloseBack)
        {
            var attr = UIValue.GetUIBaseDataAttribute(type);
            var layer = (UIViewLayer)attr.UILayer;
            var component = await components[layer].OpenUIView(type, attr, isCloseBack);
            if (attr.IsOperateMask)
            {
                layerStack.Push(layer);
            }

            return component;
        }

        public async UniTask CloseUIView(Type type, bool isCloseBack)
        {
            var attr = UIValue.GetUIBaseDataAttribute(type);
            var layer = (UIViewLayer)attr.UILayer;
            await components[layer].CloseUIView(type, attr, isCloseBack);
            if (layerStack.Count == 0)
            {
                return;
            }
            if (attr.IsOperateMask)
            {
                await components[layer].CloseUIView(UIValue.MASK_TYPE, UIValue.MASK_TYPE_ATTR, false);
                layerStack.Pop();
            }
            if (layerStack.Count == 0)
            {
                return;
            }
            layer = layerStack.Peek();
            components[layer].InsertUIMask();
        }

        public void CreateUIBlackMask()
        {
            var component = ObjectHelper.CreateComponent(UIValue.MASK_TYPE, ObjectHelper.CreateEntity(this.Entity, null, UIValue.MASK_TYPE_ATTR.PrefabPath, true).GetAwaiter().GetResult()) as UIBlackMaskComponent;
            RectTransform rect = component.Entity.Transform.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            UIBlackMaskComponent = component;
        }
    }
}