using System.Collections.Generic;
using UnityEngine;

namespace Game.Singleton
{
    public class PoolMgr : Singleton<PoolMgr>
    {
        private Dictionary<string, Queue<GameObject>> gameObjDic;
        private GameObject body;

        public override void Init()
        {
            base.Init();
            gameObjDic = new Dictionary<string, Queue<GameObject>>();
            body = new GameObject("PoolMgr");
            body.SetActive(false);
        }

        public override void Dispose()
        {
            base.Dispose();
            gameObjDic = null;
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

        public void RecycleGameObj(string sign, GameObject obj)
        {
            Queue<GameObject> queue;
            Transform parent;
            if (!gameObjDic.ContainsKey(sign))
            {
                queue = new Queue<GameObject>();
                gameObjDic.Add(sign, queue);
                parent = new GameObject(sign).transform;
                parent.SetParent(body.transform);
            }
            else
            {
                queue = gameObjDic[sign];
                parent = body.transform.Find(sign);
            }

            queue.Enqueue(obj);
            obj.transform.SetParent(parent);
        }
    }
}