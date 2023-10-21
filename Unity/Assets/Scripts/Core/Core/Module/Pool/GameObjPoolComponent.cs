using System.Collections.Generic;
using System.IO;
using M.Algorithm;
using UnityEngine;

namespace Model
{
    public class GameObjPoolComponent : Component, IAwake
    {
        public static string Ordinary_GameObject = "OrdinaryGameObject";
        public static string None_GameObject     = "NoneGameObject";

        /// <summary>
        /// 对象池中的游戏物体
        /// </summary>
        private Dictionary<string, Queue<GameObject>> gameObjDic;

        /// <summary>
        /// 对象池中游戏物体的类别，用于分类游戏物体
        /// </summary>
        private Dictionary<string, Transform> parentDic;

        private StaticLinkedListDictionary<string, TwoStaticLinkedList<GameObject>> hatchDic;

        /// <summary>
        /// 对象池的树根
        /// </summary>
        private GameObject root;

        public void Awake()
        {
            gameObjDic = new Dictionary<string, Queue<GameObject>>();
            parentDic = new Dictionary<string, Transform>();
            hatchDic = new StaticLinkedListDictionary<string, TwoStaticLinkedList<GameObject>>(4, 4);
            root = Entity.GameObject;
        }

        public override void Dispose()
        {
            gameObjDic = null;
            parentDic = null;
            hatchDic = null;
            base.Dispose();
        }

        public GameObject HatchGameObjBySign(string sign, Transform parent, bool isFromAB)
        {
            if (!hatchDic.TryGetValue(sign, out var list))
            {
                list = new TwoStaticLinkedList<GameObject>(4, 4);
                hatchDic.Add(sign, list);
            }

            if (gameObjDic.TryGetValue(sign, out var queue) && queue.Count > 0)
            {
                GameObject obj1 = queue.Dequeue();
                obj1.transform.SetParent(parent);
                obj1.name = Path.GetFileNameWithoutExtension(sign);
                list.Add(obj1);

                return obj1;
            }

            GameObject obj;

            if (isFromAB)
            {
                obj = Object.Instantiate(Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<GameObject>(sign), Vector3.zero, Quaternion.identity, parent);
            }
            else
            {
                obj = new GameObject(sign);
                obj.transform.SetParent(parent);
            }

            obj.name = Path.GetFileNameWithoutExtension(sign);
            list.Add(obj);

            return obj;
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            Queue<GameObject> queue;
            Transform parent;

            if (gameObjDic.TryGetValue(sign, out queue))
            {
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

            var list = hatchDic[sign];

            if (!list.Contains(obj))
            {
                list.Add(obj);
            }
        }

        public void DestroyGameObj(string sign, bool isHalf)
        {
            if (gameObjDic.TryGetValue(sign, out var queue))
            {
                var half = queue.Count / 2;

                for (int i = 0; i < half; i++)
                {
                    Object.Destroy(queue.Dequeue());
                }
            }
        }

        public void DestroyAllGameObj(string sign)
        {
            if (parentDic.TryGetValue(sign, out var parent))
            {
                parentDic.Remove(sign);
                Object.Destroy(parent);
            }

            if (hatchDic.TryGetValue(sign, out var list))
            {
                var data = list[1];

                while (data.right != 0)
                {
                    data = list[data.right];
                    Object.Destroy(data.element);
                }

                hatchDic.Remove(sign);
            }
        }

        public void RecycleAllGameObj(string sign)
        {
            if (hatchDic.TryGetValue(sign, out var list))
            {
                var data = list[1];

                while (data.right != 0)
                {
                    data = list[data.right];
                    RecycleGameObj(sign, data.element);
                }
            }
        }
    }
}