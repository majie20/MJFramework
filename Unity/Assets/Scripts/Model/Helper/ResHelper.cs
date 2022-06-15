using Model;
using System;

public class ResHelper
{
    #region GC

    public static void GCAndUnload()
    {
        Game.Instance.EventSystem.Clear();
        Game.Instance.Scene.GetComponent<AssetsComponent>().UnloadUnusedAssets();
        GC.Collect();
    }

    #endregion GC
}