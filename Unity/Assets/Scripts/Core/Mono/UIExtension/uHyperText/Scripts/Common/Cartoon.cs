using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WXB
{
    [System.Serializable]
    public class DSprite : ISprite
    {
        public DSprite(Sprite sprite, string name = null)
        {
            this.sprite = sprite;
            this.name = name;
        }

        public Sprite sprite;

        public string name
        {
            get;
            set;
        }

        public int width { get { return (int)sprite.rect.height; } }
        public int height { get { return (int)sprite.rect.width; } }

        public void AddRef() { }

        public void SubRef() { }

        // 请求资源
        public Sprite Get() { return sprite; }
    }

    [System.Serializable]
    public class Cartoon
    {
        [System.Serializable]
        public class Frame
        {
            public ISprite sprite;
            public float delay;
        }

        public string name; // 动画名
        public Frame[] frames; // 精灵序列桢
        public float space = 2f; // 与其他元素之间的间隔

        public int width;
        public int height;
    }

    [System.Serializable]
    public class OCartoon : Cartoon
    {
        public Frame frame; // 精灵序列桢
        public int row; // 精灵序列桢行数
        public int col; // 精灵序列桢列数
    }
}