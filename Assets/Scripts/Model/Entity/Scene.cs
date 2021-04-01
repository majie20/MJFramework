using System;
using System.Collections.Generic;
using UnityEngine;

namespace MGame.Model
{
    //[HideInHierarchy]
    public class Scene : Entity
    {
        public Scene()
        {
        }

        public override Entity Init(bool isAB)
        {
            componentDic = new Dictionary<Type, Component>();

            gameObject = new GameObject(GetType().Name);
            transform = gameObject.transform;
            transform.SetParent(Game.Instance.Transform);

            componentView = gameObject.AddComponent<ComponentView>();

            AddComponent(new ABComponent().Init(this));
            AddComponent(new PrefabAssociateComponent().Init(this));
            AddComponent(new TextManageComponent().Init(this));

            return this;
        }

        public override void Dispose()
        {
            foreach (var value in componentDic.Values)
            {
                value.Dispose();
            }
            componentDic = null;

            UnityEngine.Object.Destroy(gameObject);
            transform = null;
            gameObject = null;
        }
    }
}