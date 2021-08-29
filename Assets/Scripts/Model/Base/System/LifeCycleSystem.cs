using System;
using System.Collections.Generic;

namespace MGame.Model
{
    public class LifeCycleSystem
    {
        private Dictionary<Type, IAwakeSystem> awakeSystems = new Dictionary<Type, IAwakeSystem>();
        private Dictionary<Type, List<Type>> types = new Dictionary<Type, List<Type>>();

        public LifeCycleSystem Init()
        {
            types.Clear();
            var assembly = typeof(Init).Assembly;

            foreach (var v in assembly.GetTypes())
            {
                object[] objects = v.GetCustomAttributes(typeof(BaseAttribute), false);
                if (objects.Length != 0)
                {
                    for (int i = 0; i < objects.Length; i++)
                    {
                        Type type = ((BaseAttribute)objects[i]).AttributeType;
                        if (!types.ContainsKey(type))
                        {
                            types.Add(type, new List<Type>());
                        }
                        if (!types[type].Contains(v))
                        {
                            types[type].Add(v);
                        }
                    }
                }
            }

            awakeSystems.Clear();

            if (types.ContainsKey(typeof(LifeCycleAttribute)))
            {
                foreach (var v in types[typeof(LifeCycleAttribute)])
                {
                    object obj = Activator.CreateInstance(v);

                    switch (obj)
                    {
                        case IAwakeSystem objectSystem:
                            awakeSystems.Add(v, objectSystem);
                            break;
                    }
                }
            }

            return this;
        }

        public void Dispose()
        {
        }

        public void Awake(Component component)
        {
            if (awakeSystems.TryGetValue(component.GetType(), out IAwakeSystem iAwakeSystem))
            {
                IAwake iAwake = iAwakeSystem as IAwake;

                iAwake?.Awake();
            }
        }

        public void Awake<A>(Component component, A a)
        {
            if (awakeSystems.TryGetValue(component.GetType(), out IAwakeSystem iAwakeSystem))
            {
                IAwake<A> iAwake = iAwakeSystem as IAwake<A>;

                iAwake?.Awake(a);
            }
        }
    }
}