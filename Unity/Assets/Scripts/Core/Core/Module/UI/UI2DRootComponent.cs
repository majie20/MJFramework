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
        private Stack<UIViewLayer>                          _layerStack;
        private Stack<UIViewLayer>                          _tempLayerStack;
        private HashSet<Type>                               _fullScreenViewTypes;

        public void Awake()
        {
            _components = new Dictionary<UIViewLayer, UIManagerComponent>();
            _layerStack = new Stack<UIViewLayer>();
            _tempLayerStack = new Stack<UIViewLayer>();
            _fullScreenViewTypes = new HashSet<Type>();

            Game.Instance.GAddComponent(this);

            foreach (var v in UIValue.LayerNames)
            {
                var component = ObjectHelper.CreateComponent<UIManagerComponent>(ObjectHelper.CreateEntity<Entity>(Entity, Entity.Transform.Find(v.Value).gameObject), false);
                _components.Add(v.Key, component);
            }

            Game.Instance.EventSystem.AddListener<E_CloseUIView>(this, OnCloseUIView);
        }

        public override void Dispose()
        {
            _components = null;
            _layerStack = null;
            _tempLayerStack = null;
            _fullScreenViewTypes = null;

            Game.Instance.GRemoveComponent(this);

            base.Dispose();
        }

        private void OnCloseUIView()
        {
            if (_layerStack.Count == 0)
            {
                return;
            }

            var layer = _layerStack.Peek();
            _components[layer].CloseUpperMostView();
            _layerStack.Pop();

            if (_layerStack.Count == 0)
            {
                return;
            }

            _components[_layerStack.Peek()].SetUIMask(false);
        }

        public async UniTask<UIBaseComponent> OpenUIView(Type type, bool isCloseBack)
        {
            var attr = UIHelper.GetUIBaseDataAttribute(type);
            var layer = (UIViewLayer)attr.UILayer;
            var component = await _components[layer].OpenUIView(type, attr, isCloseBack);

            if (attr.IsOperateMask)
            {
                if (_layerStack.Count > 0)
                {
                    _components[_layerStack.Peek()].SetUIMask(true);
                }

                _layerStack.Push(layer);
            }

            if (attr.IsFullScreen && !this._fullScreenViewTypes.Contains(type))
            {
                this._fullScreenViewTypes.Add(type);
            }

            Game.Instance.EventSystem.Invoke<E_SetMainCameraShow, bool>(this._fullScreenViewTypes.Count <= 0);

            return component;
        }

        public void CloseUIView(Type type, bool isCloseBack)
        {
            var attr = UIHelper.GetUIBaseDataAttribute(type);
            var layer = (UIViewLayer)attr.UILayer;
            _components[layer].CloseUIView(type, attr, isCloseBack);

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

                if (_layerStack.Count == 0)
                {
                    return;
                }

                _components[_layerStack.Peek()].SetUIMask(false);
            }
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
                    var attr = UIHelper.GetUIBaseDataAttribute(types[i]);

                    if ((UIViewLayer)attr.UILayer == layer)
                    {
                        _fullScreenViewTypes.Remove(types[i]);
                    }
                }
            }
        }
    }
}