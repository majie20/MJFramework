using UnityEngine;

namespace Model
{
    public class Init : MonoBehaviour
    {
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            Game.Instance.Init();
        }

        private async void OnEnable()
        {
            Game.Instance.EventSystem.AddListener2(EventType.GameLoadComplete, 0, OnGameLoadComplete);

            StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>()
                .LoadAssetBundleManifestByUWR($"{Application.streamingAssetsPath}/AssetBundleRes/AssetBundleRes"));

            //netsh http add urlacl url = http://192.168.31.141:8082/ user=everyone
            //netsh http add urlacl url = http://+:8082/ user=everyone
            //netsh http show urlacl
            //netsh http delete urlacl url = http://+:8082/
            //netsh http delete urlacl url = http://192.168.31.141:8082/

            //var ips = HttpHelper.GetAddressIPs();

            //for (int i = 0; i < ips.Length; i++)
            //{
            //    Debug.Log(ips[i]); // MDEBUG:
            //}


            //Proto.Net

            //正在安装:

            //System.Buffers.4.5.1
            //System.Numerics.Vectors.4.5.0
            //System.Runtime.CompilerServices.Unsafe.4.5.3
            //System.Memory.4.5.4
            //System.Collections.Immutable.1.7.1
            //protobuf - net.Core.3.0.101
            //protobuf - net.3.0.101

            //Proto.Google

            //正在安装:

            //System.Buffers.4.4.0
            //System.Numerics.Vectors.4.4.0
            //System.Runtime.CompilerServices.Unsafe.4.5.2
            //System.Memory.4.5.3
            //Google.Protobuf.3.19.1




        }

        private void OnDisable()
        {
            //Game.Instance.EventSystem.RemoveListener2(EventType.GameLoadComplete, OnGameLoadComplete);
        }

        private void Update()
        {
            Game.Instance.LifecycleSystem.Update(Time.deltaTime);
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameUpdate(Time.deltaTime);
            }

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    Game.Instance.Hotfix.GotoHotfix();
            //}
        }

        private void LateUpdate()
        {
            Game.Instance.LifecycleSystem.LateUpdate();
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameLateUpdate();
            }
        }

        private void OnApplicationQuit()
        {
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameApplicationQuit();
            }
            Game.Instance.Dispose();
        }

        private void OnGameLoadComplete(object[] args)
        {
            Game.Instance.EventSystem.RemoveListener2(EventType.GameLoadComplete, OnGameLoadComplete);
            Debug.Log($"------完成------{args.Length}");
            Game.Instance.Hotfix.LoadHotfixAssembly();
            Game.Instance.Hotfix.GotoHotfix();
        }
    }
}