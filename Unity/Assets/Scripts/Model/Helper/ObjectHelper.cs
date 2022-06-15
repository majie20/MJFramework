using Cysharp.Threading.Tasks;
using ILRuntime.CLR.Method;
using System;
using UnityEngine;

namespace Model
{
    public class ObjectHelper
    {
        #region CreateComponent

        public static Component _CreateComponent(Type type, Entity entity, bool isFromPool = true)
        {
            Component component;
            if (isFromPool)
            {
                component = Game.Instance.Scene.GetComponent<ComponentPoolComponent>().HatchComponent(type);
            }
            else
            {
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    component = Game.Instance.Hotfix.AppDomain.Instantiate<Component>(type.FullName);
                }
                else
                {
                    component = (Component)Activator.CreateInstance(type);
                }
            }

            entity.AddComponent(component);
            component.Entity = entity;
            component.IsRuning = true;
            component.awakeCalled = true;
            component.called = false;

            component.AddComponentParent();

            return component;
        }

        public static Component CreateComponent(Type type, Entity entity, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake iAwake = component as IAwake;
            iAwake?.Awake();

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Component CreateComponent<A>(Type type, Entity entity, A a, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake<A> iAwake = component as IAwake<A>;
            iAwake?.Awake(a);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Component CreateComponent<A, B>(Type type, Entity entity, A a, B b, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake<A, B> iAwake = component as IAwake<A, B>;
            iAwake?.Awake(a, b);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static Component CreateComponent<A, B, C>(Type type, Entity entity, A a, B b, C c, bool isFromPool = true)
        {
            Component component = _CreateComponent(type, entity, isFromPool);

            IAwake<A, B, C> iAwake = component as IAwake<A, B, C>;
            iAwake?.Awake(a, b, c);

            Game.Instance.LifecycleSystem.Add(component);

            return component;
        }

        public static T CreateComponent<T>(Entity entity, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, isFromPool);
        }

        public static T CreateComponent<T, A>(Entity entity, A a, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, a, isFromPool);
        }

        public static T CreateComponent<T, A, B>(Entity entity, A a, B b, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, a, b, isFromPool);
        }

        public static T CreateComponent<T, A, B, C>(Entity entity, A a, B b, C c, bool isFromPool = true) where T : Component
        {
            return (T)CreateComponent(typeof(T), entity, a, b, c, isFromPool);
        }

        #endregion CreateComponent

        #region RemoveComponent

        public static Component _RemoveComponent(Type type, Entity entity)
        {
            Component component = entity.GetComponent(type);
            if (component != null)
            {
                component.Dispose();
                component.IsRuning = false;
                entity.RemoveComponent(type);

                return component;
            }

            return null;
        }

        public static void RemoveComponent(Type type, Entity entity)
        {
            Component component = _RemoveComponent(type, entity);
            if (component != null)
            {
                Game.Instance.LifecycleSystem.Remove(component);
            }
        }

        public static void RemoveComponent<T>(Entity entity)
        {
            RemoveComponent(typeof(T), entity);
        }

        #endregion RemoveComponent

        #region CreateEntity

        public static async UniTask<T> CreateEntity<T>(Entity eParent, Transform parent = null, string sign = "OrdinaryGameObject", bool isFromAB = false, bool isAsync = true, bool isParentCanNull = false) where T : Entity
        {
            T entity = Game.Instance.Scene.GetComponent<EntityPoolComponent>().HatchEntity<T>();
            entity.GameObject = await Game.Instance.Scene.GetComponent<GameObjPoolComponent>().HatchGameObjBySign(sign, !isParentCanNull && parent == null ? eParent.Transform : parent, isFromAB, isAsync);
            entity.Sign = sign;

            entity.Transform = entity.GameObject.transform;
            entity.SetParent(eParent);

            entity.AddComponentView();
            return entity;
        }

