using System;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class SpriteComponent : Component, IAwake
    {
        private Dictionary<string, string> assetsAtlasDic = new Dictionary<string, string>();
        private string settingNoABPath = $"{FileValue.NO_BUILD_AB_RES_PATH}Config/ScriptableObject/AssestSpriteSettings";
        private string settingABPath = $"{FileValue.BUILD_AB_RES_PATH}Config/ScriptableObject/AssestSpriteSettings";
        private AssetsComponent assetsComponent;

        public void Awake()
        {

            InitDic(false);
        }

        public void InitDic(bool isAB)
        {
            AssestSpriteSettings spriteSetting;
            if (isAB)
            {
                assetsComponent = Game.Instance.Scene.GetComponent<AssetsComponent>();
                spriteSetting = Game.Instance.Scene.GetComponent<AssetsComponent>().Load<AssestSpriteSettings>(settingABPath);
            }
            else
            {
                assetsComponent = this.Entity.GetComponent<AssetsComponent>();
                spriteSetting = assetsComponent.Load<AssestSpriteSettings>(settingNoABPath);
            }

            var nameList = spriteSetting.atlasNameList;
            var atlasPathList = spriteSetting.atlasPathList;
            for (int i = 0; i < nameList.Count; i++)
            {
                if (!assetsAtlasDic.ContainsKey(nameList[i]))
                {
                    assetsAtlasDic.Add(nameList[i], atlasPathList[i]);
                }
            }
        }

        public Sprite LoadSprite(string imageName, string atlasName)
        {
            if (assetsAtlasDic.ContainsKey(atlasName))
            {
                SpriteAtlas spriteAtlas = assetsComponent.Load<SpriteAtlas>(assetsAtlasDic[atlasName]);
                if (!spriteAtlas)
                {
                    Debug.LogError("没有这个图集");

                    return null;
                }
                Sprite sprite = spriteAtlas.GetSprite(imageName);
                if (!sprite)
                {
                    Debug.LogError("没有这张图片");
                    return null;
                }
                return sprite;
            }
            return null;
        }

        public override void Dispose()
        {
            assetsComponent = null;
            assetsAtlasDic = null;
            Entity = null;
        }
    }
}
