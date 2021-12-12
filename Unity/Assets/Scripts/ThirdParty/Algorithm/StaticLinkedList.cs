using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLinkedList<T> : IEnumerable<T>
{
    public struct Element
    {
        public int cur;
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

    public StaticLinkedList(int length)
    {
        if (length < 10)
        {
            length = 10;
        }

        this.totalLength = length;
        this.elements = new Element[length];
        for (int i = 2; i < length - 1; i++)
        {
            this.elements[i].cur = i + 1;
        }

        //0代表未使用的链表，1代表已使用的链表
        this.elements[0].cur = 2;
        this.elements[1].cur = 0;
        this.elements[length - 1].cur = 1;
    }

    public int Add(T t)
    {
        if (this.elements[0].cur == 1)
        {
            this.Expansion();
        }

        var cur = this.elements[0].cur;
        this.elements[0].cur = this.elements[cur].cur;
        this.elements[cur].cur = this.elements[1].cur;
        this.elements[1].cur = cur;
        this.elements[cur].element = t;

        Length += 1;

        return cur;
    }

    public T Remove(int index)
    {
        if (index < 2 || index >= this.elements.Length)
        {
            Debug.LogError("数组索引溢出或该索引禁止访问"); // MDEBUG:
            return default;
        }

        var lastCur = 1;
        var cur = this.elements[lastCur].cur;
        while (cur != 0)
        {
            if (cur == index)
            {
                var element = this.elements[cur].element;
                this.elements[cur].element = default;
                this.elements[lastCur].cur = this.elements[cur].cur;
                this.elements[cur].cur = this.elements[0].cur;
                this.elements[0].cur = cur;

                Length -= 1;

                return element;
            }

            lastCur = cur;
            cur = this.elements[lastCur].cur;
        }

        return default;
    }

    private void Expansion()
    {
        this.totalLength *= 2;
        var tempElements = new Element[this.totalLength];
        var length = this.elements.Length;

        for (int i = 1; i < length; i++)
        {
            tempElements[i] = this.elements[i];
        }

        for (int i = length; i < this.totalLength - 1; i++)
        {
            tempElements[i].cur = i + 1;
        }

        tempElements[0].cur = length;
        tempElements[this.totalLength - 1].cur = 1;
        this.elements = tempElements;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var cur = this.elements[1].cur;

        while (cur != 0)
        {
            yield return this.elements[cur].element;
            cur = this.elements[cur].cur;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}