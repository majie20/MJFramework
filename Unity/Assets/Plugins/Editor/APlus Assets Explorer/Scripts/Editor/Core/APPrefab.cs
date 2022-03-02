//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APPrefab : APAsset
    {
        [JSONDataMember]
        public string ContainTags { get; set; }
        
        [JSONDataMember]
        public string InLayers { get; set; }

        public APPrefab()
        {
            APType = AssetType.PREFABS;
        }
    }
}

#endif