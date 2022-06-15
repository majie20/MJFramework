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

        public async UniTask<GameObject> HatchGameObjBySign(string sign, Transform parent, bool isAB, bool isAsync)
        {
            if (gameObjDic.ContainsKey(sign))
            {
                var queue = gameObjDic[sign];
                if (queue.Count > 0)
                {
                    GameObject obj1 = queue.Dequeue();
                    obj1.transform.SetParent(parent);
                    obj1.name = Path.GetFileNameWithoutExtension(sign);
                    return obj1;
                }
            }

            GameObject origin;
            if (isAsync)
            {
                origin = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<GameObject>(sign);
            }
            else
            {
                origin = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<GameObject>(sign);
            }

            GameObject obj;
            if (isAB)
            {
                obj = UnityEngine.Object.Instantiate(origin, Vector3.zero, Quaternion.identity, parent);
            }
            else
            {
                obj = new GameObject(sign);
                obj.transform.SetParent(parent);
            }
            obj.name = Path.GetFileNameWithoutExtension(sign);

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
                parent.localPosition = Vector3.one * 10000;
                parentDic.Add(sign, parent);
            }

            queue.Enqueue(obj);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
        }
    }
}