//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
namespace APlus
{
    /// <summary>
    /// Class that descripts models
    /// </summary>
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APAsset
    {
        [JSONDataMemberAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Path of asset
        /// </summary>
        [JSONDataMemberAttribute]
        public string Path { get; set; }
        
        [JSONDataMemberAttribute]
        public long FileSize { get; set; }

        [JSONDataMemberAttribute]
        public bool? Used { get; set; }
        
        [JSONDataMemberAttribute]
        public string APType { get; set; }

        public string Json { get; set; }

        [JSONDataMemberAttribute]
        public string Icon { get; set; }
        
        [JSONDataMemberAttribute]
        public string Hash { get; set; }
        
        [JSONDataMemberAttribute]
        public bool InAssetBundle { get; set;}

        [JSONDataMemberAttribute]
        public string Id { get; set;}

        public APAsset()
        {
           InAssetBundle = false; 
        }
    }
}
#endif