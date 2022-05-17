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
        private string settingPath = $"{FileValue.RES_PATH}Config/ScriptableObject/AssestSpriteSettings";

        public void Awake()
        {
            InitDic();
        }

        public void InitDic()
        {
            AssestSpriteSettings spriteSetting;
            spriteSetting = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<AssestSpriteSettings>(settingPath);

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
                SpriteAtlas spriteAtlas = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SpriteAtlas>(assetsAtlasDic[atlasName]);
                if (!spriteAtlas)
                {
                    NLog.Log.Error("没有这个图集");

                    return null;
                }
                Sprite sprite = spriteAtlas.GetSprite(imageName);
                if (!sprite)
                {
                    NLog.Log.Error("没有这张图片");
                    return null;
                }
                return sprite;
            }
            return null;
        }

        public override void Dispose()
        {
            assetsAtlasDic = null;
            base.Dispose();
        }
    }
}
