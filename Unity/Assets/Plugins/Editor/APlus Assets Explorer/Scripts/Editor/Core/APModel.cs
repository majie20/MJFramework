//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    using UnityEditor;
    
    /// <summary>
    /// Class that descripts models
    /// </summary>
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APModel : APAsset
    {
        /// <summary>
        /// Total vertex count of all meshes in model
        /// </summary>
        [JSONDataMemberAttribute]
        public int Vertexes { get; set; }
        
        /// <summary>
        /// Total tris count of all meshes in model
        /// </summary>
        [JSONDataMemberAttribute]
        public int Tris { get; set; }
        
        /// <summary>
        /// Global scale factor of model
        /// </summary>
        [JSONDataMemberAttribute]
        public float ScaleFactor { get; set; }
        
        /// <summary>
        /// Is optimize mesh?
        /// </summary>
        [JSONDataMemberAttribute]
        public bool OptimizeMesh { get; set; }
        
        /// <summary>
        /// Mesh compression type
        /// </summary>
        [JSONDataMemberAttribute]
        public ModelImporterMeshCompression MeshCompression { get; set; }

        [JSONDataMemberAttribute]
        public bool ReadWrite { get; set; }

        [JSONDataMemberAttribute]
        public bool ImportBlendShapes { get; set; }

        [JSONDataMemberAttribute]
        public bool GenerateColliders { get; set; }

        [JSONDataMemberAttribute]
        public bool SwapUVs { get; set; }

        [JSONDataMemberAttribute]
        public bool LightmapToUV2 { get; set; }
        
        public APModel()
        {
            APType = AssetType.MODELS;
        }
    }
}
#endif