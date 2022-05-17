using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FireEffect : BaseEffect
{
    private Bloom bloom;
    private ChromaticAberration ca;

    private float initBloomIntensity;
    private float initCaIntensity;

    //private float bloomIntensity = 20.0f;
    //[Range(0.0f, 1.0f)]
    //private float caIntensity = 0.88f;
    public int bloomIntensity { get; set; } = 100;
    public float caIntensity { get; set; } = 0.88f;

    public FireEffect(PostProcessVolume ppv) : base(ppv)
    {
    }

    protected override void InitValue()
    {
        bloom = GetSetting<Bloom>();
        ca = GetSetting<ChromaticAberration>();
    }

    protected override void GrabInitialValues()
    {
        isInit = true;
        InitValue();
        initBloomIntensity = bloom.intensity;
        initCaIntensity = ca.intensity;
    }

    public override void ShowEffect()
    {
        if (!isInit)
        {
            GrabInitialValues();
        }

        bloom.intensity.Override(bloomIntensity);
        ca.intensity.Override(caIntensity);
    }

    public override void RecoverEffect()
    {
        bloom.intensity.Override(initBloomIntensity);
        ca.intensity.Override(initCaIntensity);
    }

    public override void ShowLerpEffect(float t)
    {
        if (!isInit)
        {
            GrabInitialValues();
        }

        var bi = Mathf.Lerp(initBloomIntensity, bloomIntensity, t);
        var ci = Mathf.Lerp(initCaIntensity, caIntensity, t);
        bloom.intensity.Override(bi);
        ca.intensity.Override(ci);
    }

    public override void RecoverLerpEffect(float t)
    {
        var bi = Mathf.Lerp(bloomIntensity, initBloomIntensity, t);
        var ci = Mathf.Lerp(caIntensity, initCaIntensity, t);
        bloom.intensity.Override(bi);
        ca.intensity.Override(ci);
    }
}
