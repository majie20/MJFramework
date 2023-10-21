using Cysharp.Threading.Tasks;
using M.Algorithm;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    public class UIBaseComponent : Component
    {
        private static Color TRANSPARENT       = new(1, 1, 1, 0);
        private static Color BLACK_TRANSPARENT = new(30 / 255f, 30 / 255f, 30 / 255f, 155 / 255f);

        public  Canvas           Canvas;
        public  CanvasGroup      CanvasGroup;
        public  GraphicRaycaster GraphicRaycaster;
        private Button           _btnSelf;
        private Image            _imgSelf;

        protected StaticLinkedListDictionary<GameObject, Canvas>                    _childCanvasDic;
        protected StaticLinkedListDictionary<Canvas, TwoStaticLinkedList<Renderer>> _childRendererDic;

        private int        _sortOrder;
        private UIMaskMode _maskMode;
        public  bool       IsEnable;
        public  bool       IsLoadComplete;
        public  bool       IsOpen;

        public virtual void Awake()
        {
        }

        protected void _Awake()
        {
            AddComponent();

            var canvasArray = this.Entity.Transform.GetComponentsInChildren<Canvas>(true);
            var len = canvasArray.Length;
            _childCanvasDic = new StaticLinkedListDictionary<GameObject, Canvas>(0, len);
            _childRendererDic = new StaticLinkedListDictionary<Canvas, TwoStaticLinkedList<Renderer>>(0, len);

            for (int i = 1; i < len; i++)
            {
                _childCanvasDic.Add(canvasArray[i].gameObject, canvasArray[i]);
                _childRendererDic.Add(canvasArray[i], new TwoStaticLinkedList<Renderer>(0, 8));
            }

            if (_maskMode != UIMaskMode.Null)
            {
                SetMaskMode(_maskMode);
            }

            var component = Entity.Parent.GetComponent<UIManagerComponent>();
            SetParentAndSortingLayer(component.Canvas);

            Awake();

            if (IsOpen)
            {
                RefreshEnable();
                OnLoadComplete().Forget();
            }
        }

        public override void Dispose()
        {
            _Dispose();
        }

        protected void _Dispose()
        {
            Canvas = null;
            GraphicRaycaster = null;
            CanvasGroup = null;
            _childCanvasDic = null;
            _childRendererDic = null;
            _btnSelf = null;
            _imgSelf = null;
            base.Dispose();
        }

        public async UniTaskVoid CreateView(UIBaseDataAttribute attr)
        {
            if (IsLoadComplete)
            {
                SetSortingOrder(_sortOrder);

                return;
            }

            _maskMode = UIMaskMode.Null;
            var handle = Game.Instance.Scene.GetComponent<AssetsComponent>().CreateLoadHandle(typeof(GameObject), attr.PrefabPath);

            if (!handle.IsDone)
            {
                await handle.ToUniTask().AttachExternalCancellation(Entity.CancellationToken);
            }

            IsLoadComplete = true;

            Entity.GameObject = Game.Instance.Scene.GetComponent<GameObjPoolComponent>().HatchGameObjBySign(attr.PrefabPath, Entity.Parent.Transform, true);
            Entity.Transform = Entity.GameObject.transform;
            Entity.AddComponentView();

            if (Entity.Transform.TryGetComponent<EntityIdHandle>(out var idHandle))
            {
                idHandle.Guid = Entity.Guid;
            }

            RectTransform rect = Entity.Transform.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;

            _Awake();
        }

        protected void AddComponent()
        {
            Canvas = this.Entity.Transform.GetComponent<Canvas>();

            if (Canvas == null)
            {
                Canvas = this.Entity.GameObject.AddComponent<Canvas>();
            }

            Canvas.overrideSorting = true;
            Canvas.sortingOrder = -1;
            Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;

            GraphicRaycaster = this.Entity.Transform.GetComponent<GraphicRaycaster>();

            if (GraphicRaycaster == null)
            {
                GraphicRaycaster = this.Entity.GameObject.AddComponent<GraphicRaycaster>();
            }

            CanvasGroup = this.Entity.Transform.GetComponent<CanvasGroup>();

            if (CanvasGroup == null)
            {
                CanvasGroup = this.Entity.GameObject.AddComponent<CanvasGroup>();
            }

            _imgSelf = this.Entity.Transform.GetComponent<Image>();

            if (_imgSelf == null)
            {
                _imgSelf = this.Entity.GameObject.AddComponent<Image>();
            }

            _btnSelf = this.Entity.Transform.GetComponent<Button>();

            if (_btnSelf == null)
            {
                _btnSelf = this.Entity.GameObject.AddComponent<Button>();
            }

            _btnSelf.transition = Selectable.Transition.None;
            _btnSelf.onClick.AddListener(OnBtnSelfClick);
        }

        protected void SetParentAndSortingLayer(Canvas canvas)
        {
            var sortingLayerID = canvas.sortingLayerID;

            if (this.Canvas.sortingLayerID != sortingLayerID)
            {
                this.Entity.Transform.SetParent(canvas.transform);
                this.Canvas.sortingLayerID = sortingLayerID;

                var data = _childCanvasDic.GetElement(1);

                while (data.right != 0)
                {
                    data = _childCanvasDic.GetElement(data.right);
                    data.element.sortingLayerID = sortingLayerID;
                }
            }
        }

        private void OnBtnSelfClick()
        {
            Game.Instance.EventSystem.Invoke<E_CloseUIView>();
        }

        public void SetMaskMode(UIMaskMode mode)
        {
            _maskMode = mode;

            if (!IsLoadComplete)
            {
                return;
            }

            if (_maskMode == UIMaskMode.Transparent)
            {
                SetMaskMode(0, false, true);
            }
            else if (_maskMode == UIMaskMode.TransparentClick)
            {
                SetMaskMode(0, true, true);
            }
            //else if (v == UIMaskMode.TransparentPenetrate)
            //{
            //    SetMaskMode(0, false, false);
            //}
            else if (_maskMode == UIMaskMode.BlackTransparent)
            {
                SetMaskMode(1, false, true);
            }
            else if (_maskMode == UIMaskMode.BlackTransparentClick)
            {
                SetMaskMode(1, true, true);
            }
            else if (_maskMode == UIMaskMode.None)
            {
                SetMaskMode(0, false, false);
            }
            //else if (v == UIMaskMode.BlackTransparentPenetrate)
            //{
            //    SetMaskMode(1, false, false);
            //}
        }

        private void SetMaskMode(int alpha, bool interactable, bool blocksRaycasts)
        {
            _imgSelf.color = alpha == 1 ? BLACK_TRANSPARENT : TRANSPARENT;
            _imgSelf.raycastTarget = blocksRaycasts;
            _btnSelf.interactable = interactable;
        }

        public void SetSortingOrder(int sortOrder)
        {
            this._sortOrder = sortOrder;

            if (!IsLoadComplete)
            {
                return;
            }

            if (this.Canvas.sortingOrder != sortOrder)
            {
                this.Canvas.sortingOrder = sortOrder;

                var sortingLayerID = this.Canvas.sortingLayerID;
                var data = _childCanvasDic.GetElement(1);

                while (data.right != 0)
                {
                    data = _childCanvasDic.GetElement(data.right);
                    data.element.sortingOrder = ++this._sortOrder;

                    var sortingOrder = data.element.sortingOrder;
                    var list = this._childRendererDic[data.element];
                    var len = list.Length;

                    for (int i = 0; i < len; i++)
                    {
                        var renderer = list.GetValue(i);
                        renderer.sortingLayerID = sortingLayerID;
                        renderer.sortingOrder = sortingOrder;
                    }
                }
            }
        }

        public void RefreshRenderers(Canvas canvas)
        {
            Renderer[] renderers = canvas.GetComponentsInChildren<Renderer>(true);
            var len = renderers.Length;
            var list = new TwoStaticLinkedList<Renderer>(0, len);
            var sortingLayerID = this.Canvas.sortingLayerID;
            var sortingOrder = canvas.sortingOrder;
            this._childRendererDic.Add(canvas, list);

            for (int i = 0; i < len; i++)
            {
                var renderer = renderers[i];
                renderer.sortingLayerID = sortingLayerID;
                renderer.sortingOrder = sortingOrder;
                list.Add(renderer);
            }
        }

        public void RefreshCanvas(Canvas canvas)
        {
            if (_childRendererDic.ContainsKey(canvas))
            {
                this._childRendererDic.Remove(canvas);
                RefreshRenderers(canvas);
            }
        }

        public void AddCanvas(Canvas canvas)
        {
            if (!_childCanvasDic.ContainsKey(canvas.gameObject))
            {
                canvas.sortingLayerID = this.Canvas.sortingLayerID;
                canvas.sortingOrder = ++this._sortOrder;
                _childCanvasDic.Add(canvas.gameObject, canvas);

                RefreshRenderers(canvas);
            }
        }

        public void AddCanvasByArray(Canvas[] canvasArray)
        {
            var len = canvasArray.Length;

            for (int i = 0; i < len; i++)
            {
                AddCanvas(canvasArray[i]);
            }
        }

        public void RemoveCanvas(Canvas canvas)
        {
            if (_childCanvasDic.ContainsKey(canvas.gameObject))
            {
                _childCanvasDic.Remove(canvas.gameObject);
                _childRendererDic.Remove(canvas);
            }
        }

        public void RemoveCanvasByArray(Canvas[] canvasArray)
        {
            var len = canvasArray.Length;

            for (int i = 0; i < len; i++)
            {
                RemoveCanvas(canvasArray[i]);
            }
        }

        public void AddRenderers(Canvas canvas, Renderer[] renderers)
        {
            if (_childRendererDic.ContainsKey(canvas))
            {
                var list = _childRendererDic[canvas];
                var len = renderers.Length;
                var sortingLayerID = this.Canvas.sortingLayerID;
                var sortingOrder = canvas.sortingOrder;

                for (int i = 0; i < len; i++)
                {
                    var renderer = renderers[i];

                    if (!list.Contains(renderer))
                    {
                        renderer.sortingLayerID = sortingLayerID;
                        renderer.sortingOrder = sortingOrder;
                        list.Add(renderer);
                    }
                }
            }
        }

        public void RemoveRenderers(Canvas canvas, Renderer[] renderers)
        {
            if (_childRendererDic.ContainsKey(canvas))
            {
                var list = _childRendererDic[canvas];
                var len = renderers.Length;

                for (int i = 0; i < len; i++)
                {
                    list.Remove(renderers[i]);
                }
            }
        }

        public void RefreshEnable()
        {
            Canvas.enabled = IsEnable;
        }

        public virtual void Close()
        {
            if (IsLoadComplete)
            {
                Canvas.enabled = false;
            }

            IsEnable = false;

            foreach (var v in EventList)
            {
                v.RemoveListener(this);
            }

            EventList.Clear();
            OnClose();
        }

        public void Enable()
        {
            if (IsLoadComplete)
            {
                Canvas.enabled = true;
            }

            IsEnable = true;
            OnEnable();
        }

        public void Disable()
        {
            if (IsLoadComplete)
            {
                Canvas.enabled = false;
            }

            IsEnable = false;
            OnDisable();
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public virtual async UniTaskVoid OnLoadComplete()
        {
        }
    }
}