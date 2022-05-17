using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BloodDropEffect : BaseEffect
{
    private Vignette vignette;

    private float initIntensity;
    private Color initColor;
    private float initSmoothness;

    public float Intensity { get; set; } = 0.38f; 
    public Color Color { get; set; } = Color.red;
    public float Smoothness { get; set; } = 1f;

    public BloodDropEffect(PostProcessVolume ppv) : base(ppv)
    {
    }

    protected override void InitValue()
    {
        vignette = GetSetting<Vignette>();
    }

    protected override void GrabInitialValues()
    {
        isInit = true;
        InitValue();
        initIntensity = vignette.intensity;
        initColor = vignette.color;
        initSmoothness = vignette.smoothness;
    }

    public override void ShowEffect()
    {
        if (!isInit)
        {
            GrabInitialValues();
        }

        vignette.intensity.Override(Intensity);
        vignette.color.Override(Color);
        vignette.smoothness.Override(Smoothness);
    }

    public override void RecoverEffect()
    {
        vignette.intensity.Override(initIntensity);
        vignette.color.Override(initColor);
        vignette.smoothness.Override(initSmoothness);
    }

    public override void ShowLerpEffect(float t)
    {
        if (!isInit)
        {
            GrabInitialValues();
        }

        if (vignette.color != Color|| vignette.smoothness != Smoothness)
        {
            vignette.color.Override(Color);
            vignette.smoothness.Override(Smoothness);
        }
        var gi = Mathf.Lerp(initIntensity, Intensity, t);
        vignette.intensity.Override(gi);
    }

    public override void RecoverLerpEffect(float t)
    {
        if (vignette.color != initColor || vignette.smoothness != initSmoothness)
        {
            vignette.color.Override(initColor);
            vignette.smoothness.Override(initSmoothness);
        }
        var gi = Mathf.Lerp(Intensity, initIntensity, t);
        vignette.intensity.Override(gi);
    }
}

