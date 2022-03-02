//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APFile : APAsset
    {

    }

    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APOtherFile : APFile
    {
        [JSONDataMemberAttribute]
        public string FileType { get; set; }

        public APOtherFile()
        {
            APType = AssetType.OTHERS;
        }
    }

    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APStreamingAssetsFile : APFile
    {
        public APStreamingAssetsFile()
        {
            APType = AssetType.STREAMING_ASSETS;
        }
    }

    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APCodeFile : APFile
    {
        [JSONDataMemberAttribute]
        public string FileType { get; set; }

        [JSONDataMember]
        public string ContainTags { get; set; }

        public APCodeFile()
        {
            APType = AssetType.CODE;
        }
    }

    [JSONRootAttribute]
    public class APHierarchyAsset : APAsset
    {
        [JSONDataMemberAttribute]
        public string FileType { get; set; }
    }
}

#endif