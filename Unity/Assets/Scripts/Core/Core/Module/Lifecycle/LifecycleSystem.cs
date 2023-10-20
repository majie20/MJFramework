using System;
using System.Collections.Generic;
using System.Reflection;
using M.Algorithm;

namespace Model
{
    public class LifecycleSystem : IDisposable
    {
        private HashSet<Type>                   startSystems       = new();
        private HashSet<Type>                   updateSystems      = new();
        private HashSet<Type>                   lateUpdateSystems  = new();
        private HashSet<Type>                   fixedUpdateSystems = new();
        private Dictionary<Type, HashSet<Type>> types              = new();

        public Dictionary<Type, HashSet<Type>> Types
        {
            get { return types; }
        }

        private StaticLinkedListDictionary<Component, IStartSystem>       starts                 = new(5, 20);
        private StaticLinkedListDictionary<Component, IUpdateSystem>      updates                = new(5, 20);
        private StaticLinkedListDictionary<Component, ILateUpdateSystem>  lateUpdates            = new(5, 20);
        private StaticLinkedListDictionary<Component, IFixedUpdateSystem> fixedUpdates           = new(5, 20);
        private bool                                                      isInUpdate             = false;
        private bool                                                      isInLateUpdate         = false;
        private bool                                                      isInFixedUpdate        = false;
        private List<Model.Component>                                     addStartList           = new();
        private List<Model.Component>                                     addUpdateList          = new();
        private List<Model.Component>                                     addLateUpdateList      = new();
        private List<Model.Component>                                     addFixedUpdateList     = new();
        private List<Component>                                           removeStartList        = new();
        private List<Component>                                           removeUpdateList       = new();
        private List<Component>                                           removeLateUpdateList   = new();
        private List<Component>                                           removeFixedUpdateList  = new();
        private Dictionary<string, FunctionSortAttribute>                 functionSortAttributes = new();
        private Func<IUpdateSystem, IUpdateSystem, bool>                  updateFunc;
        private Func<ILateUpdateSystem, ILateUpdateSystem, bool>          lateUpdateFunc;
        private Func<IFixedUpdateSystem, IFixedUpdateSystem, bool>        fixedUpdateFunc;

        public LifecycleSystem()
        {
            updateFunc = (c1,      c2) => Compare(c1, c2, "OnUpdate");
            lateUpdateFunc = (c1,  c2) => Compare(c1, c2, "OnLateUpdate");
            fixedUpdateFunc = (c1, c2) => Compare(c1, c2, "OnFixedUpdate");
        }

        public void InitAssembly(Assembly assembly)
        {
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
                            types.Add(type, new HashSet<Type>());
                        }

