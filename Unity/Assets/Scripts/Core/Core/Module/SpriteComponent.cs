using System;
using CatJson;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    [LifeCycle]
    public class SpriteComponent : Component, IAwake
    {
        public static Sprite                             Transparent = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100);
        private       Dictionary<Image, string>          _operateImageDic;
        private       Dictionary<SpriteRenderer, string> _operateSRDic;
        private       Dictionary<Action<Sprite>, string> _operateActionDic;

        public void Awake()
        {
            _operateImageDic = new Dictionary<Image, string>();
            _operateSRDic = new Dictionary<SpriteRenderer, string>();
            _operateActionDic = new Dictionary<Action<Sprite>, string>();
            Init();

            //SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        public override void Dispose()
        {
            _operateImageDic = null;
            _operateSRDic = null;
            _operateActionDic = null;
            //SpriteAtlasManager.atlasRequested -= RequestAtlas;
            base.Dispose();
        }

        public void Init()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
        }

        //private void RequestAtlas(string tag, System.Action<SpriteAtlas> callback)
        //{
        //    var path = $"{ConstData.ATLAS_PATH}{tag}.spriteatlas";
        //    var sa = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SpriteAtlas>(path);
        //    callback(sa);
        //}

        public async UniTask SetSprite(Image image, string path)
        {
            if (_operateImageDic.TryGetValue(image, out var old))
            {
                if (old == path)
                {
                    return;
                }

                _operateImageDic[image] = path;
            }
            else
            {
                _operateImageDic.Add(image, path);
            }

            var sprite = await GetSpriteAsync(path);

            if (_operateImageDic.TryGetValue(image, out var p) && p == path)
            {
                if (image != null)
                {
                    image.sprite = sprite;
                }

                _operateImageDic.Remove(image);
            }
        }

        public async UniTask SetSprite(SpriteRenderer sr, string path)
        {
            if (_operateSRDic.TryGetValue(sr, out var old))
            {
                if (old == path)
                {
                    return;
                }

                _operateSRDic[sr] = path;
            }
            else
            {
                _operateSRDic.Add(sr, path);
            }

            var sprite = await GetSpriteAsync(path);

            if (_operateSRDic.TryGetValue(sr, out var p) && p == path)
            {
                if (sr != null)
                {
                    sr.sprite = sprite;
                }

                _operateSRDic.Remove(sr);
            }
        }

        public async UniTask SetSprite(Action<Sprite> call, string path)
        {
            if (_operateActionDic.TryGetValue(call, out var old))
            {
                if (old == path)
                {
                    return;
                }

                _operateActionDic[call] = path;
            }
            else
            {
                _operateActionDic.Add(call, path);
            }

            var sprite = await GetSpriteAsync(path);

            if (_operateActionDic.TryGetValue(call, out var p) && p == path)
            {
                call.Invoke(sprite);
                _operateActionDic.Remove(call);
            }
        }

        public async UniTask<Sprite> GetSpriteAsync(string path)
        {
            return await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<Sprite>(path);
        }

        public Sprite GetSprite(string path)
        {
            return Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<Sprite>(path);
        }
    }
}