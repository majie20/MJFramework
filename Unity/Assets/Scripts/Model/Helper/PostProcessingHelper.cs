using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Model
{
    public class PostProcessingHelper
    {
        #region 老版的后处理
        /// <summary>
        /// 显示后处理效果
        /// </summary>
        /// <typeparam name="T">某个效果</typeparam>
        /// <param name="postProcessingComponent">后处理组件</param>
        /// <param name="v">后处理组件</param>
        public static void ShowEffect<T>(PostProcessVolume v = null) where T : BaseEffect
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessingComponent>();
            postProcessingComponent.ShowEffect<T>(v);
        }

        /// <summary>
        /// 恢复后处理效果
        /// </summary>
        /// <typeparam name="T">某个效果</typeparam>
        /// <param name="postProcessingComponent">后处理组件</param>
        /// <param name="v">后处理组件</param>
        public static void RecoverEffect<T>(PostProcessVolume v = null) where T : BaseEffect
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessingComponent>();
            postProcessingComponent.RecoverEffect<T>(v);
        }

        /// <summary>
        /// 缓动显示后处理效果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="postProcessingComponent">后处理组件</param>
        /// <param name="t">（0-1：0表示开始效果，1表示最终效果）</param>
        /// <param name="v">后处理组件</param>
        public static void ShowLerpEffect<T>(float t, PostProcessVolume v = null) where T : BaseEffect
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessingComponent>();
            postProcessingComponent.ShowLerpEffect<T>(t, v);
        }

        /// <summary>
        /// 缓动显示后处理效果（0-1：0表示开始效果，1表示最终效果）
        /// </summary>
        /// <typeparam name="T">某个效果</typeparam>
        /// <param name="postProcessingComponent">后处理组件</param>
        /// <param name="t">（0-1：0表示开始效果，1表示最终效果）</param>
        /// <param name="v">后处理组件</param>
        public static void RecoverLerpEffect<T>(float t, PostProcessVolume v = null) where T : BaseEffect
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessingComponent>();
            postProcessingComponent.RecoverLerpEffect<T>(t, v);
        }
        #endregion

        #region 新版的后处理
        /// <summary>
        /// 恢复后处理效果
        /// </summary>
        /// <param name="name">需要替换的后处理效果文件名</param>
        /// <param name="v">后处理组件</param>
        public static void NewShowEffect(string name,PostProcessVolume v = null)
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessAssetComponent>();
            postProcessingComponent.ShowEffect(name,v);
        }

        /// <summary>
        /// 恢复后处理效果
        /// </summary>
        /// <param name="v">后处理组件</param>
        public static void NewRecoverEffect(PostProcessVolume v = null)
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessAssetComponent>();
            postProcessingComponent.RecoverEffect(v);
        }


        /// <summary>
        /// 恢复后处理效果
        /// </summary>
        /// <param name="name">需要替换的后处理效果文件名</param>
        /// <param name="v">后处理组件</param>
        public static void NewDirectShowEffect(string name, PostProcessVolume v = null)
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessAssetComponent>();
            postProcessingComponent.DirectShowEffect(name, v);
        }

        /// <summary>
        /// 恢复后处理效果
        /// </summary>
        /// <param name="v">后处理组件</param>
        public static void NewDirectRecoverEffect(PostProcessVolume v = null)
        {
            var postProcessingComponent = Game.Instance.Scene.GetComponent<PostProcessAssetComponent>();
            postProcessingComponent.DirectRecoverEffect(v);
        }
        #endregion
    }
}
