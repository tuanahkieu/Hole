Shader "Renge/PPB_BuiltIn" {
	Properties {
		_Value ("Value", Float) = 0.64
		_SegmentCount ("SegmentCount", Float) = 1
		_SegmentSpacing ("SegmentSpacing", Float) = 0
		_Width ("Width", Float) = 0.2
		_Radius ("Radius", Float) = 0.3
		_Arc ("Arc", Float) = 0
		_Slant ("Slant", Float) = 0
		_AntiAlias ("AntiAlias", Float) = 1
		_Pixelate ("Pixelate", Float) = 0
		_PixelCount ("PixelCount", Float) = 0
		_FlipbookFPS ("FlipbookFPS", Float) = 24
		_VariableWidthCurve ("VariableWidthCurve", 2D) = "white" {}
		[HDR] _OverlayColor ("OverlayColor", Vector) = (1,1,1,1)
		_OverlayTexture ("OverlayTexture", 2D) = "white" {}
		_OverlayFlipbookDim ("OverlayFlipbookDim", Vector) = (1,1,0,0)
		_OverlayTextureOffset ("OverlayTextureOffset", Vector) = (0,0,0,0)
		_OverlayTextureTiling ("OverlayTextureTiling", Vector) = (1,1,0,0)
		_OverlayTextureOpacity ("OverlayTextureOpacity", Float) = 1
		[HDR] _BorderColor ("BorderColor", Vector) = (1,1,1,1)
		_BorderWidth ("BorderWidth", Float) = 0.06
		_BorderRadius ("BorderRadius", Float) = 0.3
		_BorderTextureOpacity ("BorderTextureOpacity", Float) = 1
		_BorderTexture ("BorderTexture", 2D) = "white" {}
		_BorderTextureTiling ("BorderTextureTiling", Vector) = (1,1,0,0)
		_AdjustBorderRadiusToWidthCurve ("AdjustBorderRadiusToWidthCurve", Float) = 0
		_BorderTextureOffset ("BorderTextureOffset", Vector) = (0,0,0,0)
		_BorderFlipbookDim ("BorderFlipbookDim", Vector) = (1,1,0,0)
		_BorderRadiusOffset ("BorderRadiusOffset", Vector) = (0,0,0,0)
		[HDR] _BorderInsetShadowColor ("BorderInsetShadowColor", Vector) = (0,0,0,1)
		_BorderInsetShadowSize ("BorderInsetShadowSize", Float) = 0.04
		_BorderInsetShadowFalloff ("BorderInsetShadowFalloff", Float) = 0.99
		[HDR] _BorderShadowColor ("BorderShadowColor", Vector) = (0,0,0,1)
		_BorderShadowSize ("BorderShadowSize", Float) = 0.05
		_BorderShadowFalloff ("BorderShadowFalloff", Float) = 0.99
		_InnerTextureOpacity ("InnerTextureOpacity", Float) = 0
		_BackgroundTextureOpacity ("BackgroundTextureOpacity", Float) = 0
		_ValueAsGradientTimeBackground ("ValueAsGradientTimeBackground", Float) = 0
		_ValueAsGradientTimeInner ("ValueAsGradientTimeInner", Float) = 0
		_InnerGradientEnabled ("InnerGradientEnabled", Float) = 0
		_BackgroundGradientEnabled ("BackgroundGradientEnabled", Float) = 0
		[HDR] _InnerColor ("InnerColor", Vector) = (1,1,1,1)
		[HDR] _BackgroundColor ("BackgroundColor", Vector) = (1,1,1,1)
		_BackgroundGradient ("BackgroundGradient", 2D) = "white" {}
		_InnerGradient ("InnerGradient", 2D) = "white" {}
		_InnerGradientRotation ("InnerGradientRotation", Float) = 0
		_BackgroundGradientRotation ("BackgroundGradientRotation", Float) = 0
		_BackgroundTextureScaleWithSegments ("BackgroundTextureScaleWithSegments", Float) = 0
		_InnerTextureScaleWithSegments ("InnerTextureScaleWithSegments", Float) = 1
		_BackgroundTextureTiling ("BackgroundTextureTiling", Vector) = (1,1,0,0)
		_InnerTextureTiling ("InnerTextureTiling", Vector) = (1,1,0,0)
		_BackgroundTexture ("BackgroundTexture", 2D) = "white" {}
		_InnerTexture ("InnerTexture", 2D) = "white" {}
		_InnerTextureRotation ("InnerTextureRotation", Float) = 0
		_BackgroundTextureRotation ("BackgroundTextureRotation", Float) = 0
		_InnerFlipbookDim ("InnerFlipbookDim", Vector) = (1,1,0,0)
		_BackgroundFlipbookDim ("BackgroundFlipbookDim", Vector) = (1,1,0,0)
		_InnerTextureOffset ("InnerTextureOffset", Vector) = (0,0,0,0)
		_BackgroundTextureOffset ("BackgroundTextureOffset", Vector) = (0,0,0,0)
		[HDR] _InnerBorderColor ("InnerBorderColor", Vector) = (0.745283,0,0,1)
		_InnerBorderWidth ("InnerBorderWidth", Float) = 0.02
		_InnerRoundingPercent ("InnerRoundingPercent", Float) = 0
		_UIScaling ("UIScaling", Float) = 0
		_CustomScale ("CustomScale", Vector) = (1,1,0,0)
		_CircleLength ("CircleLength", Float) = 0.2
		_CenterFill ("CenterFill", Float) = 0
		_OffsetTextureWithValue ("OffsetTextureWithValue", Float) = 1
		_PulsateWhenLow ("PulsateWhenLow", Float) = 1
		_PulseSpeed ("PulseSpeed", Float) = 10
		_PulseActivationThreshold ("PulseActivationThreshold", Range(0, 1)) = 0.5
		_PulseRamp ("PulseRamp", Range(0, 1)) = 0.1
		_ValueMaskOffset ("ValueMaskOffset", Vector) = (0,0,0,0)
		[HDR] _PulseColor ("PulseColor", Vector) = (0,0,0,1)
		[HDR] _ValueInsetShadowColor ("ValueInsetShadowColor", Vector) = (0,0,0,1)
		[HDR] _ValueShadowColor ("ValueShadowColor", Vector) = (0,0,0,1)
		_ValueInsetShadowSize ("ValueInsetShadowSize", Float) = 0.1
		_ValueShadowSize ("ValueShadowSize", Float) = 0.1
		_ValueInsetShadowFalloff ("ValueInsetShadowFalloff", Float) = 0.99
		_ValueShadowFalloff ("ValueShadowFalloff", Float) = 0.99
		_BorderTextureRotation ("BorderTextureRotation", Float) = 0
		_BorderTextureScaleWithSegments ("BorderTextureScaleWithSegments", Float) = 0
		_RatioScaling ("RatioScaling", Float) = 1
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	//CustomEditor "Renge.PPB.ProceduralProgressBarGUI"
}