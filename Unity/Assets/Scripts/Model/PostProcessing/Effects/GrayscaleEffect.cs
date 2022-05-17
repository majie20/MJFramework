using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GrayscaleEffect : BaseEffect
{
    private Grayscale gs;

    private float initgsIntensity;

    public float gsIntensity { get; set; } = 1.0f;

    public GrayscaleEffect(PostProcessVolume ppv) : base(ppv)
    {
    }

    protected override void InitValue()
    {
        gs = GetSetting<Grayscale>();
    }

    protected override void GrabInitialValues()
    {
        isInit = true;
        InitValue();
        initgsIntensity = gs.blend;
    }

    public override void ShowEffect()
    {
        if (!isInit)
        {
            GrabInitialValues();
        }

        gs.blend.Override(gsIntensity);
    }

    public override void RecoverEffect()
    {
        gs.blend.Override(initgsIntensity);
    }

    public override void ShowLerpEffect(float t)
    {
        if (!isInit)
        {
            GrabInitialValues();
        }

        var gi = Mathf.Lerp(initgsIntensity, gsIntensity, t);
        gs.blend.Override(gi);
    }

    public override void RecoverLerpEffect(float t)
    {
        var gi = Mathf.Lerp(gsIntensity, initgsIntensity, t);
        gs.blend.Override(gi);
    }
}

