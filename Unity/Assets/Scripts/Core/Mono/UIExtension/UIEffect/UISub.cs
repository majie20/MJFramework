using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [AddComponentMenu("UI/UIEffects/UISub", 103)]
    public class UISub : BaseMaterialEffect
    {
        [SerializeField]
        private BaseMeshEffect _parent;

        public BaseMeshEffect parent
        {
            set
            {
                if (_parent != null)
                {
                    _parent.graphic.UnregisterDirtyVerticesCallback(DirtyVerticesCallback);
                    _parent.graphic.UnregisterDirtyMaterialCallback(DirtyMaterialCallback);
                }

                _parent = value;

                if (_parent != null)
                {
                    _parentIsActiveAndEnabled = _parent != null && _parent.isActiveAndEnabled;
                    _parent.graphic.RegisterDirtyVerticesCallback(DirtyVerticesCallback);
                    _parent.graphic.RegisterDirtyMaterialCallback(DirtyMaterialCallback);
                }
            }
            get { return _parent; }
        }

        private bool _parentIsActiveAndEnabled;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!Application.isPlaying)
            {
                return;
            }

            if (parent != null)
            {
                parent.graphic.RegisterDirtyVerticesCallback(DirtyVerticesCallback);
                parent.graphic.RegisterDirtyMaterialCallback(DirtyVerticesCallback);
            }

            _parentIsActiveAndEnabled = _parent != null && _parent.isActiveAndEnabled;

            SetMaterialDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!Application.isPlaying)
            {
                return;
            }

            if (parent != null)
            {
                parent.graphic.UnregisterDirtyVerticesCallback(DirtyVerticesCallback);
                parent.graphic.UnregisterDirtyMaterialCallback(DirtyMaterialCallback);
            }
        }

        public override Hash128 GetMaterialHash(Material baseMaterial)
        {
            if (!isActiveAndEnabled)
                return k_InvalidHash;

            if (_parent != null && _parent.isActiveAndEnabled && _parent is BaseMaterialEffect effect)
            {
                return effect.GetMaterialHash(baseMaterial);
            }

            return k_InvalidHash;
        }

        public override void ModifyMaterial(Material newMaterial, Graphic graphic)
        {
            if (_parent != null && _parent.isActiveAndEnabled && _parent is BaseMaterialEffect effect)
            {
                effect.ModifyMaterial(newMaterial, graphic);
            }
            else
            {
                base.ModifyMaterial(newMaterial, graphic);
            }
        }

        /// <summary>
        /// Call used to modify mesh.
        /// </summary>
        public override void ModifyMesh(VertexHelper vh, Graphic graphic)
        {
            if (!isActiveAndEnabled)
                return;

            _parentIsActiveAndEnabled = _parent != null && _parent.isActiveAndEnabled;

            if (_parent != null && _parent.isActiveAndEnabled)
            {
                _parent.ModifyMesh(vh, graphic);
            }
        }

        public void DirtyVerticesCallback()
        {
            if (!_parent.isActiveAndEnabled || _parentIsActiveAndEnabled != _parent.isActiveAndEnabled)
            {
                SetMaterialDirty();
            }

            if (graphic != null)
            {
                graphic.SetVerticesDirty();
            }
        }

        public void DirtyMaterialCallback()
        {
            SetMaterialDirty();
        }
    }
}