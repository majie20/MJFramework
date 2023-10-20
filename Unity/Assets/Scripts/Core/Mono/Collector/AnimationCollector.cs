using Sirenix.OdinInspector;
using Slate;
using System;
using UnityEngine;

namespace Model
{
    [Serializable]
    public struct AnimationData
    {
        public AnimationClip Clip;
        public Cutscene      Cutscene;
        public bool          IsTemporary;
    }

    public class AnimationCollector : MonoBehaviour
    {
        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] IdleClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] AttackClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] DieClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] WalkClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] RunClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] SkillClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] HurtClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] JumpClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] ClimbClips;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AnimationData[] OtherClips;

        private bool _isInitialize = false;

        public void Initialize()
        {
            if (_isInitialize)
            {
                return;
            }

            _isInitialize = true;

            for (int i = IdleClips.Length - 1; i >= 0; i--)
            {
                IdleClips[i].Cutscene.PreInitialize();
            }

            for (int i = AttackClips.Length - 1; i >= 0; i--)
            {
                AttackClips[i].Cutscene.PreInitialize();
            }

            for (int i = DieClips.Length - 1; i >= 0; i--)
            {
                DieClips[i].Cutscene.PreInitialize();
            }

            for (int i = WalkClips.Length - 1; i >= 0; i--)
            {
                WalkClips[i].Cutscene.PreInitialize();
            }

            for (int i = RunClips.Length - 1; i >= 0; i--)
            {
                RunClips[i].Cutscene.PreInitialize();
            }

            for (int i = SkillClips.Length - 1; i >= 0; i--)
            {
                SkillClips[i].Cutscene.PreInitialize();
            }

            for (int i = HurtClips.Length - 1; i >= 0; i--)
            {
                HurtClips[i].Cutscene.PreInitialize();
            }

            for (int i = JumpClips.Length - 1; i >= 0; i--)
            {
                JumpClips[i].Cutscene.PreInitialize();
            }

            for (int i = ClimbClips.Length - 1; i >= 0; i--)
            {
                ClimbClips[i].Cutscene.PreInitialize();
            }

            for (int i = OtherClips.Length - 1; i >= 0; i--)
            {
                OtherClips[i].Cutscene.PreInitialize();
            }
        }
    }
}