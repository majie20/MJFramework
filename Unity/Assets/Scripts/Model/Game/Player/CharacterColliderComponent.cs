using System;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class CharacterColliderComponent : Component, IAwake
    {
        private PlayerCollider playerCollider;
        public void Awake()
        {
            Game.Instance.EventSystem.AddListener<E_LevelLoadFinish>(this, FinishLevelLoad);          
        }

        public void FinishLevelLoad()
        {
            playerCollider = this.Entity.GameObject.GetComponent<PlayerCollider>();
            playerCollider.CharacterColliderEventSign = "playerCollider";
            Game.Instance.EventSystem.AddListener<TargetType, string>(playerCollider.CharacterColliderEventSign, this, OnCollider);
        }

        public override void Dispose()
        {
            Game.Instance.EventSystem.RemoveListener<TargetType, string>(playerCollider.CharacterColliderEventSign, this);
        }

        public void OnCollider(TargetType targetType, string name)
        {
            if(targetType == TargetType.Item)
            {
                GetItem(name);
            }
        }

        public void GetItem(string name)
        {

        }
    }
}
