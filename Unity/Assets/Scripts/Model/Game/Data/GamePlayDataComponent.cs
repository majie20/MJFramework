using System;
using System.Collections.Generic;
using System.Reflection;

namespace Model
{
    public class LocalData
    {
        public string Data;
        public string AssemblyName;
        public string TypeFullName;
    }

    public class GamePlayDataComponent : Component, IAwake
    {
        public bool IsShowMainCamera = true;

        private Dictionary<string, object>    _gameDataMap;
        private Dictionary<string, LocalData> _localDataMap;

        public void Awake()
        {
            _gameDataMap = new Dictionary<string, object>();
            _localDataMap = new Dictionary<string, LocalData>();

            Game.Instance.EventSystem.AddListener<E_SetMainCameraShow, bool>(this, OnSetMainCameraShow);
            Game.Instance.EventSystem.AddListener<E_GameQuit>(this, OnGameQuit);
        }

        public override void Dispose()
        {
            _localDataMap = null;
            _gameDataMap = null;
            base.Dispose();
        }

        private void OnSetMainCameraShow(bool b)
        {
            IsShowMainCamera = b;
        }

        private void OnGameQuit()
        {
#if WX&&!UNITY_EDITOR
#elif TT&&!UNITY_EDITOR
#else
            UnityEngine.PlayerPrefs.Save();
#endif
        }

        public void SetGameData(string sign, object a)
        {
            if (_gameDataMap.ContainsKey(sign))
            {
                _gameDataMap[sign] = a;
                _localDataMap[sign].Data = CatJson.JsonParser.Default.ToJson(a);
            }
            else
            {
                _gameDataMap.Add(sign, a);
                var type = a.GetType();

                var data = type == typeof(string) || type.IsPrimitive ? a.ToString() : CatJson.JsonParser.Default.ToJson(a);
                var span = type.Assembly.FullName.AsSpan();
                var len = span.Length;

                for (int j = 0; j < len; j++)
                {
                    if (span[j] == ',')
                    {
                        _localDataMap.Add(sign, new LocalData() {Data = data, TypeFullName = type.FullName, AssemblyName = new string(span[..j])});
                        break;
                    }
                }
            }
#if WX&&!UNITY_EDITOR
            WeChatWASM.WXBase.StorageSetStringSync(sign, CatJson.JsonParser.Default.ToJson(_localDataMap[sign]));
#elif TT&&!UNITY_EDITOR
            StarkSDKSpace.StarkStorage.SetStringSync(sign, CatJson.JsonParser.Default.ToJson(_localDataMap[sign]));
#else
            UnityEngine.PlayerPrefs.SetString(sign, CatJson.JsonParser.Default.ToJson(_localDataMap[sign]));
#endif
        }

        public T GetGameData<T>(string sign, T def = default)
        {
            if (_gameDataMap.TryGetValue(sign, out var value))
            {
                return (T) value;
            }

#if WX&&!UNITY_EDITOR
            if (WeChatWASM.WXBase.StorageHasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(WeChatWASM.WXBase.StorageGetStringSync(sign, ""));
                def = (T) CatJson.JsonParser.Default.ParseJson(data.Data, Assembly.Load(data.AssemblyName).GetType(data.TypeFullName));
                _gameDataMap.Add(sign, def);

                return def;
            }
#elif TT&&!UNITY_EDITOR
            if (StarkSDKSpace.StarkStorage.HasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(StarkSDKSpace.StarkStorage.GetStringSync(sign, ""));
                def = (T) CatJson.JsonParser.Default.ParseJson(data.Data, Assembly.Load(data.AssemblyName).GetType(data.TypeFullName));
                _gameDataMap.Add(sign, def);

                return def;
            }
#else
            if (UnityEngine.PlayerPrefs.HasKey(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(UnityEngine.PlayerPrefs.GetString(sign));
                def = (T) CatJson.JsonParser.Default.ParseJson(data.Data, Assembly.Load(data.AssemblyName).GetType(data.TypeFullName));
                _gameDataMap.Add(sign, def);

                return def;
            }
#endif

            SetGameData(sign, def);

            return def;
        }

        public int GetGameData(string sign, int def = 0)
        {
            if (_gameDataMap.TryGetValue(sign, out var value))
            {
                return (int) value;
            }
#if WX&&!UNITY_EDITOR
            if (WeChatWASM.WXBase.StorageHasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(WeChatWASM.WXBase.StorageGetStringSync(sign, ""));
                def = int.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#elif TT&&!UNITY_EDITOR
           if (StarkSDKSpace.StarkStorage.HasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(StarkSDKSpace.StarkStorage.GetStringSync(sign, ""));
                def = int.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#else
            if (UnityEngine.PlayerPrefs.HasKey(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(UnityEngine.PlayerPrefs.GetString(sign));
                def = int.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#endif

            SetGameData(sign, def);

            return def;
        }

        public float GetGameData(string sign, float def = 0.0f)
        {
            if (_gameDataMap.TryGetValue(sign, out var value))
            {
                return (float) value;
            }
#if WX&&!UNITY_EDITOR
            if (WeChatWASM.WXBase.StorageHasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(WeChatWASM.WXBase.StorageGetStringSync(sign, ""));
                def = float.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#elif TT&&!UNITY_EDITOR
            if (StarkSDKSpace.StarkStorage.HasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(StarkSDKSpace.StarkStorage.GetStringSync(sign, ""));
                def = float.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#else

            if (UnityEngine.PlayerPrefs.HasKey(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(UnityEngine.PlayerPrefs.GetString(sign));
                def = float.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#endif

            SetGameData(sign, def);

            return def;
        }

        public bool GetGameData(string sign, bool def = true)
        {
            if (_gameDataMap.TryGetValue(sign, out var value))
            {
                return (bool) value;
            }
#if WX&&!UNITY_EDITOR
            if (WeChatWASM.WXBase.StorageHasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(WeChatWASM.WXBase.StorageGetStringSync(sign, ""));
                def = bool.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#elif TT&&!UNITY_EDITOR
            if (StarkSDKSpace.StarkStorage.HasKeySync(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(StarkSDKSpace.StarkStorage.GetStringSync(sign, ""));
                def = bool.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#else
            if (UnityEngine.PlayerPrefs.HasKey(sign))
            {
                var data = CatJson.JsonParser.Default.ParseJson<LocalData>(UnityEngine.PlayerPrefs.GetString(sign));
                def = bool.Parse(data.Data);
                _gameDataMap.Add(sign, def);

                return def;
            }
#endif

            SetGameData(sign, def);

            return def;
        }
    }
}