using Cinemachine;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class LevelCtrlComponent : Component, IAwake, IUpdateSystem
    {
        public void Awake()
        {
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            var vcam = rc.Get<GameObject>("vcam1").GetComponent<CinemachineVirtualCamera>();
            NLog.Log.Error(vcam.name);

            Game.Instance.EventSystem.AddListener<E_LevelLoadFinish>(this, FinishLevelLoad);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {
        }
        public void FinishLevelLoad()
        {
            ReferenceCollector rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            var vcam = rc.Get<GameObject>("vcam1").GetComponent<CinemachineVirtualCamera>();
            vcam.Follow = GameObject.Find("player").transform;

        }
    }
}