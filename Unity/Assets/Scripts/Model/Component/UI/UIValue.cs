using System;
using System.Collections.Generic;

namespace Model
{
    public enum UIViewType
    {
        None,
        Normal,
        Pop,
        Tips
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
}