using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Model
{
    public class VolumeInfo
    {
        public PostProcessVolume volume;
        private PostProcessProfile profile;

        public PostProcessProfile Profile
        {
            get
            {
                return profile;
            }
            set
            {
                volume.profile = value;
            }
        }

        public VolumeInfo(PostProcessVolume volume, PostProcessProfile profile)
        {
            this.volume = volume;
            this.profile = profile;
        }


        public void AddToObj(GameObject obj)
        {
            volume = obj.AddComponent<PostProcessVolume>();
        }
        /// <summary>
        /// 是否全局
        /// </summary>
        public bool IsGlobal
        {
            set
            {
                volume.isGlobal = value;
            }
            get
            {
                return volume.isGlobal;
            }
        }

        /// <summary>
        /// 混合距离
        /// </summary>
        public float BlendDistance
        {
            set
            {
                volume.blendDistance = value;
            }
            get
            {
                return volume.blendDistance;
            }
        }

        /// <summary>
        /// 权重 
        /// </summary>
        public float Weight
        {
            set
            {
                volume.weight = value;
            }
            get
            {
                return volume.weight;
            }
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public float Priority
        {
            set
            {
                volume.priority = value;
            }
            get
            {
                return volume.priority;
            }
        }
    }
}
