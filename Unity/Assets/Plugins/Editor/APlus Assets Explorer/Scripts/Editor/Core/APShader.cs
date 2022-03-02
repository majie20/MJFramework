//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APShader : APAsset
    {
        [JSONDataMemberAttribute]
        public string FileName { get; set; }

        [JSONDataMemberAttribute]
        public int LOD { get; set; }

        [JSONDataMemberAttribute]
        public bool IgnoreProjector { get; set; }

        [JSONDataMemberAttribute]
        public bool CastShadows { get; set; }

        [JSONDataMemberAttribute]
        public bool SurfaceShader { get; set; }

        [JSONDataMemberAttribute]
#if UNITY_5
        public int VariantsIncluded { get; set; }
#else
        public ulong VariantsIncluded { get; set; }
#endif

        [JSONDataMemberAttribute]
#if UNITY_5
        public int VariantsTotal { get; set; }
#else
        public ulong VariantsTotal { get; set; }
#endif

        [JSONDataMemberAttribute]
        public int RenderQueue { get; set; }

        [JSONDataMemberAttribute]
        public string RenderQueueText { get; set; }

        [JSONDataMemberAttribute]
        public string DisableBatching { get; set; }

        public APShader()
        {
            APType = AssetType.SHADERS;
        }
    }
}
#endif