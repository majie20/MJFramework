using Cysharp.Threading.Tasks;
using JSAM;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class AudioComponent : Component, IAwake
    {
        public void Awake()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #region Sound

        public async UniTask<SoundChannelHelper> PlaySound(int sign, Transform transform = null, SoundChannelHelper helper = null)
        {
            SoundFileObject sound = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");

            return AudioManager.PlaySound(sound, transform, helper);
        }

        public async UniTask<SoundChannelHelper> PlaySound(int sign, Vector3 position, SoundChannelHelper helper = null)
        {
            SoundFileObject sound = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");

            return AudioManager.PlaySound(sound, position, helper);
        }

        public void StopSound(int sign, Transform transform = null, bool stopInstantly = true)
        {
            SoundFileObject sound = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");
            AudioManager.StopSound(sound, transform, stopInstantly);
        }

        public void StopSound(int sign, Vector3 position, bool stopInstantly = true)
        {
            SoundFileObject sound = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");
            AudioManager.StopSound(sound, position, stopInstantly);
        }

        public bool StopSoundIfPlaying(int sign, Transform transform = null, bool stopInstantly = true)
        {
            SoundFileObject sound = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");

            return AudioManager.StopSoundIfPlaying(sound, transform, stopInstantly);
        }

        public bool StopSoundIfPlaying(int sign, Vector3 position, bool stopInstantly = true)
        {
            SoundFileObject sound = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");

            return AudioManager.StopSoundIfPlaying(sound, position, stopInstantly);
        }

        public void StopAllSounds(bool stopInstantly = true)
        {
            AudioManager.StopAllSounds(stopInstantly);
        }

        public bool IsSoundPlaying(int sign, Transform transform = null)
        {
            SoundFileObject sound = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");

            return AudioManager.IsSoundPlaying(sound, transform);
        }

        public bool IsSoundPlaying(int sign, Vector3 position)
        {
            SoundFileObject sound = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");

            return AudioManager.IsSoundPlaying(sound, position);
        }

        public bool TryGetPlayingSound(int sign, out SoundChannelHelper helper)
        {
            SoundFileObject sound = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<SoundFileObject>($"{ConstData.SOUND_PATH}{sign}");

            return AudioManager.TryGetPlayingSound(sound, out helper);
        }

        #endregion

        #region Music

        public async UniTask<MusicChannelHelper> PlayMusic(int sign, bool isMainMusic)
        {
            MusicFileObject music = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.PlayMusic(music, isMainMusic);
        }

        public async UniTask<MusicChannelHelper> PlayMusic(int sign, Transform transform = null, MusicChannelHelper helper = null)
        {
            MusicFileObject music = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.PlayMusic(music, transform, helper);
        }

        public async UniTask<MusicChannelHelper> PlayMusic(int sign, Vector3 position, MusicChannelHelper helper = null)
        {
            MusicFileObject music = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.PlayMusic(music, position, helper);
        }

        public async UniTask<MusicChannelHelper> FadeMusicIn(int sign, float fadeInTime, bool isMainmusic = false)
        {
            MusicFileObject music = await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadAsync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.FadeMusicIn(music, fadeInTime, isMainmusic);
        }

        public MusicChannelHelper FadeMainMusicOut(float fadeOutTime)
        {
            return AudioManager.FadeMainMusicOut(fadeOutTime);
        }

        public MusicChannelHelper FadeMusicOut(MusicChannelHelper helper, float fadeOutTime)
        {
            return AudioManager.FadeMusicOut(helper, fadeOutTime);
        }

        public bool IsMusicPlaying(int sign)
        {
            MusicFileObject music = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.IsMusicPlaying(music);
        }

        public bool TryGetPlayingMusic(int sign, out MusicChannelHelper helper)
        {
            MusicFileObject music = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.TryGetPlayingMusic(music, out helper);
        }

        public MusicChannelHelper StopMusic(int sign, Transform transform = null, bool stopInstantly = true)
        {
            MusicFileObject music = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.StopMusic(music, transform, stopInstantly);
        }

        public MusicChannelHelper StopMusic(int sign, Vector3 position, bool stopInstantly = true)
        {
            MusicFileObject music = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.StopMusic(music, position, stopInstantly);
        }

        public bool StopMusicIfPlaying(int sign, Transform transform = null, bool stopInstantly = true)
        {
            MusicFileObject music = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.StopMusicIfPlaying(music, transform, stopInstantly);
        }

        public bool StopMusicIfPlaying(int sign, Vector3 position, bool stopInstantly = true)
        {
            MusicFileObject music = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<MusicFileObject>($"{ConstData.MUSIC_PATH}{sign}");

            return AudioManager.StopMusicIfPlaying(music, position, stopInstantly);
        }

        #endregion

        #region 音量

        public float MasterVolume
        {
            get => AudioManager.MasterVolume;
            set => AudioManager.MasterVolume = value;
        }

        public bool MasterMuted
        {
            get => AudioManager.MasterMuted;
            set => AudioManager.MasterMuted = value;
        }

        public float MusicVolume
        {
            get => AudioManager.MusicVolume;
            set => AudioManager.MusicVolume = value;
        }

        public bool MusicMuted
        {
            get => AudioManager.MusicMuted;
            set => AudioManager.MusicMuted = value;
        }

        public float SoundVolume
        {
            get => AudioManager.SoundVolume;
            set => AudioManager.SoundVolume = value;
        }

        public bool SoundMuted
        {
            get => AudioManager.SoundMuted;
            set => AudioManager.SoundMuted = value;
        }

        #endregion

        public void DestroyAudio(int sign)
        {
            if (sign < 200000)
            {
                Game.Instance.Scene.GetComponent<AssetsComponent>().Unload($"{ConstData.MUSIC_PATH}{sign}");
            }
            else
            {
                Game.Instance.Scene.GetComponent<AssetsComponent>().Unload($"{ConstData.SOUND_PATH}{sign}");
            }
        }
    }
}