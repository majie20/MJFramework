using System.Collections.Generic;
using UnityEngine;

namespace MGame.Model
{
    public class GameObjPoolComponent : Component
    {
        private Dictionary<string, Queue<GameObject>> gameObjDic;
        private Dictionary<string, Transform> parentDic;
        private GameObject body;

        public GameObjPoolComponent()
        {
        }

        public override Component Init()
        {
            base.Init();
            gameObjDic = new Dictionary<string, Queue<GameObject>>();
            parentDic = new Dictionary<string, Transform>();
#if UNITY_EDITOR
            body = entity.gameObject;
#else
            body = new GameObject("ObjectPool");
#endif
            body.SetActive(false);
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
            gameObjDic = null;
            parentDic = null;
        }

        public GameObject GetGameObjByName(string name)
        {
            if (gameObjDic.ContainsKey(name))
            {
                return gameObjDic[name].Dequeue();
            }

            var obj = UnityEngine.Object.Instantiate(Game.Instance.Scene.GetComponent<ABComponent>().GetPrefabByName(name));

            return obj;
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
    }
}