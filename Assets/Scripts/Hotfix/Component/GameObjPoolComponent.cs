using System.Collections.Generic;
using UnityEngine;

namespace MGame.Hotfix
{
    public class GameObjPoolComponent : Component
    {
        /// <summary>
        /// 对象池中的游戏物体
        /// </summary>
        private Dictionary<string, Queue<GameObject>> gameObjDic;

        /// <summary>
        /// 对象池中游戏物体的类别，用于分类游戏物体
        /// </summary>
        private Dictionary<string, Transform> parentDic;

        /// <summary>
        /// 对象池的树根
        /// </summary>
        private GameObject root;

        public GameObjPoolComponent()
        {
        }

        public override Component Init()
        {
            base.Init();

            gameObjDic = new Dictionary<string, Queue<GameObject>>();
            parentDic = new Dictionary<string, Transform>();
            root = Entity.GameObject;

            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
            gameObjDic = null;
            parentDic = null;
        }

        public GameObject HatchGameObjByName(string name, bool isAB)
        {
            if (gameObjDic.ContainsKey(name))
            {
                return gameObjDic[name].Dequeue();
            }

            var obj = isAB ? UnityEngine.Object.Instantiate(Model.Game.Instance.Scene.GetComponent<Model.ABComponent>().GetPrefabByName(name)) : new GameObject(name);

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
                parent.SetParent(root.transform);
                parentDic.Add(sign, parent);
            }

            queue.Enqueue(obj);
            obj.transform.SetParent(parent);
        }
    }
}