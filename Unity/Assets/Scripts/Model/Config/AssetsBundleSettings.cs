using Sirenix.OdinInspector;
using System.Collections;
using System.Reflection;
using UnityEngine;
using YooAsset;

namespace Model
{
    public enum EncryptType
    {
        Empty,
        Offset,
    }

    [CreateAssetMenu(fileName = "AssetsBundleSettings", menuName = "ScriptableObjects/AssetsBundleSettings", order = 1)]
    public class AssetsBundleSettings : ScriptableObject
    {
        [LabelText("是否是单机游戏")]
        public bool IsOfflineGame;

        [LabelText("加密类型")]
        [OnValueChanged("OnValueChanged")]
        public EncryptType EncryptType;

        [LabelText("加密密码")]
        public string EncryptPassword;

        [LabelText("加密偏移量")]
        public int EncryptOffsetVolume;

        [ValueDropdown("GetPlayModeValues", ExpandAllMenuItems = true)]
        [LabelText("运行模式")]
        public YooAssets.EPlayMode PlayMode;

#if UNITY_EDITOR

        public static IEnumerable GetPlayModeValues = new ValueDropdownList<YooAssets.EPlayMode>()
        {
            {"编辑器模拟模式", YooAssets.EPlayMode.EditorPlayMode},
            {"单机模式", YooAssets.EPlayMode.OfflinePlayMode},
            {"联机模式", YooAssets.EPlayMode.HostPlayMode},
        };

        private void OnValueChanged()
        {
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            Assembly.Load("Unity.Editor").GetType("EditorHelper")
                .InvokeMember("RefreshAssetBundleBuilderSetting", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
        }

#endif
    }
}