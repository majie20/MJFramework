using UnityEngine;
using UnityEngine.U2D;

namespace Model
{
    [LifeCycle]
    public class UIBaseComponent : Component, IAwake
    {
        public static UIBaseComponent instance;
        public void Awake()
        {            
            instance = this;
            UIValue.Add();
        }

        public Sprite GetSprite(string imageName, string atlasName)
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            SpriteAtlas spriteAtlas =  component.Load<SpriteAtlas>(atlasName);
            Debug.Log(spriteAtlas);
            return spriteAtlas.GetSprite(imageName);
        }
    }
}