using CatJson;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    public class SpriteComponent : Component, IAwake
    {
        private Dictionary<string, string> _uiSpriteInfo;
        private Dictionary<Image, string> _operateImageDic;
        private Dictionary<SpriteRenderer, string> _operateSRDic;

        public void Awake()
        {
            _operateImageDic = new Dictionary<Image, string>();
            _operateSRDic = new Dictionary<SpriteRenderer, string>();
            Init();

            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        public override void Dispose()
        {
            base.Dispose();
            _uiSpriteInfo = null;
            _operateImageDic = null;
            _operateImageDic = null;
            SpriteAtlasManager.atlasRequested -= RequestAtlas;
        }

        public void Init()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            _uiSpriteInfo = JsonParser.ParseJson<Dictionary<string, string>>(component.LoadSync<TextAsset>(FileValue.UI_Sprite_Info).text);

            component.Unload(FileValue.UI_Sprite_Info);
        }

        private void RequestAtlas(string tag, System.Action<SpriteAtlas> callback)
        {
            var path = $"{FileValue.ATLAS_PATH}{tag}.spriteatlas";
            var sa = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SpriteAtlas>(path);
            callback(sa);
        }

        public async UniTask SetSprite(Image image, string path)
        {
            if (_operateImageDic.ContainsKey(image))
            {
                _operateImageDic[image] = path;
            }
            else
            {
                _operateImageDic.Add(image, path);
            }

            Sprite sprite;
            if (_uiSpriteInfo[path] == null)
            {
                sprite = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<Sprite>(path);
            }
            else
            {
                sprite = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSubAsync<SpriteAtlas, Sprite>($"{FileValue.ATLAS_PATH}{_uiSpriteInfo[path]}.spriteatlas", path);
            }

            if (_operateImageDic[image] == path)
            {
                image.sprite = sprite;
                _operateImageDic.Remove(image);
            }
        }

        public async UniTask SetSprite(SpriteRenderer sr, string path)
        {
            if (_operateSRDic.ContainsKey(sr))
            {
                _operateSRDic[sr] = path;
            }
            else
            {
                _operateSRDic.Add(sr, path);
            }

            Sprite sprite;
            if (_uiSpriteInfo[path] == null)
            {
                sprite = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<Sprite>(path);
            }
            else
            {
                sprite = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSubAsync<SpriteAtlas, Sprite>($"{FileValue.ATLAS_PATH}{_uiSpriteInfo[path]}.spriteatlas", path);
            }

            if (_operateSRDic[sr] == path)
            {
                sr.sprite = sprite;
                _operateSRDic.Remove(sr);
            }
        }

        public async UniTask<Sprite> GetSpriteAsync(string path)
        {
            Sprite sprite;
            if (_uiSpriteInfo[path] == null)
            {
                sprite = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<Sprite>(path);
            }
            else
            {
                sprite = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSubAsync<SpriteAtlas, Sprite>($"{FileValue.ATLAS_PATH}{_uiSpriteInfo[path]}.spriteatlas", path);
            }

            return sprite;
        }

        public Sprite GetSprite(string path)
        {
            Sprite sprite;
            if (_uiSpriteInfo[path] == null)
            {
                sprite = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<Sprite>(path);
            }
            else
            {
                sprite = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSubSync<SpriteAtlas, Sprite>($"{FileValue.ATLAS_PATH}{_uiSpriteInfo[path]}.spriteatlas", path);
            }

            return sprite;
        }
    }
}