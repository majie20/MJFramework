using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace Model
{
    [LifeCycle]
    public class UIBaseComponent : Component, IAwake
    {
        public void Awake()
        {            
            UIValue.Add();
        }

        public Sprite GetSprite(string imageName, string atlasName)
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath(component.LoadAtlasPath(atlasName) + ".spriteAtlas", typeof(Object)) as SpriteAtlas;

            if (!spriteAtlas)
            {
                Debug.LogError("没有这个图集");

                return default;
            }
            Sprite sprite = spriteAtlas.GetSprite(imageName);
            if(!sprite)
            {
                Debug.LogError("没有这张图片");
                return default;
            }
            return sprite;
        }
    }
}