        public static async UniTask<T> CreateEntity<T>(Entity eParent, Transform parent = null, string sign = "OrdinaryGameObject", bool isFromAB = false, bool isAsync = true, bool isParentCanNull = false, params Type[] types) where T : Entity
        {
            T entity = await CreateEntity<T>(eParent, parent, sign, isFromAB, isAsync, isParentCanNull);
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.CreateComponent3"];

                    using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                    {
                        ctx.PushObject(type);
                        ctx.PushObject(entity);
                        ctx.PushBool(true);
                        ctx.Invoke();
                    }
                }
                else
                {
                    CreateComponent(type, entity);
                }
            }

            return entity;
        }

        public static T CreateEntity<T>(Entity eParent, GameObject obj) where T : Entity
        {
            T entity = Game.Instance.Scene.GetComponent<EntityPoolComponent>().HatchEntity<T>();
            entity.GameObject = obj;
            entity.Sign = GameObjPoolComponent.None_GameObject;
            entity.Transform = entity.GameObject.transform;
            entity.SetParent(eParent);

            entity.AddComponentView();

            return entity;
        }

        public static T CreateEntity<T>(Entity eParent, GameObject obj, params Type[] types) where T : Entity
        {
            T entity = CreateEntity<T>(eParent, obj);
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (type is ILRuntime.Reflection.ILRuntimeType)
                {
                    IMethod method = Game.Instance.Hotfix.MethodDic["Hotfix.ObjectHelper.CreateComponent3"];

                    using (var ctx = Game.Instance.Hotfix.AppDomain.BeginInvoke(method))
                    {
                        ctx.PushObject(type);
                        ctx.PushObject(entity);
                        ctx.PushBool(true);
                        ctx.Invoke();
                    }
                }
                else
                {
                    CreateComponent(type, entity);
                }
            }

            return entity;
        }

        public static async UniTask<Entity> CreateEntity(Entity eParent, Transform parent = null, string sign = "OrdinaryGameObject", bool isFromAB = false, bool isAsync = true, bool isParentCanNull = false)
        {
            return await CreateEntity<Entity>(eParent, parent, sign, isFromAB, isAsync, isParentCanNull);
        }

        public static async UniTask<Entity> CreateEntity(Entity eParent, Transform parent = null, string sign = "OrdinaryGameObject", bool isFromAB = false, bool isAsync = true, bool isParentCanNull = false, params Type[] types)
        {
            return await CreateEntity<Entity>(eParent, parent, sign, isFromAB, isAsync, isParentCanNull, types);
        }

        public static Entity CreateEntity(Entity eParent, GameObject obj)
        {
            return CreateEntity<Entity>(eParent, obj);
        }

        public static Entity CreateEntity(Entity eParent, GameObject obj, params Type[] types)
        {
            return CreateEntity<Entity>(eParent, obj, types);
        }

        #endregion CreateEntity

        #region RemoveEntity

        public static void RemoveEntity(Entity entity)
        {
            entity.Dispose();
            Game.Instance.Scene.GetComponent<EntityPoolComponent>().RecycleEntity(entity);
        }

        #endregion RemoveEntity

        #region OpenUIView

        public static async UniTask<UIBaseComponent> _OpenUIView(Type type, bool isCloseBack = false)
        {
            var component = await Game.Instance.GGetComponent<UI2DRootComponent>().OpenUIView(type, isCloseBack);

            return component;
        }

        public static async UniTask<UIBaseComponent> OpenUIView(Type type, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen iOpen = component as IOpen;
                iOpen?.Open();
            }

            return component;
        }

        public static async UniTask<UIBaseComponent> OpenUIView<A>(Type type, A a, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A> iOpen = component as IOpen<A>;
                iOpen?.Open(a);
            }

            return component;
        }

        public static async UniTask<UIBaseComponent> OpenUIView<A, B>(Type type, A a, B b, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B> iOpen = component as IOpen<A, B>;
                iOpen?.Open(a, b);
            }

            return component;
        }

        public static async UniTask<UIBaseComponent> OpenUIView<A, B, C>(Type type, A a, B b, C c, bool isCloseBack = false)
        {
            var component = await _OpenUIView(type, isCloseBack);
            if (component == null)
            {
                NLog.Log.Error($"打开UI界面失败！===>{type.FullName}"); // MDEBUG:
            }
            else
            {
                IOpen<A, B, C> iOpen = component as IOpen<A, B, C>;
                iOpen?.Open(a, b, c);
            }

            return component;
        }

        public static async UniTask<T> OpenUIView<T>(bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A>(A a, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A, B>(A a, B b, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, b, isCloseBack);
        }

        public static async UniTask<T> OpenUIView<T, A, B, C>(A a, B b, C c, bool isCloseBack = false) where T : UIBaseComponent
        {
            return (T)await OpenUIView(typeof(T), a, b, c, isCloseBack);
        }

        #endregion OpenUIView

        #region CloseUIView

        public static void CloseUIView(Type type, bool isCloseBack = false)
        {
            UniTask.Void(async () => await Game.Instance.GGetComponent<UI2DRootComponent>().CloseUIView(type, isCloseBack));
        }

        public static void CloseUIView<T>(bool isCloseBack = false)
        {
            CloseUIView(typeof(T), isCloseBack);
        }

        #endregion CloseUIView

        #region 设置Camera

        public static void SetMainCamera()
        {
            var gameRoot = Game.Instance.Scene.GetChild("GameRoot");

            ObjectHelper.CreateComponent<VCameraCtrlComponent>(ObjectHelper.CreateEntity(gameRoot, gameRoot.Transform.Find("VcamList").gameObject));

            var mainCamera = gameRoot.GetChild("Main Camera");
            if (mainCamera != null)
            {
                ObjectHelper.CreateComponent<CameraCtrlComponent>(mainCamera);
            }
            else
            {
                ObjectHelper.CreateComponent<CameraCtrlComponent>(ObjectHelper.CreateEntity(gameRoot, gameRoot.Transform.Find("Main Camera").gameObject));
            }
        }

        #endregion 设置Camera
    }
}