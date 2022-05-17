using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Model
{
    public class ProfileInfo
    {
        public bool isinit = true;
        public PostProcessProfile originalEffect;
        public Dictionary<string, PostProcessProfile> profilesDic;

        public ProfileInfo()
        {
            isinit = true;
            originalEffect = ScriptableObject.CreateInstance<PostProcessProfile>();
            profilesDic = new Dictionary<string, PostProcessProfile>();
        }
    }

    [LifeCycle]
    public class PostProcessAssetComponent : Component,IAwake,IDisposable
    {
        public PostProcessVolume defaultPPV;
        private PostProcessProfile MainEffect;
        private Dictionary<PostProcessVolume, ProfileInfo> profileDic;

        public void Awake()
        {
            defaultPPV = GameObject.Find("Main Camera").GetComponent<PostProcessVolume>();
            MainEffect = defaultPPV.profile;

            profileDic = new Dictionary<PostProcessVolume, ProfileInfo>();
        }
        public override void Dispose()
        {
            defaultPPV = null;
            MainEffect = null;
            profileDic = null;
            Entity = null;
        }

        /// <summary>
        /// 展示效果
        /// </summary>
        public void ShowEffect(string name,PostProcessVolume volume=null)
        {
            var vProfile = volume == null ? MainEffect : volume.profile;

            var v = volume == null ? defaultPPV : volume;
            var profileInfo = GetProfileInfo(v);

            if (profileInfo.isinit)
            {
                SaveEffect();
            }

            var profile= GetProfileInfoProfile(profileInfo,name);
            Replace(profile, vProfile);
        }

        /// <summary>
        /// 保存当前的效果
        /// </summary>
        public void SaveEffect(PostProcessVolume volume = null)
        {
            var vProfile = volume == null ? MainEffect : volume.profile;

            var v = volume == null ? defaultPPV : volume;
            var profileInfo = GetProfileInfo(v);

            profileInfo.isinit = false;
            Replace(vProfile, profileInfo.originalEffect, true);
        }

        /// <summary>
        /// 恢复效果
        /// </summary>
        public void RecoverEffect(PostProcessVolume volume = null)
        {
            var vProfile = volume == null ? MainEffect : volume.profile;

            var v = volume == null ? defaultPPV : volume;
            var profileInfo = GetProfileInfo(v);

            Replace(profileInfo.originalEffect, vProfile, true);
        }

        /// <summary>
        /// source赋值信息到target
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标</param>
        /// <param name="isAll">是否完全对应source,默认target不会移除target比source多的setting</param>
        private void Replace(PostProcessProfile source, PostProcessProfile target, bool isAll = false)
        {
            if (isAll)
            {
                RemoveNeedlessSetting(source,target);
            }

            var list = source.settings;
            foreach (var effect in list)
            {
                Type type = effect.GetType();
                var settings = type.Name;
                var fieldInfos = type.GetFields();
                foreach (var fieldInfo in fieldInfos)
                {
                    var field = fieldInfo.Name;
                    SetSettings(source, target, settings, field);
                }
            }
        }

        /// <summary>
        /// 区分Settings的Type进行不同的设置
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标</param>
        /// <param name="settings">settings的Type名称</param>
        /// <param name="field">field的FieldInfo名称</param>
        private void SetSettings(PostProcessProfile source, PostProcessProfile target, string settings, string field)
        {
            switch (settings)
            {
                case "AmbientOcclusion":
                    if (!target.HasSettings<AmbientOcclusion>())
                    {
                        target.AddSettings<AmbientOcclusion>();
                    }
                    SetAmbientOcclusion(source.GetSetting<AmbientOcclusion>(), target.GetSetting<AmbientOcclusion>(), field);
                    break;
                case "AutoExposure":
                    if (!target.HasSettings<AutoExposure>())
                    {
                        target.AddSettings<AutoExposure>();
                    }
                    SetAutoExposure(source.GetSetting<AutoExposure>(), target.GetSetting<AutoExposure>(), field);
                    break;
                case "Bloom":
                    if (!target.HasSettings<Bloom>())
                    {
                        target.AddSettings<Bloom>();
                    }
                    SetBloom(source.GetSetting<Bloom>(), target.GetSetting<Bloom>(), field);
                    break;
                case "ChromaticAberration":
                    if (!target.HasSettings<ChromaticAberration>())
                    {
                        target.AddSettings<ChromaticAberration>();
                    }
                    SetChromaticAberration(source.GetSetting<ChromaticAberration>(), target.GetSetting<ChromaticAberration>(), field);
                    break;
                case "ColorGrading":
                    if (!target.HasSettings<ColorGrading>())
                    {
                        target.AddSettings<ColorGrading>();
                    }
                    SetColorGrading(source.GetSetting<ColorGrading>(), target.GetSetting<ColorGrading>(), field);
                    break;
                case "LensDistortion":
                    if (!target.HasSettings<LensDistortion>())
                    {
                        target.AddSettings<LensDistortion>();
                    }
                    SetLensDistortion(source.GetSetting<LensDistortion>(), target.GetSetting<LensDistortion>(), field);
                    break;
            }
        }

        #region 所有Settings的设置
        private void SetAmbientOcclusion(AmbientOcclusion source, AmbientOcclusion target, string field)
        {
            switch (field)
            {
                case "mode":
                    target.mode.value = source.mode.value;
                    target.mode.overrideState = source.mode.overrideState;
                    break;
                case "intensity":
                    target.intensity.value = source.intensity.value;
                    target.intensity.overrideState = source.intensity.overrideState;
                    break;
                case "thicknessModifier":
                    target.thicknessModifier.value = source.thicknessModifier.value;
                    target.thicknessModifier.overrideState = source.thicknessModifier.overrideState;
                    break;
                case "color":
                    target.color.value = source.color.value;
                    target.color.overrideState = source.color.overrideState;
                    break;
                case "ambientOnly":
                    target.ambientOnly.value = source.ambientOnly.value;
                    target.ambientOnly.overrideState = source.ambientOnly.overrideState;
                    break;
            }
        }
        private void SetAutoExposure(AutoExposure source, AutoExposure target, string field)
        {
            switch (field)
            {
                case "filtering":
                    target.filtering.value = source.filtering.value;
                    target.filtering.overrideState = source.filtering.overrideState;
                    break;
                case "minLuminance":
                    target.minLuminance.value = source.minLuminance.value;
                    target.minLuminance.overrideState = source.minLuminance.overrideState;
                    break;
                case "maxLuminance":
                    target.maxLuminance.value = source.maxLuminance.value;
                    target.maxLuminance.overrideState = source.maxLuminance.overrideState;
                    break;
                case "keyValue":
                    target.keyValue.value = source.keyValue.value;
                    target.keyValue.overrideState = source.keyValue.overrideState;
                    break;
                case "eyeAdaptation":
                    target.eyeAdaptation.value = source.eyeAdaptation.value;
                    target.eyeAdaptation.overrideState = source.eyeAdaptation.overrideState;
                    break;
                case "speedUp":
                    target.speedUp.value = source.speedUp.value;
                    target.speedUp.overrideState = source.speedUp.overrideState;
                    break;
                case "speedDown":
                    target.speedDown.value = source.speedDown.value;
                    target.speedDown.overrideState = source.speedDown.overrideState;
                    break;
            }
        }
        private void SetBloom(Bloom source, Bloom target, string field)
        {
            switch (field)
            {
                case "intensity":
                    target.intensity.value = source.intensity.value;
                    target.intensity.overrideState = source.intensity.overrideState;
                    break;
                case "threshold":
                    target.threshold.value = source.threshold.value;
                    target.threshold.overrideState = source.threshold.overrideState;
                    break;
                case "softKnee":
                    target.softKnee.value = source.softKnee.value;
                    target.softKnee.overrideState = source.softKnee.overrideState;
                    break;
                case "clamp":
                    target.clamp.value = source.clamp.value;
                    target.clamp.overrideState = source.clamp.overrideState;
                    break;
                case "diffusion":
                    target.diffusion.value = source.diffusion.value;
                    target.diffusion.overrideState = source.diffusion.overrideState;
                    break;
                case "anamorphicRatio":
                    target.anamorphicRatio.value = source.anamorphicRatio.value;
                    target.anamorphicRatio.overrideState = source.anamorphicRatio.overrideState;
                    break;
                case "color":
                    target.color.value = source.color.value;
                    target.color.overrideState = source.color.overrideState;
                    break;
                case "fastMode":
                    target.fastMode.value = source.fastMode.value;
                    target.fastMode.overrideState = source.fastMode.overrideState;
                    break;
            }
        }
        private void SetChromaticAberration(ChromaticAberration source, ChromaticAberration target, string field)
        {
            switch (field)
            {
                case "intensity":
                    target.intensity.value = source.intensity.value;
                    target.intensity.overrideState = source.intensity.overrideState;
                    break;
                case "spectralLut":
                    target.spectralLut.value = source.spectralLut.value;
                    target.spectralLut.overrideState = source.spectralLut.overrideState;
                    break;
                case "fastMode":
                    target.fastMode.value = source.fastMode.value;
                    target.fastMode.overrideState = source.fastMode.overrideState;
                    break;
            }
        }
        private void SetColorGrading(ColorGrading source, ColorGrading target, string field)
        {
            switch (field)
            {
                case "gradingMode":
                    target.gradingMode.value = source.gradingMode.value;
                    target.gradingMode.overrideState = source.gradingMode.overrideState;
                    break;
                case "ldrLut":
                    target.ldrLut.value = source.ldrLut.value;
                    target.ldrLut.overrideState = source.ldrLut.overrideState;
                    break;
                case "ldrLutContribution":
                    target.ldrLutContribution.value = source.ldrLutContribution.value;
                    target.ldrLutContribution.overrideState = source.ldrLutContribution.overrideState;
                    break;
                case "mixerGreenOutGreenIn":
                    target.mixerGreenOutGreenIn.value = source.mixerGreenOutGreenIn.value;
                    target.mixerGreenOutGreenIn.overrideState = source.mixerGreenOutGreenIn.overrideState;
                    break;
                case "mixerGreenOutBlueIn":
                    target.mixerGreenOutBlueIn.value = source.mixerGreenOutBlueIn.value;
                    target.mixerGreenOutBlueIn.overrideState = source.mixerGreenOutBlueIn.overrideState;
                    break;
                case "mixerBlueOutRedIn":
                    target.mixerBlueOutRedIn.value = source.mixerBlueOutRedIn.value;
                    target.mixerBlueOutRedIn.overrideState = source.mixerBlueOutRedIn.overrideState;
                    break;
                case "mixerBlueOutGreenIn":
                    target.mixerBlueOutGreenIn.value = source.mixerBlueOutGreenIn.value;
                    target.mixerBlueOutGreenIn.overrideState = source.mixerBlueOutGreenIn.overrideState;
                    break;
                case "mixerBlueOutBlueIn":
                    target.mixerBlueOutBlueIn.value = source.mixerBlueOutBlueIn.value;
                    target.mixerBlueOutBlueIn.overrideState = source.mixerBlueOutBlueIn.overrideState;
                    break;
                case "lift":
                    target.lift.value = source.lift.value;
                    target.lift.overrideState = source.lift.overrideState;
                    break;
                case "gamma":
                    target.gamma.value = source.gamma.value;
                    target.gamma.overrideState = source.gamma.overrideState;
                    break;
                case "mixerGreenOutRedIn":
                    target.mixerGreenOutRedIn.value = source.mixerGreenOutRedIn.value;
                    target.mixerGreenOutRedIn.overrideState = source.mixerGreenOutRedIn.overrideState;
                    break;
                case "gain":
                    target.gain.value = source.gain.value;
                    target.gain.overrideState = source.gain.overrideState;
                    break;
                case "redCurve":
                    target.redCurve.value = source.redCurve.value;
                    target.redCurve.overrideState = source.redCurve.overrideState;
                    break;
                case "greenCurve":
                    target.greenCurve.value = source.greenCurve.value;
                    target.greenCurve.overrideState = source.greenCurve.overrideState;
                    break;
                case "blueCurve":
                    target.blueCurve.value = source.blueCurve.value;
                    target.blueCurve.overrideState = source.blueCurve.overrideState;
                    break;
                case "hueVsHueCurve":
                    target.hueVsHueCurve.value = source.hueVsHueCurve.value;
                    target.hueVsHueCurve.overrideState = source.hueVsHueCurve.overrideState;
                    break;
                case "hueVsSatCurve":
                    target.hueVsSatCurve.value = source.hueVsSatCurve.value;
                    target.hueVsSatCurve.overrideState = source.hueVsSatCurve.overrideState;
                    break;
                case "satVsSatCurve":
                    target.satVsSatCurve.value = source.satVsSatCurve.value;
                    target.satVsSatCurve.overrideState = source.satVsSatCurve.overrideState;
                    break;
                case "lumVsSatCurve":
                    target.lumVsSatCurve.value = source.lumVsSatCurve.value;
                    target.lumVsSatCurve.overrideState = source.lumVsSatCurve.overrideState;
                    break;
                case "masterCurve":
                    target.masterCurve.value = source.masterCurve.value;
                    target.masterCurve.overrideState = source.masterCurve.overrideState;
                    break;
                case "mixerRedOutBlueIn":
                    target.mixerRedOutBlueIn.value = source.mixerRedOutBlueIn.value;
                    target.mixerRedOutBlueIn.overrideState = source.mixerRedOutBlueIn.overrideState;
                    break;
                case "mixerRedOutGreenIn":
                    target.mixerRedOutGreenIn.value = source.mixerRedOutGreenIn.value;
                    target.mixerRedOutGreenIn.overrideState = source.mixerRedOutGreenIn.overrideState;
                    break;
                case "toneCurveToeStrength":
                    target.toneCurveToeStrength.value = source.toneCurveToeStrength.value;
                    target.toneCurveToeStrength.overrideState = source.toneCurveToeStrength.overrideState;
                    break;
                case "toneCurveToeLength":
                    target.toneCurveToeLength.value = source.toneCurveToeLength.value;
                    target.toneCurveToeLength.overrideState = source.toneCurveToeLength.overrideState;
                    break;
                case "toneCurveShoulderStrength":
                    target.toneCurveShoulderStrength.value = source.toneCurveShoulderStrength.value;
                    target.toneCurveShoulderStrength.overrideState = source.toneCurveShoulderStrength.overrideState;
                    break;
                case "toneCurveShoulderLength":
                    target.toneCurveShoulderLength.value = source.toneCurveShoulderLength.value;
                    target.toneCurveShoulderLength.overrideState = source.toneCurveShoulderLength.overrideState;
                    break;
                case "toneCurveShoulderAngle":
                    target.toneCurveShoulderAngle.value = source.toneCurveShoulderAngle.value;
                    target.toneCurveShoulderAngle.overrideState = source.toneCurveShoulderAngle.overrideState;
                    break;
                case "toneCurveGamma":
                    target.toneCurveGamma.value = source.toneCurveGamma.value;
                    target.toneCurveGamma.overrideState = source.toneCurveGamma.overrideState;
                    break;
                case "mixerRedOutRedIn":
                    target.mixerRedOutRedIn.value = source.mixerRedOutRedIn.value;
                    target.mixerRedOutRedIn.overrideState = source.mixerRedOutRedIn.overrideState;
                    break;
                case "tonemapper":
                    target.tonemapper.value = source.tonemapper.value;
                    target.tonemapper.overrideState = source.tonemapper.overrideState;
                    break;
                case "tint":
                    target.tint.value = source.tint.value;
                    target.tint.overrideState = source.tint.overrideState;
                    break;
                case "colorFilter":
                    target.colorFilter.value = source.colorFilter.value;
                    target.colorFilter.overrideState = source.colorFilter.overrideState;
                    break;
                case "hueShift":
                    target.hueShift.value = source.hueShift.value;
                    target.hueShift.overrideState = source.hueShift.overrideState;
                    break;
                case "saturation":
                    target.saturation.value = source.saturation.value;
                    target.saturation.overrideState = source.saturation.overrideState;
                    break;
                case "brightness":
                    target.brightness.value = source.brightness.value;
                    target.brightness.overrideState = source.brightness.overrideState;
                    break;
                case "postExposure":
                    target.postExposure.value = source.postExposure.value;
                    target.postExposure.overrideState = source.postExposure.overrideState;
                    break;
                case "contrast":
                    target.contrast.value = source.contrast.value;
                    target.contrast.overrideState = source.contrast.overrideState;
                    break;
                case "temperature":
                    target.temperature.value = source.temperature.value;
                    target.temperature.overrideState = source.temperature.overrideState;
                    break;
                case "externalLut":
                    target.externalLut.value = source.externalLut.value;
                    target.externalLut.overrideState = source.externalLut.overrideState;
                    break;
            }
        }
        private void SetLensDistortion(LensDistortion source, LensDistortion target, string field)
        {
            switch (field)
            {
                case "intensity":
                    target.intensity.value = source.intensity.value;
                    target.intensity.overrideState = source.intensity.overrideState;
                    break;
                case "intensityX":
                    target.intensityX.value = source.intensityX.value;
                    target.intensityX.overrideState = source.intensityX.overrideState;
                    break;
                case "intensityY":
                    target.intensityY.value = source.intensityY.value;
                    target.intensityY.overrideState = source.intensityY.overrideState;
                    break;
                case "centerX":
                    target.centerX.value = source.centerX.value;
                    target.centerX.overrideState = source.centerX.overrideState;
                    break;
                case "centerY":
                    target.centerY.value = source.centerY.value;
                    target.centerY.overrideState = source.centerY.overrideState;
                    break;
                case "scale":
                    target.scale.value = source.scale.value;
                    target.scale.overrideState = source.scale.overrideState;
                    break;
            }
        }
        #endregion

        #region 第二种方法：直接替换settings的内容
        /// <summary>
        /// 展示效果
        /// </summary>
        public void DirectShowEffect(string name, PostProcessVolume volume = null)
        {
            var vProfile = volume == null ? MainEffect : volume.profile;

            var v = volume == null ? defaultPPV : volume;
            var profileInfo = GetProfileInfo(v);

            if (profileInfo.isinit)
            {
                DirectSaveEffect();
            }

            var profile = GetProfileInfoProfile(profileInfo, name);
            DirectReplace(profile, vProfile);
        }

        /// <summary>
        /// 保存当前的效果
        /// </summary>
        public void DirectSaveEffect(PostProcessVolume volume = null)
        {
            var vProfile = volume == null ? MainEffect : volume.profile;

            var v = volume == null ? defaultPPV : volume;
            var profileInfo = GetProfileInfo(v);

            profileInfo.isinit = false;
            DirectReplace(vProfile, profileInfo.originalEffect, true);
        }

        /// <summary>
        /// 恢复效果
        /// </summary>
        public void DirectRecoverEffect(PostProcessVolume volume = null)
        {
            var vProfile = volume == null ? MainEffect : volume.profile;

            var v = volume == null ? defaultPPV : volume;
            var profileInfo = GetProfileInfo(v);

            DirectReplace(profileInfo.originalEffect, vProfile, true);
        }

        /// <summary>
        /// 自定义的
        /// </summary>
        /// <param name="source"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        private bool HasSettingDirect(PostProcessProfile source, PostProcessEffectSettings setting)
        {
            source.HasSettings(setting.GetType());
            var s = source.settings;
            foreach (var temp in s)
            {
                if (temp == setting)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 第二种方法：直接替换settings的内容
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标</param>
        private void DirectReplace(PostProcessProfile source, PostProcessProfile target, bool isAll = false)
        {
            if (isAll)
            {
                RemoveNeedlessSetting(source, target);
            }

            var settings = source.settings;
            foreach (var item in settings)
            {
                var t = target.settings;
                PostProcessEffectSettings temporary = null;
                foreach (var temp in t)
                {
                    if (temp.GetType() == item.GetType())
                    {
                        temporary = temp;
                    }
                }
                target.settings.Remove(temporary);

                target.settings.Add(item);
            }
        }
        #endregion

        /// <summary>
        /// 第三种方法：用反射进行source和target信息的传递（问题）
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标</param>
        private void ReplaceRef(PostProcessProfile source, PostProcessProfile target)
        {
            var list = source.settings;
            foreach (var effect in list)
            {
                Type type = effect.GetType();
                PostProcessEffectSettings e;
                if (target.HasSettings(type))
                {
                    target.RemoveSettings(type);
                }
                e = target.AddSettings(type);
                var fieldInfos = type.GetFields();
                foreach (var fieldInfo in fieldInfos)
                {
                    if (fieldInfo.Name == "intensity")
                    {
                        var s = fieldInfo.FieldType;
                        var sinfo = s.GetField("value");
                        var sinfovalue = sinfo.GetValue(effect);
                        sinfo = s.GetField("overrideState");
                        var sinfooverrideState = sinfo.GetValue(effect);

                        var t = e.GetType();
                        var tinfo = t.GetField(fieldInfo.Name);
                        var tinfotype = tinfo.FieldType;
                        var tinfovalue = tinfotype.GetField("value");
                        var tinfooverrideState = tinfotype.GetField("overrideState");

                        tinfovalue.SetValue(e, sinfovalue);
                        tinfooverrideState.SetValue(e, sinfooverrideState);
                    }
                }
            }
        }

        /// <summary>
        /// 获取ProfileInfo中的profile
        /// </summary>
        /// <param name="name">文件名唯一</param>
        /// <returns></returns>
        private PostProcessProfile GetProfileInfoProfile(ProfileInfo info,string name)
        {
            PostProcessProfile profile;
            var dic = info.profilesDic;
            if (!dic.TryGetValue(name, out profile))
            {
                profile = Game.Instance.Scene.GetComponent<AssetsComponent>().LoadSync<PostProcessProfile>(name);
            }
            return profile;
        }

        private ProfileInfo GetProfileInfo(PostProcessVolume volume)
        {
            ProfileInfo profileInfo;
            if (!profileDic.TryGetValue(volume, out profileInfo))
            {
                profileInfo = new ProfileInfo();
            }
            return profileInfo;
        }

        /// <summary>
        /// 删除多余的Settings，target上有的source上没有的是多余的
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void RemoveNeedlessSetting(PostProcessProfile source, PostProcessProfile target)
        {
            var t = target.settings;
            var removes = new List<Type>();
            foreach (var item in t)
            {
                var type = item.GetType();
                var has = source.HasSettings(type);
                if (!has)
                {
                    removes.Add(type);
                }
            }
            foreach (var effect in removes)
            {
                target.RemoveSettings(effect);
            }
            removes.Clear();
        }
    }
}
