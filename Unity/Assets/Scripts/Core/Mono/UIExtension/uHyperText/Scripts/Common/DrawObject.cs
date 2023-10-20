using UnityEngine;
using System;
using UnityEngine.UI;

namespace WXB
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public class DrawObject : MonoBehaviour, Draw, IClippable
    {
        private SymbolText text;
        private RectMask2D m_ParentMask;
        readonly Vector3[] m_Corners = new Vector3[4];
        private Rect rootCanvasRect
        {
            get
            {
                rectTransform.GetWorldCorners(m_Corners);

                if (canvas)
                {
                    Matrix4x4 mat = canvas.rootCanvas.transform.worldToLocalMatrix;
                    for (int i = 0; i < 4; ++i)
                        m_Corners[i] = mat.MultiplyPoint(m_Corners[i]);
                }

                // bounding box is now based on the min and max of all corners (case 1013182)

                Vector2 min = m_Corners[0];
                Vector2 max = m_Corners[0];
                for (int i = 1; i < 4; i++)
                {
                    min.x = Mathf.Min(m_Corners[i].x, min.x);
                    min.y = Mathf.Min(m_Corners[i].y, min.y);
                    max.x = Mathf.Max(m_Corners[i].x, max.x);
                    max.y = Mathf.Max(m_Corners[i].y, max.y);
                }

                return new Rect(min, max - min);
            }
        }

        private Canvas m_Canvas;
        public Canvas canvas
        {
            get
            {
                if (m_Canvas == null)
                    CacheCanvas();
                return m_Canvas;
            }
        }

        private void CacheCanvas()
        {
            var list = ListPool<Canvas>.Get();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count > 0)
            {
                // Find the first active and enabled canvas.
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].isActiveAndEnabled)
                    {
                        m_Canvas = list[i];
                        break;
                    }

                    // if we reached the end and couldn't find an active and enabled canvas, we should return null . case 1171433
                    if (i == list.Count - 1)
                        m_Canvas = null;
                }
            }
            else
            {
                m_Canvas = null;
            }

            ListPool<Canvas>.Release(list);
        }


        protected virtual void OnTransformParentChanged()
        {
            if (!isActiveAndEnabled)
                return;

            UpdateRect(Vector2.zero);
            UpdateClipParent();
        }

        protected virtual void OnDisable()
        {
            if (canvasRenderer == null)
                return;

            canvasRenderer.Clear();
            UpdateClipParent();
        }

        protected void OnEnable()
        {
            UpdateRect(Vector2.zero);
            UpdateClipParent();
        }

        public void OnInit(SymbolText text)
        {
            this.text = text;
            enabled = true;
            UpdateRect(Vector2.zero);
        }

        protected void Start()
        {
            UpdateRect(Vector2.zero);
        }

        protected virtual void Init()
        {

        }

        protected void Awake()
        {
            canvasRenderer = GetComponent<CanvasRenderer>();
            if (canvasRenderer == null)
            {
                gameObject.AddComponent<CanvasRenderer>();
            }

            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                gameObject.AddComponent<RectTransform>();
            }
            Init();
        }

        public RectTransform rectTransform { get; private set; }

        public virtual DrawType type { get { return DrawType.Default; } }
        public virtual long key { get; set; }

        private CanvasRenderer m_canvasRenderer;
        public CanvasRenderer canvasRenderer
        {
            get
            {
                if (m_canvasRenderer == null)
                {
                    m_canvasRenderer = GetComponent<CanvasRenderer>();
                    if (m_canvasRenderer == null)
                    {
                        m_canvasRenderer = gameObject.AddComponent<CanvasRenderer>();
                    }
                }

                return m_canvasRenderer;
            }
            private set => m_canvasRenderer = value;
        }

        protected void UpdateRect(Vector2 offset)
        {
            Tools.UpdateRect(rectTransform, offset);
        }

        public virtual void UpdateSelf(float deltaTime)
        {

        }

        Material m_Material;
        Texture m_Texture;

        public Material srcMat { get { return m_Material; } set { m_Material = value; } }

        public Texture texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }

        public void FillMesh(Mesh workerMesh)
        {
            canvasRenderer.SetMesh(workerMesh);
        }

        public virtual void UpdateMaterial(Material mat)
        {
            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(mat, 0);
            canvasRenderer.SetTexture(m_Texture);
        }

        public virtual void Release()
        {
            m_Material = null;
            m_Texture = null;
            key = 0;
            if (canvasRenderer != null)
            {
                canvasRenderer.Clear();
            }
        }

        public void DestroySelf()
        {
            Tools.Destroy(gameObject);
        }
        
        public void RecalculateClipping()
        {
            UpdateClipParent();
        }

        public void Cull(Rect clipRect, bool validRect)
        {
            var cull = !validRect || !clipRect.Overlaps(rootCanvasRect, true);
            UpdateCull(cull);
        }

        public void SetClipRect(Rect value, bool validRect)
        {
            if (validRect)
                canvasRenderer.EnableRectClipping(value);
            else
                canvasRenderer.DisableRectClipping();
        }

        public void SetClipSoftness(Vector2 clipSoftness)
        {
            canvasRenderer.clippingSoftness = clipSoftness;
        }

        private void UpdateClipParent()
        {
            if (text == null && this.transform.parent != null)
            {
                text = this.transform.parent.GetComponent<SymbolText>();
            }
            var newParent = (text && text.maskable && text.IsActive() && isActiveAndEnabled) ? MaskUtilities.GetRectMaskForClippable(text) : null;

            // if the new parent is different OR is now inactive
            if (m_ParentMask != null && (newParent != m_ParentMask || !newParent.IsActive()))
            {
                m_ParentMask.RemoveClippable(this);
                UpdateCull(false);
            }

            // don't re-add it if the newparent is inactive
            if (newParent != null && newParent.IsActive())
                newParent.AddClippable(this);

            m_ParentMask = newParent;
        }

        private void UpdateCull(bool cull)
        {
            if (canvasRenderer.cull != cull)
            {
                canvasRenderer.cull = cull;
                UISystemProfilerApi.AddMarker("MaskableGraphic.cullingChanged", this);
                OnCullingChanged();
            }
        }

        public virtual void OnCullingChanged()
        {
        }
    }
}