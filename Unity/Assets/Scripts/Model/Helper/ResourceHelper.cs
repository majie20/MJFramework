using System.Threading.Tasks;
using UnityEngine;

namespace Model
{
    public class ResourceHelper
    {
        /// <summary>
        /// 创建一个Sprie
        /// </summary>
        /// <param name="path">图片地址</param>
        public static async Task<Sprite> CreateSprite(string path, FileHelper.LoadMode mode)
        {
            // 创建Texture
            Texture2D texture = new Texture2D(256, 256);
            byte[] buffer = null;

            switch (mode)
            {
                case FileHelper.LoadMode.Stream:
                    {
                        buffer = await FileHelper.LoadFileByStreamAsync(path);
                    }
                    break;

                case FileHelper.LoadMode.UnityWebRequest:
                    {
                        buffer = await FileHelper.LoadFileByUnityWebRequestAsync(path);
                    }
                    break;
            }

            texture.LoadImage(buffer);
            //创建Sprite
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }
    }
}