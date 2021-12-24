using System;
using System.Collections.Generic;

namespace Model
{
    public sealed class TypeSystem : IDisposable
    {
        private Dictionary<Type, List<Type>> typeDic = new Dictionary<Type, List<Type>>();

        public Dictionary<Type, List<Type>> TypeDic
        {
            private set
            {
                typeDic = value;
            }
            get
            {
                return typeDic;
            }
        }

        public TypeSystem()
        {
            TypeDic.Clear();
            var assembly = typeof(Init).Assembly;

            foreach (var v in assembly.GetTypes())
            {
                object[] objects = v.GetCustomAttributes(typeof(BaseAttribute), false);
                if (objects.Length != 0)
                {
                    for (int i = 0; i < objects.Length; i++)
                    {
                        Type type = ((BaseAttribute)objects[i]).AttributeType;
                        if (!TypeDic.ContainsKey(type))
                        {
                            TypeDic.Add(type, new List<Type>());
                        }
                        if (!TypeDic[type].Contains(v))
                        {
                            TypeDic[type].Add(v);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
