//  Copyright (c) 2016 amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    using UnityEditor;
    
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APFont : APAsset
    {
        [JSONDataMemberAttribute]
        public FontRenderingMode RenderingMode { get; set; }
        
        [JSONDataMemberAttribute]
        public FontTextureCase Character { get; set; }
        
        [JSONDataMemberAttribute]
        public string FontNames { get; set; }
        
        public APFont()
        {
            APType = AssetType.FONTS;
        }
    }
}
#endif

