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
        Highest,
    }

    public enum UIMaskMode
    {
        Null,
        None,
        Transparent,
        TransparentClick,
        //TransparentPenetrate,
        BlackTransparent,
        BlackTransparentClick,
        //BlackTransparentPenetrate,
    }

    public class UIValue
    {
        //public static Type                MASK_TYPE      = typeof(UIBlackMaskComponent);
        //public static UIBaseDataAttribute MASK_TYPE_ATTR = UIHelper.GetUIBaseDataAttribute(MASK_TYPE);

        public static Dictionary<UIViewLayer, string> LayerNames = new Dictionary<UIViewLayer, string>()
        {
            { UIViewLayer.Low, "Low" },
            { UIViewLayer.Normal, "Normal" },
            { UIViewLayer.High, "High" },
            { UIViewLayer.Top, "Top" },
            { UIViewLayer.Highest, "Highest" },
        };
    }
}