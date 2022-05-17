using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Model
{
    public class GameObjPoolComponent : Component, IAwake
    {
        public static string Ordinary_GameObject = "OrdinaryGameObject";
        public static string None_GameObject = "NoneGameObject";

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

        public void Awake()
        {
            gameObjDic = new Dictionary<string, Queue<GameObject>>();
            parentDic = new Dictionary<string, Transform>();
            root = Entity.GameObject;
        }

        public override void Dispose()
        {
            gameObjDic = null;
            parentDic = null;
            base.Dispose();
        }

        public async UniTask<GameObject> HatchGameObjByName(string name, Transform parent, bool isAB)
        {
            if (gameObjDic.ContainsKey(name))
            {
                var queue = gameObjDic[name];
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }

            var obj = isAB ? UnityEngine.Object.Instantiate(await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<GameObject>(name), Vector3.zero, Quaternion.identity, parent) : new GameObject(name);
            obj.name = Path.GetFileName(name);

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