//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
namespace APlus
{
    using UnityEngine;

    [JSONRootAttribute]
    [System.SerializableAttribute]
    public class APAudio : APAsset
    {
        [JSONDataMemberAttribute]
        public string Duration { get; set; }

        [JSONDataMemberAttribute]
        public long ImportedSize { get; set; }

        [JSONDataMemberAttribute]
        public float Ratio { get; set; }

        [JSONDataMemberAttribute]
        public float Quality { get; set; }

        [JSONDataMemberAttribute]
        public AudioCompressionFormat Compress { get; set; }

        [JSONDataMemberAttribute]
        public int Frequency { get; set; }

        [JSONDataMemberAttribute]
        public bool Background { get; set; }

        public APAudio()
        {
            APType = AssetType.AUDIOS;
        }
    }
}
#endif

