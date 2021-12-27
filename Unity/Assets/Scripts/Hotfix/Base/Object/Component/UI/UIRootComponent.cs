using UnityEngine;

namespace Hotfix
{
    [LifeCycle]
    public class UIRootComponent : Component, IAwake
    {
        public static string UIROOT_PATH = "Assets/Res/Prefabs/UIRoot";

        public void Awake()
        {
        }

        public override void Dispose()
        {
            Entity = null;
        }
    }
}