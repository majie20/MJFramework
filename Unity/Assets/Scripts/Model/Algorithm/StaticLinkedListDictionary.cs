using System.Collections;
using System.Collections.Generic;

namespace Model
{
    public class StaticLinkedListDictionary<K, V> : IEnumerable<V>
    {
        private Dictionary<K, int> indexs;
        private TwoStaticLinkedList<V> elements;

        public int Length
        {
            get { return indexs.Count; }
        }

        /// <summary>
        /// 长度不能小于8
        /// </summary>
        /// <param name="length"></param>
        public StaticLinkedListDictionary(int length)
        {
            if (length < 8)
            {
                length = 8;
            }

            indexs = new Dictionary<K, int>(length);
            elements = new TwoStaticLinkedList<V>(length);
        }

        public bool ContainsKey(K k)
        {
            return indexs.ContainsKey(k);
        }

        public bool ContainsValue(V v)
        {
            return elements.Contains(v);
        }

        public void Add(K k, V v)
        {
            indexs.Add(k, elements.Add(v));
        }

        public void Remove(K k)
        {
            var index = indexs[k];
            elements.Remove(index);
            indexs.Remove(k);
        }

        public void Clear()
        {
            indexs.Clear();
            elements.Clear();
        }

        public TwoStaticLinkedList<V>.Element this[int i]
        {
            get { return this.elements[i]; }
        }

        public V this[K k]
        {
            get { return this.elements[this.indexs[k]].element; }
        }

        public TwoStaticLinkedList<V> GetElementList()
        {
            return elements;
        }

        public Dictionary<K, int>.KeyCollection GetKeys()
        {
            return indexs.Keys;
        }

        public IEnumerator<V> GetEnumerator()
        {
            var list = this.elements;
            var cur = list[1].right;

            while (cur != 0)
            {
                yield return list[cur].element;
                cur = list[cur].right;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}