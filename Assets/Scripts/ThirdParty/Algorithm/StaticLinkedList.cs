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
    private int length;

    public StaticLinkedList(int length)
    {
        if (length < 10)
        {
            length = 10;
        }

        this.length = length;
        this.elements = new Element[length];
        for (int i = 2; i < length - 1; i++)
        {
            this.elements[i].cur = i + 1;
        }

        this.elements[0].cur = 2;
        this.elements[1].cur = 0;
        this.elements[length - 1].cur = 1;
    }

    public int Length()
    {
        var cur = this.elements[1].cur;
        var i = 0;
        while (cur != 0)
        {
            i++;
            cur = this.elements[cur].cur;
        }

        return i;
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

        return cur;
    }

    public T Remove(int index)
    {
        if (index < 2 || index >= this.elements.Length)
        {
            Debug.Log("数组索引溢出或该索引禁止访问"); // MDEBUG:
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

                return element;
            }

            lastCur = cur;
            cur = this.elements[cur].cur;
        }

        return default;
    }

    private void Expansion()
    {
        this.length *= 2;
        var elements = new Element[this.length];
        var length = this.elements.Length;

        for (int i = 1; i < length; i++)
        {
            elements[i] = this.elements[i];
        }

        for (int i = length; i < this.length - 1; i++)
        {
            elements[i].cur = i + 1;
        }

        elements[0].cur = length;
        elements[this.length - 1].cur = 1;
        this.elements = elements;
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