                        if (!types[type].Contains(v))
                        {
                            types[type].Add(v);
                        }
                    }
                }
            }
        }

        public void InitAttr()
        {
            if (types.ContainsKey(typeof(LifeCycleAttribute)))
            {
                foreach (var v in types[typeof(LifeCycleAttribute)])
                {
                    object obj = Activator.CreateInstance(v);

                    if (!startSystems.Contains(v) && obj is IStartSystem)
                    {
                        startSystems.Add(v);
                    }

                    if (!updateSystems.Contains(v) && obj is IUpdateSystem)
                    {
                        updateSystems.Add(v);
                    }

                    if (!lateUpdateSystems.Contains(v) && obj is ILateUpdateSystem)
                    {
                        lateUpdateSystems.Add(v);
                    }

                    if (!fixedUpdateSystems.Contains(v) && obj is IFixedUpdateSystem)
                    {
                        fixedUpdateSystems.Add(v);
                    }
                }
            }
        }

        public void Dispose()
        {
            types = null;
            startSystems = null;
            updateSystems = null;
            lateUpdateSystems = null;
            fixedUpdateSystems = null;

            starts = null;
            updates = null;
            lateUpdates = null;
            fixedUpdates = null;

            addStartList = null;
            addUpdateList = null;
            addLateUpdateList = null;
            addFixedUpdateList = null;

            removeStartList = null;
            removeUpdateList = null;
            removeLateUpdateList = null;
            removeFixedUpdateList = null;

            functionSortAttributes = null;
            updateFunc = null;
            lateUpdateFunc = null;
            fixedUpdateFunc = null;
        }

        public void Add(Component component)
        {
            Type type = component.GetType();

            if (isInUpdate)
            {
                if (this.startSystems.Contains(type) && !addStartList.Contains(component))
                {
                    addStartList.Add(component);
                }

                if (this.updateSystems.Contains(type) && !addUpdateList.Contains(component))
                {
                    addUpdateList.Add(component);
                }
            }
            else
            {
                if (this.startSystems.Contains(type) && !this.starts.ContainsKey(component))
                {
                    this.starts.Add(component, component as IStartSystem);
                }

                if (this.updateSystems.Contains(type) && !this.updates.ContainsKey(component))
                {
                    AddFunctionSortAttribute(type, "OnUpdate");
                    this.updates.Add(component, component as IUpdateSystem, updateFunc);
                }
            }

            if (isInLateUpdate)
            {
                if (this.lateUpdateSystems.Contains(type) && !addLateUpdateList.Contains(component))
                {
                    addLateUpdateList.Add(component);
                }
            }
            else
            {
                if (this.lateUpdateSystems.Contains(type) && !this.lateUpdates.ContainsKey(component))
                {
                    AddFunctionSortAttribute(type, "OnLateUpdate");
                    this.lateUpdates.Add(component, component as ILateUpdateSystem, lateUpdateFunc);
                }
            }

            if (isInFixedUpdate)
            {
                if (this.fixedUpdateSystems.Contains(type) && !addFixedUpdateList.Contains(component))
                {
                    addFixedUpdateList.Add(component);
                }
            }
            else
            {
                if (this.fixedUpdateSystems.Contains(type) && !this.fixedUpdates.ContainsKey(component))
                {
                    AddFunctionSortAttribute(type, "OnFixedUpdate");
                    this.fixedUpdates.Add(component, component as IFixedUpdateSystem, fixedUpdateFunc);
                }
            }
        }

        public void Remove(Component component)
        {
            Type type = component.GetType();

            if (isInUpdate)
            {
                if (this.startSystems.Contains(type))
                {
                    removeStartList.Add(component);
                }

                if (this.updateSystems.Contains(type))
                {
                    removeUpdateList.Add(component);
                }
            }
            else
            {
                this.starts.Remove(component);
                this.updates.Remove(component);
            }

            if (isInLateUpdate)
            {
                if (lateUpdateSystems.Contains(type))
                {
                    removeLateUpdateList.Add(component);
                }
            }
            else
            {
                this.lateUpdates.Remove(component);
            }

            if (isInFixedUpdate)
            {
                if (fixedUpdateSystems.Contains(type))
                {
                    removeFixedUpdateList.Add(component);
                }
            }
            else
            {
                this.fixedUpdates.Remove(component);
            }
        }

        public void Start()
        {
            var data = this.starts.GetElement(1);
            var len = removeStartList.Count;

            while (data.right != 0)
            {
                data = this.starts.GetElement(data.right);

                if (len == 0 || !removeStartList.Contains((Component)data.element))
                {
                    data.element.Start();
                }
            }

            this.starts.Clear();
        }

        public void Update(float tick)
        {
            isInUpdate = true;

            this.Start();

            var data = this.updates.GetElement(1);
            var len = removeUpdateList.Count;

            while (data.right != 0)
            {
                data = this.updates.GetElement(data.right);

                if (len == 0 || !removeUpdateList.Contains((Component)data.element))
                {
                    data.element.OnUpdate(tick);
                }
            }

            isInUpdate = false;

            //for (int i = removeStartList.Count - 1; i >= 0; i--)
            //{
            //    starts.Remove(removeStartList[i]);
            //}

            for (int i = removeUpdateList.Count - 1; i >= 0; i--)
            {
                updates.Remove(removeUpdateList[i]);
            }

            for (int i = addStartList.Count - 1; i >= 0; i--)
            {
                starts.Add(addStartList[i], addStartList[i] as IStartSystem);
            }

            for (int i = addUpdateList.Count - 1; i >= 0; i--)
            {
                AddFunctionSortAttribute(addUpdateList[i].GetType(), "OnUpdate");
                updates.Add(addUpdateList[i], addUpdateList[i] as IUpdateSystem, updateFunc);
            }

            removeStartList.Clear();
            removeUpdateList.Clear();
            addStartList.Clear();
            addUpdateList.Clear();
        }

        public void LateUpdate()
        {
            isInLateUpdate = true;

            var data = this.lateUpdates.GetElement(1);
            var len = removeLateUpdateList.Count;

            while (data.right != 0)
            {
                data = this.lateUpdates.GetElement(data.right);

                if (len == 0 || !removeLateUpdateList.Contains((Component)data.element))
                {
                    data.element.OnLateUpdate();
                }
            }

            isInLateUpdate = false;

            for (int i = removeLateUpdateList.Count - 1; i >= 0; i--)
            {
                lateUpdates.Remove(removeLateUpdateList[i]);
            }

            for (int i = addLateUpdateList.Count - 1; i >= 0; i--)
            {
                AddFunctionSortAttribute(addLateUpdateList[i].GetType(), "OnLateUpdate");
                lateUpdates.Add(addLateUpdateList[i], addLateUpdateList[i] as ILateUpdateSystem, lateUpdateFunc);
            }

            removeLateUpdateList.Clear();
            addLateUpdateList.Clear();
        }

        public void FixedUpdate(float tick)
        {
            isInFixedUpdate = true;

            var data = this.fixedUpdates.GetElement(1);
            var len = removeFixedUpdateList.Count;

            while (data.right != 0)
            {
                data = this.fixedUpdates.GetElement(data.right);

                if (len == 0 || !removeFixedUpdateList.Contains((Component)data.element))
                {
                    data.element.OnFixedUpdate(tick);
                }
            }

            isInFixedUpdate = false;

            for (int i = removeFixedUpdateList.Count - 1; i >= 0; i--)
            {
                fixedUpdates.Remove(removeFixedUpdateList[i]);
            }

            for (int i = addFixedUpdateList.Count - 1; i >= 0; i--)
            {
                AddFunctionSortAttribute(addFixedUpdateList[i].GetType(), "OnFixedUpdate");
                fixedUpdates.Add(addFixedUpdateList[i], addFixedUpdateList[i] as IFixedUpdateSystem, fixedUpdateFunc);
            }

            removeFixedUpdateList.Clear();
            addFixedUpdateList.Clear();
        }

        private bool Compare<T>(T c1, T c2, string methodName)
        {
            var b1 = functionSortAttributes.TryGetValue($"{c1.GetType().FullName}_{methodName}", out var attribute1);
            var b2 = functionSortAttributes.TryGetValue($"{c2.GetType().FullName}_{methodName}", out var attribute2);

            if (b1 && b2)
            {
                if (attribute1.Layer == attribute2.Layer)
                {
                    return attribute1.Order >= attribute2.Order;
                }

                return (int)attribute1.Layer >= (int)attribute2.Layer;
            }

            if (b2)
            {
                return attribute2.Layer == FunctionLayer.Low;
            }

            return true;
        }

        private void AddFunctionSortAttribute(Type type, string methodName)
        {
            var str = $"{type.FullName}_{methodName}";

            if (!functionSortAttributes.ContainsKey(str))
            {
                var attribute = ObjectHelper.GetFunctionSortAttribute(type, methodName);

                if (attribute != null)
                {
                    functionSortAttributes.Add(str, attribute);
                }
            }
        }
    }
}