using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    public class UIBaseComponent : Component
    {
        protected Canvas _canvas;

        protected CanvasGroup canvasGroup;

        protected GraphicRaycaster graphicRaycaster;

        protected bool isEnable;

        public bool IsEnable
        {
            protected set
            {
                isEnable = value;
            }
            get
            {
                return isEnable;
            }
        }

        protected StaticLinkedListDictionary<GameObject, Canvas> childCanvasDic;
        protected StaticLinkedListDictionary<Canvas, TwoStaticLinkedList<Renderer>> childRendererDic;
        protected int sortOrder;

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
            childCanvasDic = new StaticLinkedListDictionary<GameObject, Canvas>(len);
            childRendererDic = new StaticLinkedListDictionary<Canvas, TwoStaticLinkedList<Renderer>>(len);
            for (int i = 1; i < len; i++)
            {
                childCanvasDic.Add(canvasArray[i].gameObject, canvasArray[i]);
                childRendererDic.Add(canvasArray[i], new TwoStaticLinkedList<Renderer>(8));
            }
        }

        public override void Dispose()
        {
            _Dispose();
        }

        protected void _Dispose()
        {
            _canvas = null;
            graphicRaycaster = null;
            canvasGroup = null;
            childCanvasDic = null;
            childRendererDic = null;
            base.Dispose();
        }

        protected virtual void AddComponent()
        {
            _canvas = this.Entity.Transform.GetComponent<Canvas>();
            if (_canvas == null)
            {
                _canvas = this.Entity.GameObject.AddComponent<Canvas>();
            }
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = -1;

            graphicRaycaster = this.Entity.Transform.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null)
            {
                graphicRaycaster = this.Entity.GameObject.AddComponent<GraphicRaycaster>();
            }

            canvasGroup = this.Entity.Transform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = this.Entity.GameObject.AddComponent<CanvasGroup>();
            }
        }

        public void SetParentAndSortingLayer(Canvas canvas)
        {
            var sortingLayerID = canvas.sortingLayerID;
            if (this._canvas.sortingLayerID != sortingLayerID)
            {
                this.Entity.Transform.SetParent(canvas.transform);
                this._canvas.sortingLayerID = sortingLayerID;

                var data = childCanvasDic[1];
                while (data.right != 0)
                {
                    data = childCanvasDic[data.right];
                    data.element.sortingLayerID = sortingLayerID;
                }
            }
        }

        public void SetSortingOrder(int sortOrder)
        {
            if (this._canvas.sortingOrder != sortOrder)
            {
                this._canvas.sortingOrder = sortOrder;
                this.sortOrder = sortOrder;

                var sortingLayerID = this._canvas.sortingLayerID;
                var data = childCanvasDic[1];
                while (data.right != 0)
                {
                    data = childCanvasDic[data.right];
                    data.element.sortingOrder = ++this.sortOrder;

                    var sortingOrder = data.element.sortingOrder;
                    var list = this.childRendererDic[data.element];
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
            var sortingLayerID = this._canvas.sortingLayerID;
            var sortingOrder = canvas.sortingOrder;
            this.childRendererDic.Add(canvas, list);
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
            if (childRendererDic.ContainsKey(canvas))
            {
                this.childRendererDic.Remove(canvas);
                RefreshRenderers(canvas);
            }
        }

        public void AddCanvas(Canvas canvas)
        {
            if (!childCanvasDic.ContainsKey(canvas.gameObject))
            {
                canvas.sortingLayerID = this._canvas.sortingLayerID;
                canvas.sortingOrder = ++this.sortOrder;
                childCanvasDic.Add(canvas.gameObject, canvas);

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
            if (childCanvasDic.ContainsKey(canvas.gameObject))
            {
                childCanvasDic.Remove(canvas.gameObject);
                childRendererDic.Remove(canvas);
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
            if (childRendererDic.ContainsKey(canvas))
            {
                var list = childRendererDic[canvas];
                var len = renderers.Length;
                var sortingLayerID = this._canvas.sortingLayerID;
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
            if (childRendererDic.ContainsKey(canvas))
            {
                var list = childRendererDic[canvas];
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
            _canvas.enabled = true;
        }

        protected virtual void OnClose()
        {
            _OnClose();
        }

        protected void _OnClose()
        {
            _canvas.enabled = false;
            foreach (var e in eventList)
            {
                e.RemoveListener2(this);
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
            _canvas.enabled = true;
            IsEnable = true;
        }

        public virtual void Disable()
        {
            _canvas.enabled = false;
            IsEnable = false;
        }
    }
}