using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Singleton
{
    public class PoolMgr : Singleton<PoolMgr>
    {
        private Dictionary<string, Queue<GameObject>> gameObjDic;
        private Dictionary<string, Transform> parentDic;
        private Dictionary<string, Queue<Model.Component>> componentDic;
        private GameObject body;

        public override void Init()
        {
            base.Init();
            gameObjDic = new Dictionary<string, Queue<GameObject>>();
            parentDic = new Dictionary<string, Transform>();
            componentDic = new Dictionary<string, Queue<Model.Component>>();
            body = new GameObject("PoolMgr");
            body.SetActive(false);
        }

        public override void Dispose()
        {
            base.Dispose();
            gameObjDic = null;
            parentDic = null;
            componentDic = null;
        }

        public GameObject GetGameObjByName(string name)
        {
            GameObject obj;
            if (gameObjDic.ContainsKey(name))
            {
                obj = gameObjDic[name].Dequeue();
            }
            else
            {
                obj = UnityEngine.Object.Instantiate(ABMgr.Instance.GetPrefabByName(name));
            }

            return obj;
        }

        public T GetComponent<T>() where T : Model.Component, new()
        {
            T component;
            Type type = typeof(T);
            if (componentDic.ContainsKey(type.Name))
            {
                component = (T)componentDic[type.Name].Dequeue();
            }
            else
            {
                component = new T();
            }

            return component;
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            Queue<GameObject> queue;
            Transform parent;
            if (gameObjDic.ContainsKey(sign))
            {
                queue = gameObjDic[sign];
                parent = parentDic[sign];
            }
            else
            {
                queue = new Queue<GameObject>();
                gameObjDic.Add(sign, queue);
                parent = new GameObject(sign).transform;
                parent.SetParent(body.transform);
                parentDic.Add(sign, parent);
            }

            queue.Enqueue(obj);
            obj.transform.SetParent(parent);
        }

        public void RecycleComponent(Model.Component component)
        {
            var name = component.GetType().Name;
            Queue<Model.Component> queue;
            if (!componentDic.ContainsKey(name))
            {
                queue = new Queue<Model.Component>();
                componentDic.Add(name, queue);
            }
            else
            {
                queue = componentDic[name];
            }

            queue.Enqueue(component);
        }
    }
}