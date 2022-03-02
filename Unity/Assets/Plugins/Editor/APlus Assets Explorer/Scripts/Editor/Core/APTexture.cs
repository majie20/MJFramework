//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    /// <summary>
    /// Class that descripts texture
    /// </summary>
    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APTexture : APAsset
    {
        /// <summary>
        /// Size of texture in current texture settings
        /// </summary>
        [JSONDataMemberAttribute]
        public int StorageSize { get; set; }

        /// <summary>
        /// Runtime size of texture in current texture settings
        /// </summary>
        /// <returns></returns>
        [JSONDataMemberAttribute]
#if UNITY_5 && !UNITY_5_6
        public int RuntimeSize { get; set; }
#else
        public long RuntimeSize { get; set; }
#endif

        /// <summary>
        /// Importer format of texture
        /// </summary>
        [JSONDataMemberAttribute]
        public string TextureFormat { get; set; }
        
        /// <summary>
        /// Importer type of texture
        /// </summary>
        [JSONDataMemberAttribute]
        public string TextureType { get; set; }

        /// <summary>
        /// Read and Write
        /// </summary>
        [JSONDataMemberAttribute]
        public bool ReadWrite { get; set; }
        
        /// <summary>
        /// Generate mip maps or not
        /// </summary>
        [JSONDataMemberAttribute]
        public bool MipMap { get; set; }
        
        /// <summary>
        /// Max size that unity allowed
        /// </summary>
        [JSONDataMemberAttribute]
        public int MaxSize { get; set; }
        
        /// <summary>
        /// Width of texture
        /// </summary>
        [JSONDataMemberAttribute]
        public int Width { get; set; }
        
        /// <summary>
        /// Height of texture
        /// </summary>
        [JSONDataMemberAttribute]
        public int Height { get; set; }

        [JSONDataMemberAttribute]
        public int WidthInPixel { get; set; }

        [JSONDataMemberAttribute]
        public int HeightInPixel { get; set; }

        [JSONDataMemberAttribute]
        public string Compress { get; set;}

        [JSONDataMemberAttribute]
        public string PackingTag { get; set; }

        [JSONDataMemberAttribute]
        public bool CrunchedCompression { get; set; }

        [JSONDataMemberAttribute]
        public int CompressionQuality { get; set; }
        
        public APTexture()
        {
            APType = AssetType.TEXTURES;
            PackingTag = "";
        }
    }
}
#endif