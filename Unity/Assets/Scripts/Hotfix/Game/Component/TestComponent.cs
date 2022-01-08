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
            Debug.Log($"Awake:{a}"); // MDEBUG:
        }

        public void OnUpdate(float tick)
        {
            Debug.Log($"OnUpdate:{tick}"); // MDEBUG:
        }
    }
}