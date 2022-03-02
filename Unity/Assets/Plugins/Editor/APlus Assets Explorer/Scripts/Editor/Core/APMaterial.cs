//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APMaterial : APAsset
    {
        [JSONDataMemberAttribute]
        public MaterialType Type { get; set; }

        [JSONDataMemberAttribute]
        public string Shader { get; set; }

        public APMaterial()
        {
            APType = AssetType.MATERIALS;
        }
    }
}
#endif