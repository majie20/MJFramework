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
        private string settingPath = "Assets/Res/Config/AssestSpriteSettings";
        private AssetsComponent assetsComponent;

        public void Awake()
        {
            assetsComponent = Game.Instance.Scene.GetComponent<AssetsComponent>();
            var sprintSetting = Game.Instance.Scene.GetComponent<AssetsComponent>().Load<AssestSpriteSettings>(settingPath);
            var nameList = sprintSetting.atlasNameList;
            var atlasPathList = sprintSetting.atlasPathList;
            for (int i = 0; i < nameList.Count; i++)
            {
                assetsAtlasDic.Add(nameList[i], atlasPathList[i]);
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
