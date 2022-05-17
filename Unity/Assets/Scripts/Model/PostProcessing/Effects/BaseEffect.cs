using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public abstract class BaseEffect
{
    protected PostProcessVolume ppv;
    protected bool isInit = false;

    /// <summary>
    /// 必须进行初始化
    /// </summary>
    /// <param name="ppv"></param>
    public BaseEffect(PostProcessVolume ppv)
    {
        this.ppv = ppv;
    }

    /// <summary>
    /// 初始化settings和表现效果的值
    /// </summary>
    protected abstract void InitValue();
    /// <summary>
    /// 抓取默认效果的值
    /// </summary>
    protected abstract void GrabInitialValues();
    /// <summary>
    /// 显示效果
    /// </summary>
    public abstract void ShowEffect();
    /// <summary>
    /// 恢复效果
    /// </summary>
    public abstract void RecoverEffect();
    /// <summary>
    /// 显示效果
    /// </summary>
    public abstract void ShowLerpEffect(float t);
    /// <summary>
    /// 过程恢复效果
    /// </summary>
    public abstract void RecoverLerpEffect(float t);

    /// <summary>
    /// 获取Settings
    /// </summary>
    /// <typeparam name="T">PostProcessEffectSettings类型的Settings</typeparam>
    /// <returns></returns>
    protected T GetSetting<T>() where T : PostProcessEffectSettings
    {
        if (!HasSettings<T>())
        {
            ppv.profile.AddSettings<T>();
        }
        var settings = ppv.profile.GetSetting<T>();
        return settings;
    }

    /// <summary>
    /// 是否拥有Settings
    /// </summary>
    /// <typeparam name="T">PostProcessEffectSettings类型的Settings</typeparam>
    /// <returns></returns>
    protected bool HasSettings<T>() where T : PostProcessEffectSettings
    {
        return ppv.profile.HasSettings<T>();
    }
}
