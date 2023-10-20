using System;
using System.Collections.Generic;

namespace M.Algorithm
{
    public class StaticLinkedListDictionary<K, V> //: IEnumerable<V>
    {
        private Dictionary<K, int>     indexs;
        private TwoStaticLinkedList<V> elements;

        public int Length => indexs.Count;

        /// <summary>
        /// 长度不能小于4
        /// </summary>
        /// <param name="length"></param>
        public StaticLinkedListDictionary(int jump, int length)
        {
            if (length < 4)
            {
                length = 4;
            }

            indexs = new Dictionary<K, int>(length);
            elements = new TwoStaticLinkedList<V>(jump, length);
        }

        public bool ContainsKey(K k)
        {
            return indexs.ContainsKey(k);
        }

        public bool TryGetValue(K k, out V v)
        {
            if (indexs.TryGetValue(k, out var i))
            {
                v = elements[i].element;

                return true;
            }

            v = default;

            return false;
        }

        public bool ContainsValue(V v)
        {
            return elements.Contains(v);
        }

        public void Add(K k, V v)
        {
            indexs.Add(k, elements.Add(v));
        }

        public void Add(K k, V v, Func<V, V, bool> Compare)
        {
            indexs.Add(k, elements.Add(v, Compare));
        }

        public bool Remove(K k)
        {
            if (indexs.TryGetValue(k, out var i))
            {
                elements.Remove(i);
                indexs.Remove(k);

                return true;
            }

            return false;
        }

        public void Clear()
        {
            indexs.Clear();
            elements.Clear();
        }

        public TwoStaticLinkedList<V>.Element GetElement(int i) => elements[i];

        public V this[K k]
        {
            set
            {
                if (ContainsKey(k))
                {
                    var index = indexs[k];
                    var element = elements[index];
                    element.element = value;
                    elements[index] = element;
                }
                else
                {
                    Add(k, value);
                }
            }
            get => elements[indexs[k]].element;
        }

        public TwoStaticLinkedList<V> GetElementList()
        {
            return elements;
        }

        public Dictionary<K, int>.KeyCollection GetKeys()
        {
            return indexs.Keys;
        }

        //public IEnumerator<V> GetEnumerator()
        //{
        //    var list = this.elements;
        //    var cur = list[1].right;

        //    while (cur != 0)
        //    {
        //        yield return list[cur].element;
        //        cur = list[cur].right;
        //    }
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return this.GetEnumerator();
        //}
    }
}