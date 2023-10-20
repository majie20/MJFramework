using UnityEngine;

namespace Model

{
    [Model.LifeCycle]
    public class Test1Component : Model.Component, IAwake<int>, IUpdateSystem, ILateUpdateSystem, IFixedUpdateSystem
    {
        private int aa;
        public override void Dispose()
        {
            base.Dispose();
        }

        public void Awake(int a)
        {
            aa = a;
        }

        public void OnUpdate(float tick)
        {
            //NLog.Log.Error(aa);
            aa += 1;
        }

        public void OnLateUpdate()
        {
            aa += 1;
        }

        public void OnFixedUpdate(float tick)
        {
            aa += 1;
        }
    }
}