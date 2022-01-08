using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    public class UIBaseComponent : Component
    {
        private Canvas canvas;

        public Canvas Canvas
        {
            private set
            {
                canvas = value;
            }
            get
            {
                return canvas;
            }
        }

        private CanvasGroup canvasGroup;

        public CanvasGroup CanvasGroup
        {
            private set
            {
                canvasGroup = value;
            }
            get
            {
                return canvasGroup;
            }
        }

        private GraphicRaycaster graphicRaycaster;

        public GraphicRaycaster GraphicRaycaster
        {
            private set
            {
                graphicRaycaster = value;
            }
            get
            {
                return graphicRaycaster;
            }
        }

        private bool isEnable;

        public bool IsEnable
        {
            private set
            {
                isEnable = value;
            }
            get
            {
                return isEnable;
            }
        }

        public virtual void Awake()
        {
            AddComponent();
            IsEnable = true;
        }

        public override void Dispose()
        {
            Entity = null;
        }

        protected virtual void AddComponent()
        {
            Canvas = this.Entity.Transform.GetComponent<Canvas>();
            if (Canvas == null)
            {
                Canvas = this.Entity.GameObject.AddComponent<Canvas>();
            }

            Canvas.overrideSorting = true;

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
            Canvas.enabled = false;
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