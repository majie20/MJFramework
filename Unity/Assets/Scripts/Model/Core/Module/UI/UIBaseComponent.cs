using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    public class UIBaseComponent : Component
    {
        public Canvas Canvas;

        public CanvasGroup CanvasGroup;

        public GraphicRaycaster GraphicRaycaster;

        public bool IsEnable;

        protected StaticLinkedListDictionary<GameObject, Canvas> _childCanvasDic;
        protected StaticLinkedListDictionary<Canvas, TwoStaticLinkedList<Renderer>> _childRendererDic;
        protected int _sortOrder;

        public virtual void Awake()
        {
            _Awake();
        }

        protected void _Awake()
        {
            AddComponent();
            IsEnable = true;

            Canvas[] canvasArray = this.Entity.Transform.GetComponentsInChildren<Canvas>(true);
            var len = canvasArray.Length;
            _childCanvasDic = new StaticLinkedListDictionary<GameObject, Canvas>(len);
            _childRendererDic = new StaticLinkedListDictionary<Canvas, TwoStaticLinkedList<Renderer>>(len);
            for (int i = 1; i < len; i++)
            {
                _childCanvasDic.Add(canvasArray[i].gameObject, canvasArray[i]);
                _childRendererDic.Add(canvasArray[i], new TwoStaticLinkedList<Renderer>(8));
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
            base.Dispose();
        }

        protected virtual void AddComponent()
        {
            Canvas = this.Entity.Transform.GetComponent<Canvas>();
            if (Canvas == null)
            {
                Canvas = this.Entity.GameObject.AddComponent<Canvas>();
            }
            Canvas.overrideSorting = true;
            Canvas.sortingOrder = -1;

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
        }

        public void SetParentAndSortingLayer(Canvas canvas)
        {
            var sortingLayerID = canvas.sortingLayerID;
            if (this.Canvas.sortingLayerID != sortingLayerID)
            {
                this.Entity.Transform.SetParent(canvas.transform);
                this.Canvas.sortingLayerID = sortingLayerID;

                var data = _childCanvasDic[1];
                while (data.right != 0)
                {
                    data = _childCanvasDic[data.right];
                    data.element.sortingLayerID = sortingLayerID;
                }
            }
        }

        public void SetSortingOrder(int sortOrder)
        {
            if (this.Canvas.sortingOrder != sortOrder)
            {
                this.Canvas.sortingOrder = sortOrder;
                this._sortOrder = sortOrder;

                var sortingLayerID = this.Canvas.sortingLayerID;
                var data = _childCanvasDic[1];
                while (data.right != 0)
                {
                    data = _childCanvasDic[data.right];
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
            var list = new TwoStaticLinkedList<Renderer>(len);
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

        public virtual void Close()
        {
            OnClose();
        }

        protected virtual void OnOpen()
        {
            Canvas.enabled = true;
        }

        protected virtual void OnClose()
        {
            _OnClose();
        }

        protected void _OnClose()
        {
            Canvas.enabled = false;
            foreach (var e in eventList)
            {
                e.RemoveListener(this);
            }
            foreach (var e in eventGroupList)
            {
                e.RemoveListener(this);
            }
            eventList.Clear();
            eventGroupList.Clear();

            called = true;
            awakeCalled = false;

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        public virtual void Enable()
        {
            Canvas.enabled = true;
            IsEnable = true;
        }

        public virtual void Disable()
        {
            Canvas.enabled = false;
            IsEnable = false;
        }
    }
}