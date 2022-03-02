using System;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class BaseGraph : NodeGraph
{
    public T GetNodeFirstByType<T>() where T : Node
    {
        Type type = typeof(T);
        for (int i = 0; i < this.nodes.Count; i++)
        {
            if (type == this.nodes[i].GetType())
            {
                return (T)nodes[i];
            }
        }

        return null;
    }
}