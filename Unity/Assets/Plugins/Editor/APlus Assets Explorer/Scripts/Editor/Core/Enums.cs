//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    public enum APAssetType
    {
        Texture = 1,
        MovieTexture,
        Sprite,
        AnimationClip,
        AudioClip,
        AudioMixer,
        Font,
        GUISkin,
        Material,
        Mesh,
        Model,
        PhysicMaterial,
        PhysicsMaterial2D,
        Prefab,
        Scene,
        Script,
        Shader,
        StreamingAssets,
        Blacklist,
        Others,
        VideoClip,
        None,
    }

    public class AssetType 
    {
        public const string TEXTURES = "textures";
        public const string ANIMATIONS = "animations";
        public const string MODELS = "models";
        public const string AUDIOS = "audios";
        public const string MOVIES = "movies";
        public const string MATERIALS = "materials";
        public const string SHADERS = "shaders";
        public const string FONTS = "fonts";
        public const string PREFABS = "prefabs";
        public const string STREAMING_ASSETS = "streamingassets";
        public const string CODE = "code";
        public const string BLACKLIST = "blacklist";
        public const string SCENE = "Scene";
        public const string OTHERS = "others";
    }
    
    public enum Platforms
    {
        Default,
        Standalone,
        iPhone,
        Web,
        WebGL,
        Android,
        Tizen
    }

    public enum MaterialType
	{
		Material,
		PhysicMaterial,
        PhysicsMaterial2D
	}
}
#endif