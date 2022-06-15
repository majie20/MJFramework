using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    public class LoadHelper
    {
        public static async UniTask LoadScene(LoadSceneData data)
        {
            var gameRoot = Game.Instance.Scene.GetChild("GameRoot");
            if (gameRoot != null)
            {
                ObjectHelper.RemoveEntity(gameRoot);
            }
            Game.Instance.GGetComponent<UI2DRootComponent>().ClearUIViewByLayer(UIViewLayer.Low);
            Game.Instance.GGetComponent<UI2DRootComponent>().ClearUIViewByLayer(UIViewLayer.Normal);
            Game.Instance.EventSystem.Invoke<E_LoadStateSwitch, LoadProgressType>(LoadProgressType.LoadAssets);
            ResHelper.GCAndUnload();
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            var isNull = string.IsNullOrEmpty(data.SettingsPath);

            var progressCall1 = Progress.Create<float>(f =>
            {
                Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh2, float>(isNull ? f : f * 0.5f);
            });

            var sceneHandle = component.CreateLoadSceneHandle(data.ScenePath);
            await sceneHandle.ToUniTask(progressCall1);
            progressCall1.Report(1);

            if (!isNull)
            {
                var settings = await component.LoadAsync<AssetReferenceSettings>(data.SettingsPath);
                var list = settings.AssetPathList;
                var len = list.Count;
                var assembly = typeof(UnityEngine.GameObject).Assembly;
                var i = 0;
                var progressCall2 = Progress.Create<float>(f =>
                {
                    var value = 0.5f + (i + f) / len * 0.5f;
                    Game.Instance.EventSystem.Invoke<E_LoadingViewProgressRefresh2, float>(value);
                });
                for (; i < len; i++)
                {
                    var e = list[i];
                    var path = e.path;
                    await component.CreateLoadHandle(assembly.GetType(e.typeName), path, true).ToUniTask(progressCall2);
                    progressCall2.Report(1);
                }
                component.Unload(data.SettingsPath);
            }

            await UniTask.Delay(500);
            ObjectHelper.CreateComponent<GameRootComponent>(ObjectHelper.CreateEntity(Game.Instance.Scene, GameObject.Find("GameRoot")));
            if (data.Call != null)
            {
                await data.Call();
            }

            ResHelper.GCAndUnload();
            ObjectHelper.CloseUIView<LoadingViewComponent>();
            Game.Instance.EventSystem.Invoke<E_LoadSceneFinish>();
        }
    }
}