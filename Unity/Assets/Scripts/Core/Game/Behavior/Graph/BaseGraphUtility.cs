using System;
using GraphProcessor;

namespace Model
{
    public static class BaseGraphUtility
    {
        public static T GetNodeFirstByType<T>(this BaseGraph baseGraph) where T : BaseNode
        {
            Type type = typeof(T);

            for (int i = 0; i < baseGraph.nodes.Count; i++)
            {
                if (type == baseGraph.nodes[i].GetType())
                {
                    return (T)baseGraph.nodes[i];
                }
            }

            return null;
        }
    }
}