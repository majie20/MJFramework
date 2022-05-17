using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RainDropRenderer), PostProcessEvent.AfterStack, "Unlit/Post_RainDrop")]
public sealed class RainDrop : PostProcessEffectSettings
{
    public TextureParameter noise=new TextureParameter{value=null};
    public FloatParameter _Size = new FloatParameter { value = 0.5f };
    public FloatParameter _T = new FloatParameter { value = 0.5f };
    public FloatParameter _Distortion = new FloatParameter { value = 0.5f };
    public FloatParameter _Blur = new FloatParameter { value = 0.5f };
}
public sealed class RainDropRenderer : PostProcessEffectRenderer<RainDrop>
{
    public override void Render(PostProcessRenderContext context)
    {
        var dirtTexture = settings.noise.value == null
            ? RuntimeUtilities.blackTexture
            : settings.noise.value;
        var sheet = context.propertySheets.Get(Shader.Find("Unlit/Post_RainDrop"));
        sheet.properties.SetTexture("_NoiseTex", dirtTexture);
        sheet.properties.SetFloat("_Size", settings._Size);
        sheet.properties.SetFloat("_T", settings._T);
        sheet.properties.SetFloat("_Distortion", settings._Distortion);
        sheet.properties.SetFloat("_Blur", settings._Blur);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
