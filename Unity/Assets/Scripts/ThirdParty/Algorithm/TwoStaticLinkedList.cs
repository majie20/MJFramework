using System;
using System.Collections.Generic;

namespace M.Algorithm
{
    public class TwoStaticLinkedList<T> //: IEnumerable<T>
    {
        public struct Element
        {
            public int left;
            public int right;
            public T   element;
        }

        private Element[] _elements;
        private int       _totalLength; //总长度
        private int       _effectiveLength; //有效长度
        private int       _jump;

        public int Length
        {
            private set { _effectiveLength = value; }
            get { return _effectiveLength; }
        }

        /// <summary>
        /// 长度不能小于4
        /// </summary>
        /// <param name="length"></param>
        public TwoStaticLinkedList(int jump, int length)
        {
            if (length < 4)
            {
                length = 4;
            }

            _jump = jump;
            this._totalLength = length;
            var list = new Element[length];
            this._elements = list;
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
            var list = this._elements;

            if (list[0].right == 1)
            {
                this.Expansion();
                list = this._elements;
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

        public int Add(T t, Func<T, T, bool> compare)
        {
            var list = this._elements;

            if (list[0].right == 1)
            {
                this.Expansion();
                list = this._elements;
            }

            var lastCur = 1;
            var cur = lastCur;
            var cur1 = cur;
            var b = Length <= _jump;

            while (true)
            {
                if (b)
                {
                    lastCur = cur;
                    cur = list[lastCur].right;

                    if (cur == 0 || compare(t, list[cur].element))
                    {
                        Length += 1;
                        int index;

                        if (lastCur == 1)
                        {
                            var right1 = list[0].right;
                            var right2 = list[1].right;
                            list[0].right = list[right1].right;
                            list[right1].left = 1;
                            list[right1].right = right2;
                            list[right2].left = right1;
                            list[1].right = right1;
                            list[right1].element = t;
                            index = right1;
                        }
                        else
                        {
                            if (cur == 0)
                            {
                                var right = list[0].right;
                                list[0].right = list[right].right;
                                list[right].left = lastCur;
                                list[right].right = 0;
                                list[lastCur].right = right;
                                list[right].element = t;
                                index = right;
                            }
                            else
                            {
                                var right = list[0].right;
                                list[0].right = list[right].right;
                                list[right].left = list[cur].left;
                                list[right].right = cur;
                                list[list[cur].left].right = right;
                                list[cur].left = right;
                                list[right].element = t;
                                index = right;
                            }
                        }

                        return index;
                    }

                    if (Length > _jump && cur == cur1)
                    {
                        b = false;
                    }
                }
                else
                {
                    cur1 = cur;
                    var result = false;

                    for (int i = 0; i < _jump; i++)
                    {
                        cur1 = list[cur1].right;

                        if (cur1 == 0)
                        {
                            result = true;

                            break;
                        }
                    }

                    if (result || compare(t, list[cur1].element))
                    {
                        b = true;
                    }
                    else
                    {
                        cur = cur1;
                    }
                }
            }
        }

        public void Remove(T t)
        {
            var list = this._elements;
            var lastCur = 1;
            var cur = list[lastCur].right;

            while (cur != 0)
            {
                if (EqualityComparer<T>.Default.Equals(list[cur].element, t))
                {
                    var right = list[cur].right;
                    list[cur].element = default;
                    list[lastCur].right = right;
                    list[right].left = lastCur;
                    list[cur].right = list[0].right;
                    list[0].right = cur;

                    Length -= 1;

                    return;
                }

                lastCur = cur;
                cur = list[lastCur].right;
            }
        }

        public void Remove(int index)
        {
            if (index < 2 || index >= this._totalLength)
            {
                throw new IndexOutOfRangeException("数组索引溢出或该索引禁止访问");
            }

            var list = this._elements;
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
            var list = this._elements;
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
            var list = this._elements;
            var temp = this._elements[1];

            while (temp.right != 0)
            {
                temp = list[temp.right];

                if (EqualityComparer<T>.Default.Equals(temp.element, t))
                {
                    return true;
                }
            }

            return false;
        }

        private void Expansion()
        {
            this._totalLength *= 2;
            var tempElements = new Element[this._totalLength];
            var length = this._elements.Length;
            var value = this._totalLength - 1;

            Array.Copy(this._elements, 0, tempElements, 0, length);

            for (int i = length; i < value; i++)
            {
                tempElements[i].right = i + 1;
            }

            tempElements[0].right = length;
            tempElements[value].right = 1;
            this._elements = tempElements;
        }

        public void Clear()
        {
            var list = this._elements;
            var len = this._totalLength - 1;

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
            return this._elements[i].element;
        }

        public Element this[int i]
        {
            set => _elements[i] = value;
            get => _elements[i];
        }

        //public IEnumerator<T> GetEnumerator()
        //{
        //    var list = this._elements;
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