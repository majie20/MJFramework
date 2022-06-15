using System;
using System.Collections.Generic;
using System.Reflection;

namespace Model
{
    public enum UIViewType
    {
        None,
        Normal,
        Pop,
        Tips
    }

    public enum UIViewLayer
    {
        None,
        Low,
        Normal,
        High,
        Top,
    }

    public enum UIMaskMode
    {
        None,
        Transparent,
        TransparentClick,
        TransparentPenetrate,
        BlackTransparent,
        BlackTransparentClick,
        BlackTransparentPenetrate,
    }

    public class UIValue
    {
        public static Type MASK_TYPE = typeof(UIBlackMaskComponent);
        public static UIBaseDataAttribute MASK_TYPE_ATTR = UIValue.GetUIBaseDataAttribute(MASK_TYPE);

        public static Dictionary<UIViewLayer, string> LayerNames = new Dictionary<UIViewLayer, string>()
        {
            {UIViewLayer.Low, "Low"},
            {UIViewLayer.Normal, "Normal"},
            {UIViewLayer.High, "High"},
            {UIViewLayer.Top, "Top"},
        };

        public static UIBaseDataAttribute GetUIBaseDataAttribute(Type type)
        {
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                var attrs = type.GetCustomAttributes(typeof(UIBaseDataAttribute), false);
                if (attrs.Length > 0)
                {
                    if (attrs[0] is UIBaseDataAttribute attr)
                    {
                        return attr;
                    }
                }
            }
            else
            {
                return type.GetCustomAttribute<UIBaseDataAttribute>();
            }

            return null;
        }
    }
}