using System;

namespace Hotfix
{
    public class ArrayQueue<T> //: IEnumerable<T>
    {
        private int _maxSize; //最大容量
        private int _front; //队列头  指向第一个元素
        private int _rear; //队列尾  指向最后一个元素的后一位置
        private T[] _arr; //用于存放数据的数组

        public ArrayQueue(int maxSize)
        {
            this._maxSize = maxSize;
            _arr = new T[maxSize];
            _front = 0;
            _rear = 0;
        }

        //判断是否已满
        public bool IsFull()
        {
            return (_rear + 1) % _maxSize == _front;
        }

        //判断队列是否为空
        public bool IsEmpty()
        {
            return _rear == _front;
        }

        //添加数据
        public void Enqueue(T n)
        {
            //判断队列是否已满
            if (IsFull())
            {
                Expansion();
            }

            //此时rear指向的是最后一个元素的后一位置，因此可以直接添加
            _arr[_rear] = n;
            //将 rear 后移
            _rear = (_rear + 1) % _maxSize;
        }

        //出队
        public T Dequeue()
        {
            //判断队列是否为空
            if (IsEmpty())
            {
                throw new Exception("队列为空");
            }

            //此时front 指向第一个元素
            T val = _arr[_front];
            _arr[_front] = default;

            //将front后移
            _front = (_front + 1) % _maxSize;

            return val;
        }

        public void Clear()
        {
            //判断队列是否为空
            if (IsEmpty())
            {
                return;
            }

            var len = this.GetSize();

            for (int i = 0; i < len; i++)
            {
                _arr[(_front + i) % _maxSize] = default;
            }

            _front = 0;
            _rear = 0;
        }

        public bool Contains(T n)
        {
            //判断队列是否为空
            if (IsEmpty())
            {
                return false;
            }

            var len = this.GetSize();

            for (int i = 0; i < len; i++)
            {
                if (_arr[(_front + i) % _maxSize].Equals(n))
                {
                    return true;
                }
            }

            return false;
        }

        //求出当前队列有效数据的个数
        public int GetSize()
        {
            return (_rear + _maxSize - _front) % _maxSize;
        }

        //显示头数据
        public T Peek()
        {
            //判断是否为空
            if (IsEmpty())
            {
                throw new Exception("队列为空");
            }

            return _arr[_front];
        }

        public T Peek(int index)
        {
            //判断是否为空
            if (IsEmpty())
            {
                throw new Exception("队列为空");
            }

            return _arr[(_front + index) % _maxSize];
        }

        private void Expansion()
        {
            int _tempMaxSize = _maxSize * 2;
            T[] _tempArr = new T[_tempMaxSize];
            var r = 0;

            while (true)
            {
                if (IsEmpty())
                {
                    _arr = _tempArr;
                    _maxSize = _tempMaxSize;
                    _rear = r;
                    _front = 0;

                    break;
                }

                _tempArr[r] = Dequeue();
                r = (r + 1) % _tempMaxSize;
            }
        }

        //public IEnumerator<T> GetEnumerator()
        //{
        //    var f = _front;

        //    while (true)
        //    {
        //        if (_rear == f)
        //        {
        //            break;
        //        }

        //        yield return _arr[f];
        //        f = (f + 1) % _maxSize;
        //    }
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return this.GetEnumerator();
        //}
    }
}