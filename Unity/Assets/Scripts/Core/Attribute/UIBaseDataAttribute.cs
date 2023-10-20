using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIBaseDataAttribute : BaseAttribute
    {
        /// <summary>
        /// UI界面类型
        /// </summary>
        public int UIViewType;

        /// <summary>
        /// UI界面预制体地址
        /// </summary>
        public string PrefabPath;

        /// <summary>
        /// UI界面对应的私有图集地址,暂时没用
        /// </summary>
        public string AtlasPath = string.Empty;

        /// <summary>
        /// 遮罩背景显示模式
        /// </summary>
        public int UIMaskMode;

        /// <summary>
        /// UI层级
        /// </summary>
        public int UILayer;

        /// <summary>
        /// 是否设置遮罩背景
        /// </summary>
        public bool IsOperateMask = true;

        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullScreen = false;
    }
}