using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public class UI2DRootComponent : Component, IAwake
    {
        public static string GAME_OBJECT_NAME = "2DRoot";
        private Dictionary<UIViewLayer, UIManagerComponent> _components;

        public UIBlackMaskComponent UIBlackMaskComponent { private set; get; }

        private Stack<UIViewLayer> _layerStack;
        private Stack<UIViewLayer> _tempLayerStack;

        private HashSet<Type> _fullScreenViewTypes;

        public void Awake()
        {
            _components = new Dictionary<UIViewLayer, UIManagerComponent>();
            _layerStack = new Stack<UIViewLayer>();
            _tempLayerStack = new Stack<UIViewLayer>();
            _fullScreenViewTypes = new HashSet<Type>();

            Game.Instance.GAddComponent(this);

            CreateUIBlackMask();

            foreach (var v in UIValue.LayerNames)
            {
                var component = ObjectHelper.CreateComponent<UIManagerComponent, UIBlackMaskComponent>(ObjectHelper.CreateEntity(Entity, Entity.Transform.Find(v.Value).gameObject), this.UIBlackMaskComponent, false);
                _components.Add(v.Key, component);
            }

            Game.Instance.EventSystem.AddListenerAsync<E_CloseUIViewEvent>(this, OnCloseUIViewEvent);
        }

        public override void Dispose()
        {
            this.UIBlackMaskComponent.Dispose();
            this.UIBlackMaskComponent = null;

            _components = null;
            _layerStack = null;
            _tempLayerStack = null;
            _fullScreenViewTypes = null;

            Game.Instance.GRemoveComponent(this);

            base.Dispose();
        }

        private async UniTask OnCloseUIViewEvent()
        {
            if (_layerStack.Count == 0)
            {
                return;
            }
            var layer = _layerStack.Peek();
            await _components[layer].CloseUpperMostView();
            await _components[layer].CloseUIView(UIValue.MASK_TYPE, UIValue.MASK_TYPE_ATTR, false);
            _layerStack.Pop();
            if (_layerStack.Count == 0)
            {
                return;
            }
            layer = _layerStack.Peek();
            _components[layer].InsertUIMask();
        }

        public async UniTask<UIBaseComponent> OpenUIView(Type type, bool isCloseBack)
        {
            var attr = UIValue.GetUIBaseDataAttribute(type);
            var layer = (UIViewLayer)attr.UILayer;
            var component = await _components[layer].OpenUIView(type, attr, isCloseBack);
            if (attr.IsOperateMask)
            {
                _layerStack.Push(layer);
            }

            if (attr.IsFullScreen && !this._fullScreenViewTypes.Contains(type))
            {
                this._fullScreenViewTypes.Add(type);
            }

            Game.Instance.EventSystem.Invoke<E_SetMainCameraShow, bool>(this._fullScreenViewTypes.Count <= 0);

            return component;
        }

        public async UniTask CloseUIView(Type type, bool isCloseBack)
        {
            var attr = UIValue.GetUIBaseDataAttribute(type);
            var layer = (UIViewLayer)attr.UILayer;
            await _components[layer].CloseUIView(type, attr, isCloseBack);

            if (this._fullScreenViewTypes.Contains(type))
            {
                this._fullScreenViewTypes.Remove(type);
            }

            Game.Instance.EventSystem.Invoke<E_SetMainCameraShow, bool>(this._fullScreenViewTypes.Count <= 0);

            if (_layerStack.Count == 0)
            {
                return;
            }
            if (attr.IsOperateMask)
            {
                await _components[layer].CloseUIView(UIValue.MASK_TYPE, UIValue.MASK_TYPE_ATTR, false);

                _tempLayerStack.Clear();
                while (true)
                {
                    var tempLayer = _layerStack.Pop();
                    if (tempLayer == layer)
                    {
                        while (_tempLayerStack.Count > 0)
                        {
                            _layerStack.Push(_tempLayerStack.Pop());
                        }
                        break;
                    }

                    _tempLayerStack.Push(tempLayer);
                }
            }
            if (_layerStack.Count == 0 || this.UIBlackMaskComponent.Canvas.sortingLayerName != UIValue.LayerNames[layer])
            {
                return;
            }
            layer = _layerStack.Peek();
            _components[layer].InsertUIMask();
        }

        public void CreateUIBlackMask()
        {
            var component = ObjectHelper.CreateComponent(UIValue.MASK_TYPE, ObjectHelper.CreateEntity(this.Entity, null, UIValue.MASK_TYPE_ATTR.PrefabPath, true, false).GetAwaiter().GetResult()) as UIBlackMaskComponent;
            RectTransform rect = component.Entity.Transform.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            UIBlackMaskComponent = component;
        }

        public void ClearUIViewByLayer(UIViewLayer layer)
        {
            _components[layer].Clear();

            _tempLayerStack.Clear();
            while (_layerStack.Count > 0)
            {
                var tempLayer = _layerStack.Pop();
                if (tempLayer != layer)
                {
                    _tempLayerStack.Push(tempLayer);
                }
            }

            while (_tempLayerStack.Count > 0)
            {
                _layerStack.Push(_tempLayerStack.Pop());
            }

            if (_fullScreenViewTypes.Count > 0)
            {
                var types = _fullScreenViewTypes.ToArray();
                for (int i = types.Length - 1; i >= 0; i--)
                {
                    var attr = UIValue.GetUIBaseDataAttribute(types[i]);
                    if ((UIViewLayer)attr.UILayer == layer)
                    {
                        _fullScreenViewTypes.Remove(types[i]);
                    }
                }
            }
        }
    }
}