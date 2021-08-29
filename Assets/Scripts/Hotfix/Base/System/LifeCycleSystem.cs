using System;
using System.Collections.Generic;

namespace MGame.Hotfix
{
    public class LifeCycleSystem
    {
        private Dictionary<Type, IAwakeSystem> awakeSystems = new Dictionary<Type, IAwakeSystem>();

        public LifeCycleSystem Init()
        {
            awakeSystems.Clear();

            foreach (var v in Model.Game.Instance.Hotfix.GetHotfixTypes())
            {
                var atts = v.GetCustomAttributes(typeof(LifeCycleAttribute), false);
                if (atts.Length != 0)
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