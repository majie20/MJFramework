using UnityEngine;

namespace Hotfix
{
    [Model.LifeCycle]
    public class TestComponent : Model.Component, IAwake<string>, IUpdateSystem
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        public void Awake(string a)
        {
            NLog.Log.Debug($"Awake:{a}"); // MDEBUG:
        }

        public void OnUpdate(float tick)
        {
            NLog.Log.Debug($"OnUpdate:{tick}"); // MDEBUG:
        }
    }
}