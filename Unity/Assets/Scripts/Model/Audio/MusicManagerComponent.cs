using System.Collections;
using System.Collections.Generic;
using System.Data;
using cfg;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Model
{
    /// <summary>
    /// 声音总控制器
    /// </summary>
    public class MusicManagerComponent : Component, IAwake
    {
        /// <summary>
        /// 所有音效
        /// </summary>
        private Dictionary<int, EventInstance> allSounds;

        /// <summary>
        /// 背景音乐
        /// </summary>
        private EventInstance Music;

        /// <summary>
        /// 音效声音大小
        /// </summary>
        private float soundValue;

        /// <summary>
        /// 背景音乐声音大小
        /// </summary>
        private float musicValue;

        private int _SoundID;

        /// <summary>
        /// 声音ID
        /// </summary>
        int SoundID
        {
            get
            {
                _SoundID++;
                return _SoundID;
            }
        }

        audioData audioData => Game.Instance.Scene.GetComponent<GameConfigDataComponent>().JsonTables.audioData;
        AssetsComponent component => Game.Instance.Scene.GetComponent<AssetsComponent>();
        public void Awake()
        {
            musicValue = 1;
            soundValue = 1;
            allSounds = new Dictionary<int, EventInstance>();
            Game.Instance.EventSystem.AddListener<E_SetSoundValue, float>(this, SetSoundValue);
            Game.Instance.EventSystem.AddListener<E_SetMusicValue, float>(this, SetMusicValue);
            Game.Instance.EventSystem.AddListener<E_StopSound, int>(this, StopSound);
            Game.Instance.EventSystem.AddListener<E_StopMusic>(this, StopMusic);
            Game.Instance.EventSystem.AddListenerAsync<E_PlayMusic, int>(this, PlayMusic);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="eventPath">声音地址</param>
        /// <returns></returns>
        public int PlaySound(int eventPath)
        {
            int id = SoundID;
            if (audioData.DataMap.ContainsKey(eventPath))
            {
                if (Settings.Instance.ImportType == ImportType.StreamingAssets)
                {
                    var instance = CreateInstance(audioData.DataMap[eventPath].Streaning);
                    instance.start();
                    instance.setVolume(soundValue);
                    allSounds.Add(id, instance);

                }
                else
                {
                    var t = component.LoadAsync<TextAsset>(audioData.DataMap[eventPath].AssetBuild);
                    RuntimeManager.LoadBank(t.GetAwaiter().GetResult());
                }
            }

            return id;
        }

        ///// <summary>
        ///// 触发式音效
        ///// </summary>
        ///// <param name="soundGame">触发物体</param>
        ///// <param name="eventPath">声音路径</param>
        ///// <param name="PlayEvent">触发条件</param>
        ///// <param name="StopEvent">声音结束条件</param>
        //public void PlaySound(GameObject soundGame, string eventPath, EmitterGameEvent PlayEvent,
        //    EmitterGameEvent StopEvent)
        //{
        //    StudioEventEmitter studioEvent = soundGame.GetComponent<StudioEventEmitter>();
        //    if (studioEvent == null)
        //    {
        //        studioEvent = soundGame.AddComponent<FMODUnity.StudioEventEmitter>();
        //    }

        //    studioEvent.EventReference.Path = eventPath;
        //    studioEvent.EventReference.Guid = RuntimeManager.PathToGUID(eventPath);
        //    studioEvent.PlayEvent = PlayEvent;
        //    studioEvent.StopEvent = StopEvent;
        //    studioEvent.Play();
        //    studioEvent.EventInstance.setVolume(soundValue);
        //}

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="eventPath"></param>
        public async UniTask PlayMusic(int eventPath)
        {
            if (Music.isValid())
            {
                Music.stop(STOP_MODE.IMMEDIATE);
            }
            if (Settings.Instance.ImportType == ImportType.StreamingAssets)
            {
                if (audioData.DataMap.ContainsKey(eventPath))
                {
                    Music = CreateInstance(audioData.DataMap[eventPath].Streaning);
                    Music.start();
                    Music.setVolume(musicValue);
                }
            }
            else
            {
                var t = await component.LoadAsync<TextAsset>(audioData.DataMap[eventPath].AssetBuild);
                RuntimeManager.LoadBank(t);
                Music = CreateInstance(audioData.DataMap[eventPath].Streaning);
                Music.start();
                Music.setVolume(musicValue);
            }
        }

        /// <summary>
        /// 停止声音
        /// </summary>
        /// <param name="id">播放ID</param>
        public void StopSound(int id)
        {
            EventInstance instance;
            allSounds.TryGetValue(id, out instance);
            if (instance.isValid())
            {
                instance.stop(STOP_MODE.IMMEDIATE);
                allSounds.Remove(id);
            }
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopMusic()
        {
            if (Music.isValid())
            {
                Music.stop(STOP_MODE.IMMEDIATE);
            }
        }

        /// <summary>
        /// 设置声音大小
        /// </summary>
        /// <param name="value"></param>
        public void SetSoundValue(float value)
        {
            soundValue = value;
            foreach (var allSound in allSounds)
            {
                EventInstance instance = allSound.Value;
                if (instance.isValid())
                {
                    instance.setVolume(value);
                }
            }
        }

        /// <summary>
        /// 设置背景音乐大小
        /// </summary>
        /// <param name="value"></param>
        public void SetMusicValue(float value)
        {
            musicValue = value;
            if (Music.isValid())
            {
                Music.setVolume(value);
            }
        }

        /// <summary>
        /// 创建声音事件
        /// </summary>
        /// <param name="eventPath"></param>
        /// <returns></returns>
        EventInstance CreateInstance(string eventPath)
        {
            var instance = RuntimeManager.CreateInstance(eventPath);
            return instance;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}