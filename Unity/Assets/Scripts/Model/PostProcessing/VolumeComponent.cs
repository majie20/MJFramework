using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Model
{
    [LifeCycle]
    public class VolumeComponent : Component, IAwake
    {
        /// <summary>
        /// 物体上对应添加的后处理相关信息
        /// </summary>
        private Dictionary<GameObject, Dictionary<string, VolumeInfo>> ObjVolumeDictionary;
        /// <summary>
        /// 创建过的后处理效果信息保存
        /// </summary>
        private Dictionary<string, VolumeInfo> VolumeInfoDic;
        /// <summary>
        /// 创建过的后处理效果保存
        /// </summary>
        private Dictionary<string, PostProcessProfile> ProfileDic;

        public void Awake()
        {
            ObjVolumeDictionary = new Dictionary<GameObject, Dictionary<string, VolumeInfo>>();
            VolumeInfoDic = new Dictionary<string, VolumeInfo>();
            ProfileDic = new Dictionary<string, PostProcessProfile>();
        }

        public override void Dispose()
        {
            ObjVolumeDictionary = null;
            VolumeInfoDic = null;
            ProfileDic = null;
        }

        /// <summary>
        /// 获取ProfileInfo中的profile
        /// </summary>
        /// <param name="name">文件名唯一</param>
        /// <returns></returns>
        private PostProcessProfile GetProfile(string name)
        {
            PostProcessProfile profile;
            if (!ProfileDic.TryGetValue(name, out profile))
            {
                profile = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<PostProcessProfile>(name);
                ProfileDic.Add(name, profile);
            }
            return profile;
        }

        /// <summary>
        /// 获取一个VolumeInfo
        /// </summary>
        /// <param name="name">资源名</param>
        /// <param name="profile">资源</param>
        /// <returns></returns>
        private VolumeInfo GetVolumeInfo(PostProcessVolume volume,string name,PostProcessProfile profile)
        {
            VolumeInfo volumeInfo;
            if (!VolumeInfoDic.TryGetValue(name,out volumeInfo))
            {
                volumeInfo = new VolumeInfo(volume, profile);
                VolumeInfoDic.Add(name, volumeInfo);
            }
            return volumeInfo;
        }

        /// <summary>
        /// 添加Volume组件
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="obj">添加到的物体</param>
        public void AddVolumeToObj(string name, GameObject obj)
        {
            var volumeInfoDic = GetObjVolumeInfoDic(obj);
            VolumeInfo volumeInfo;
            if (volumeInfoDic != null && volumeInfoDic.ContainsKey(name))
            {
                NLog.Log.Debug($"{obj.name}已经拥有名字为{name}的Volume组件");
            }
            else
            {
                var volume = obj.AddComponent<PostProcessVolume>();
                var profile = GetProfile(name);
                volumeInfo = GetVolumeInfo(volume, name, profile);
                if (volumeInfoDic == null)
                {
                    volumeInfoDic = new Dictionary<string, VolumeInfo>();
                }
                volumeInfoDic.Add(name,volumeInfo);
                ObjVolumeDictionary.Add(obj, volumeInfoDic);
                volume.profile = volumeInfo.Profile;
            }
        }

        /// <summary>
        /// 切换Volume组件的setting配置
        /// </summary>
        /// <param name="obj">物体</param>
        /// <param name="oldName">旧的组件名</param>
        /// <param name="newName">新的组件名</param>
        public void ChangeVolumeSetting(GameObject obj,string oldName, string newName)
        {
            VolumeInfo volumeInfo;
            if (VolumeInfoDic.TryGetValue(oldName, out volumeInfo))
            {
                DeletImmediateVolume(oldName, obj);
                AddVolumeToObj(newName, obj);
            }
        }

        /// <summary>
        /// 获取物体上的Volume组件
        /// </summary>
        /// <param name="obj">物体</param>
        /// <param name="name">组件名</param>
        /// <returns></returns>
        public PostProcessVolume GetObjVolume(GameObject obj, string name)
        {
            var volumeInfoDic= GetObjVolumeInfoDic(obj);
            if (volumeInfoDic!=null)
            {
                VolumeInfo volumeInfo;
                if (volumeInfoDic.TryGetValue(name, out volumeInfo))
                {
                    return volumeInfo.volume;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取物体上的Volume组件信息
        /// </summary>
        /// <param name="obj">物体</param>
        /// <param name="name">组件名</param>
        /// <returns></returns>
        public VolumeInfo GetObjVolumeInfo(GameObject obj, string name)
        {
            var volumeInfoDic = GetObjVolumeInfoDic(obj);
            if (volumeInfoDic != null)
            {
                VolumeInfo volumeInfo;
                if (volumeInfoDic.TryGetValue(name, out volumeInfo))
                {
                    return volumeInfo;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取保存物体上的Volume组件的Dic
        /// </summary>
        /// <param name="obj">物体</param>
        /// <returns></returns>
        public Dictionary<string, VolumeInfo> GetObjVolumeInfoDic(GameObject obj)
        {
            Dictionary<string, VolumeInfo> volumeInfoDic;
            if (ObjVolumeDictionary.TryGetValue(obj, out volumeInfoDic))
            {
                return volumeInfoDic;
            }
            return null;
        }

        /// <summary>
        /// 删除obj上的Volume组件
        /// </summary>
        /// <param name="name">组件名</param>
        /// <param name="obj">物体</param>
        public void DeletVolume(string name, GameObject obj)
        {
            var volumeInfoDic = GetObjVolumeInfoDic(obj);
            var volume = GetObjVolume(obj,name);
            if (volumeInfoDic != null && volume != null)
            {
                Object.Destroy(volume);
                volumeInfoDic.Remove(name);
            }
            else
            {
                NLog.Log.Debug($"{obj.name}没有名字为{name}的Volume组件");
            }
        }
        /// <summary>
        /// 删除obj上的Volume组件（Immediate）
        /// </summary>
        /// <param name="name">组件名</param>
        /// <param name="obj">物体</param>
        public void DeletImmediateVolume(string name, GameObject obj)
        {
            var volumeInfoDic = GetObjVolumeInfoDic(obj);
            var volume = GetObjVolume(obj, name);
            if (volumeInfoDic != null&&volume != null)
            {
                Object.DestroyImmediate(volume);
                volumeInfoDic.Remove(name);
            }
            else
            {
                NLog.Log.Debug($"{obj.name}没有名字为{name}的Volume组件");
            }
        }
    }
}
