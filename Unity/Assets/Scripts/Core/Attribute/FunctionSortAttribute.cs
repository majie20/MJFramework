using System;

namespace Model
{
    public enum FunctionLayer
    {
        Low,
        Normal,
        High,
    }

    /// <summary>
    /// Layer等级越高越先执行，Order越大越先执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionSortAttribute : BaseAttribute
    {
        public FunctionLayer Layer;
        public int           Order;

        public FunctionSortAttribute(FunctionLayer layer, int order)
        {
            Layer = layer;
            Order = order;
        }

        public FunctionSortAttribute(int order)
        {
            Layer = FunctionLayer.Normal;
            Order = order;
        }

        public FunctionSortAttribute(FunctionLayer layer)
        {
            Layer = layer;
            Order = -1;
        }
    }
}