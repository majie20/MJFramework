using System;
using System.Collections;
using System.Collections.Generic;

namespace Model
{
    public class TwoStaticLinkedList<T> : IEnumerable<T>
    {
        public struct Element
        {
            public int left;
            public int right;
            public T element;
        }

        private Element[] elements;
        private int totalLength;//总长度
        private int effectiveLength;//有效长度

        public int Length
        {
            private set
            {
                effectiveLength = value;
            }
            get
            {
                return effectiveLength;
            }
        }

        /// <summary>
        /// 长度不能小于8
        /// </summary>
        /// <param name="length"></param>
        public TwoStaticLinkedList(int length)
        {
            if (length < 8)
            {
                length = 8;
            }
            this.totalLength = length;
            var list = new Element[length];
            this.elements = list;
            var value = length - 1;

            for (int i = 2; i < value; i++)
            {
                list[i].right = i + 1;
            }

            //[0]的元素代表未使用的链表的起点，[1]的元素代表已使用的链表的起点
            list[0].right = 2;
            list[1].right = 0;
            list[value].right = 1;
            this.Length = 0;
        }

        public int Add(T t)
        {
            var list = this.elements;
            if (list[0].right == 1)
            {
                this.Expansion();
            }

            var right1 = list[0].right;
            var right2 = list[1].right;
            list[0].right = list[right1].right;
            list[right1].left = 1;
            list[right1].right = right2;
            list[right2].left = right1;
            list[1].right = right1;
            list[right1].element = t;

            Length += 1;

            return right1;
        }

        public void Remove(T t)
        {
            var list = this.elements;
            var lastCur = 1;
            var cur = list[lastCur].right;
            while (cur != 0)
            {
                if (list[cur].Equals(t))
                {
                    var right = list[cur].right;
                    list[cur].element = default;
                    list[lastCur].right = right;
                    list[right].left = lastCur;
                    list[cur].right = list[0].right;
                    list[0].right = cur;

                    Length -= 1;
                }

                lastCur = cur;
                cur = list[lastCur].right;
            }
        }

        public void Remove(int index)
        {
            if (index < 2 || index >= this.totalLength)
            {
                NLog.Log.Error("数组索引溢出或该索引禁止访问"); // MDEBUG:
            }

            var list = this.elements;
            var left = list[index].left;
            var right = list[index].right;
            list[index].element = default;
            list[left].right = right;
            list[right].left = left;
            list[index].right = list[0].right;
            list[0].right = index;
            Length -= 1;
        }

        public T Remove(Func<T, bool> call)
        {
            var list = this.elements;
            var lastCur = 1;
            var cur = list[lastCur].right;
            while (cur != 0)
            {
                if (call(list[cur].element))
                {
                    var element = list[cur].element;
                    var right = list[cur].right;
                    list[cur].element = default;
                    list[lastCur].right = right;
                    list[right].left = lastCur;
                    list[cur].right = list[0].right;
                    list[0].right = cur;

                    Length -= 1;
                    return element;
                }

                lastCur = cur;
                cur = list[lastCur].right;
            }

            return default;
        }

        public bool Contains(T t)
        {
            var list = this.elements;
            var temp = this.elements[1];
            while (temp.right != 0)
            {
                temp = list[temp.right];
                if (temp.element.Equals(t))
                {
                    return true;
                }
            }
            return false;
        }

        private void Expansion()
        {
            this.totalLength *= 2;
            var list = this.elements;
            var tempElements = new Element[this.totalLength];
            var length = this.elements.Length;
            var value = this.totalLength - 1;

            for (int i = 1; i < length; i++)
            {
                tempElements[i] = list[i];
            }

            for (int i = length; i < value; i++)
            {
                tempElements[i].right = i + 1;
            }

            tempElements[0].right = length;
            tempElements[value].right = 1;
            this.elements = tempElements;
        }

        public void Clear()
        {
            var list = this.elements;
            var len = this.totalLength - 1;
            for (int i = 2; i < len; i++)
            {
                list[i].right = i + 1;
                list[i].element = default;
            }

            //0代表未使用的链表，1代表已使用的链表
            list[0].right = 2;
            list[1].right = 0;
            list[len].right = 1;
            list[len].element = default;
            this.Length = 0;
        }

        public T GetValue(int i)
        {
            var e = this.elements[i];
            if (e.element.Equals(default))
            {
                return default;
            }

            return e.element;
        }

        public Element this[int i]
        {
            get
            {
                return this.elements[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
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