using UnityEngine;

namespace Model
{
    [LifeCycle]
    [UIBaseData(Type = UIViewType.Normal, PrefabPath = "")]
    public class UIBaseComponent : Component, IAwake
    {

        private LifecycleHandle lifecycleHandle;


        public virtual void Awake()
        {
            lifecycleHandle = this.Entity.Transform.GetComponent<LifecycleHandle>();
            if (lifecycleHandle == null)
            {
                lifecycleHandle = this.Entity.GameObject.AddComponent<LifecycleHandle>();
            }

            lifecycleHandle.EnableEventSign = $"UILifecycleEnable{this.Guid}";
            lifecycleHandle.DisableEventSign = $"UILifecycleDisable{this.Guid}";
            lifecycleHandle.DestroyEventSign = $"UILifecycleDestroy{this.Guid}";
        }

        public virtual void OnOpen()
        {
            Game.Instance.EventSystem.AddListener(lifecycleHandle.EnableEventSign, OnUIEnable);
            Game.Instance.EventSystem.AddListener(lifecycleHandle.DisableEventSign, OnUIDisable);
            Game.Instance.EventSystem.AddListener(lifecycleHandle.DestroyEventSign, OnUIDestroy);
        }

        public virtual void OnClose()
        {
            Game.Instance.EventSystem.RemoveListener(lifecycleHandle.EnableEventSign, OnUIEnable);
            Game.Instance.EventSystem.RemoveListener(lifecycleHandle.DisableEventSign, OnUIDisable);
            Game.Instance.EventSystem.RemoveListener(lifecycleHandle.DestroyEventSign, OnUIDestroy);
        }

        protected virtual void OnUIEnable()
        {

        }

        protected virtual void OnUIDisable()
        {

        }

        protected virtual void OnUIDestroy()
        {

        }
    }
}