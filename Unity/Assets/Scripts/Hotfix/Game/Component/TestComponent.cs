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
            //_component = this;
            NLog.Log.Error($"Awake:{a}"); // MDEBUG:
        }

        private int           i = 0;
        //private TestComponent _component;

        public void OnUpdate(float tick)
        {
            //i++;
            //NLog.Log.Error($"OnUpdate:{tick}_{i}"); // MDEBUG:
            //var a = new Vector3(i, i, i);
        }
    }
}