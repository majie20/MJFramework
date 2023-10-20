using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class VCameraCtrlComponent : Component, IAwake
    {
        private List<CinemachineVirtualCamera> VcamList;

        public void Awake()
        {
            //var vcamList = camera.Transform.Find("VcamList");

            var tranform = this.Entity.Transform.Find("VcamList");
            this.VcamList = new List<CinemachineVirtualCamera>(tranform.childCount);

            for (int i = tranform.childCount - 1; i >= 0; i--)
            {
                var vcam = tranform.GetChild(i).GetComponent<CinemachineVirtualCamera>();
                this.VcamList.Add(vcam);
            }

            Game.Instance.EventSystem.AddListener<E_SetMainCameraFollowTarget, Transform>(this, OnSetMainCameraFollowTarget);
        }

        public override void Dispose()
        {
            this.VcamList = null;
            base.Dispose();
        }

        private void OnSetMainCameraFollowTarget(Transform target)
        {
            for (var i = VcamList.Count - 1; i >= 0; i--)
            {
                VcamList[i].LookAt = target;
                VcamList[i].Follow = target;
            }
        }
    }
}