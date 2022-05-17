Shader "Unlit/Post_RainDrop"
{
	HLSLINCLUDE
	// StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	// Lerp the pixel color with the luminance using the _Blend uniform.
	float _Blend;

	half _Size;
	half _T;
	half _Distortion;
	half _Blur;
	// 求伪随机数
	half N21(half2 p) {
		p = frac(p * half2(123.34, 345.45));
		p += dot(p, p + 34.345);
		return frac(p.x + p.y);
	}

	half3 layer(half2 UV, half T) {
		half t = fmod(_Time.y + T, 3600);
		half4 col = half4(0, 0, 0, 1.0);
		half aspect = half2(2, 1);
		half2 uv = UV * _Size * aspect;
		uv.y += t * 0.25;
		half2 gv = frac(uv) - 0.5;//-0.5，调整原点为中间
		half2 id = floor(uv);
		half n = N21(id); // 0 1
		t += n * 6.2831; //2PI

		half w = UV.y * 10;
		half x = (n - 0.5) * 0.8;
		x += (0.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6) * 0.45;
		half y = -sin(t + sin(t + sin(t) * 0.5)) * 0.45;
		y -= (gv.x - x) * (gv.x - x);
		half2 dropPos = (gv - half2(x, y)) / aspect; //- half2(x,y) 为了移动
		half drop = smoothstep(0.05, 0.03, length(dropPos));

		half2 trailPos = (gv - half2(x, t * 0.25)) / aspect; //- half2(x,y) 为了移动
		trailPos.y = (frac(trailPos.y * 8) - 0.5) / 8;
		half trail = smoothstep(0.03, 0.01, length(trailPos));
		half fogTrail = smoothstep(-0.05, 0.05, dropPos.y);// 拖尾小水滴慢慢被拖掉了
		fogTrail *= smoothstep(0.5, y, gv.y);// 拖尾小水滴渐变消失
		fogTrail *= smoothstep(0.05, 0.04, abs(dropPos.x));
		trail *= fogTrail;
		//col += fogTrail * 0.5;
		//col += trail;
		//col += drop;
		//if(gv.x > 0.48 || gv.y > 0.49) col = half4(1.0, 0, 0, 1.0); // 辅助线
		half2 offset = drop * dropPos + trail * trailPos;
		return half3(offset, fogTrail);
	}

	float4 Frag(VaryingsDefault i) : SV_Target
	{ 
		half3 drops = layer(i.texcoord, _T);
		drops += layer(i.texcoord * 1.25 + 7.52, _T);
		drops += layer(i.texcoord * 1.35 + 1.54, _T);
		drops += layer(i.texcoord * 1.57 - 7.52, _T);
		half blur = _Blur * 7 * (1 - drops.z);
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, half4(i.texcoord + drops.xy * _Distortion, 0, blur));
		return color;
	}
	ENDHLSL
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			HLSLPROGRAM
				#pragma vertex VertDefault
				#pragma fragment Frag
			ENDHLSL
		}
	}
}
