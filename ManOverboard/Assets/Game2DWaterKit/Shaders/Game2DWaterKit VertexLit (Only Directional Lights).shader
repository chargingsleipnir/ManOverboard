Shader "Game2DWaterKit/VertexLit (Only Directional Lights)"
{
        Properties {
    	//Water Properties
		[HideInInspector] _WaterColor ("Water Color",color) = (0.11,0.64,0.92,0.25)
		[HideInInspector] _WaterColorGradientStart ("Water Color Gradient Start",color) = (1.0,1.0,1.0,0.25)
		[HideInInspector] _WaterColorGradientEnd ("Water Color Gradient End",color) = (1.0,1.0,1.0,0.25)
		[HideInInspector] _WaterTexture ("Water Texture (RGBA)" , 2D) = "white" {}
		[HideInInspector] _WaterTextureOpacity ("Water Texture Opacity",range(0,1)) = 0.5
		[HideInInspector] _WaterNoiseSpeed ("Water Noise Speed",float) = 0.025
		[HideInInspector] _WaterNoiseScaleOffset ("Water Noise Scale Offset",vector) = (1,1,0,0)
		[HideInInspector] _WaterNoiseStrength ("Water Noise Strength",Range(0.001,1.0)) = 0.025

		//Water Surface Properties
		[HideInInspector] _SurfaceLevel ("Surface Level",range(0.0,1.0)) = 0.9
		[HideInInspector] _SurfaceColor ("Surface Color",color) = (0.14,0.54,0.85,0.25)
      	[HideInInspector] _SurfaceTexture ("Surface Texture (RGBA)",2D) = "white" {}
		[HideInInspector] _SurfaceTextureOpacity ("Surface Texture Opacity",range(0,1)) = 0.5
		[HideInInspector] _SurfaceNoiseSpeed ("Surface Noise Speed",float) = 0.025
		[HideInInspector] _SurfaceNoiseScaleOffset ("Surface Noise Scale Offset",vector) = (1,1,0,0)
		[HideInInspector] _SurfaceNoiseStrength ("Surface Noise Strength",Range(0.001,1.0)) = 0.025

		//Refraction Properties
		[HideInInspector] _RefractionAmountOfBending ("Refraction Amount Of Bending",Range(0.0,0.025)) = 0.0
		[HideInInspector] _RefractionNoiseSpeed ("Refraction Speed",float) = 0.075
		[HideInInspector] _RefractionNoiseScaleOffset ("Refraction Noise Scale Offset",vector) = (8,5,0,0)
		[HideInInspector] _RefractionNoiseStrength ("Refraction Noise Strength",Range(0.001,0.1)) = 0.015

		//Reflection Properties
		[HideInInspector] _ReflectionVisibility ("Reflection Visibility",range(0,1)) = 0.3
		[HideInInspector] _ReflectionNoiseSpeed ("Reflection Speed",float) = 0.075
		[HideInInspector] _ReflectionNoiseScaleOffset ("Reflection Noise Scale Offset",vector) = (5,14,0,0)
		[HideInInspector] _ReflectionNoiseStrength ("Reflection Noise Strength",Range(0.001,0.1)) = 0.02

		//Noise Texture (RGBA): water(A) , surface(B) , reflection(G) and refraction(R)
		[HideInInspector] _NoiseTexture ("Noise Texture (RGBA)",2D) = "black"{}

		//Camera Render Rendertextures
		[HideInInspector] _RefractionTexture ("Refraction Texture", 2D) = "white" {}
		[HideInInspector] _ReflectionTexture ("Reflection Texture",2D) = "white" {}

		// Other properties
		[HideInInspector] _Mode ("__mode", float) = 2000.0
		[HideInInspector] _SrcBlend ("__src", float) = 1.0
		[HideInInspector] _DstBlend ("__dst", float) = 0.0
		[HideInInspector] _ZWrite ("__zw", float) = 1.0

		[HideInInspector] _Water2D_IsRefractionEnabled ("__refractionState",float) = 0.0
		[HideInInspector] _Water2D_IsReflectionEnabled ("__reflectionState",float) = 0.0
		[HideInInspector] _Water2D_IsWaterNoiseEnabled ("__waterNoiseState",float) = 0.0
		[HideInInspector] _Water2D_IsSurfaceEnabled ("__surfaceState",float) = 0.0
		[HideInInspector] _Water2D_IsSurfaceNoiseEnabled ("__surfaceNoiseState",float) = 0.0
		[HideInInspector] _Water2D_IsColorGradientEnabled ("__colorGradientState",float) = 0.0
    }

	SubShader
	{
		Tags {
		"RenderType"="Opaque"
		"IgnoreProjector"="True"
		"PreviewType"="Plane"
		}

		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		Cull off

		Pass
		{
			Tags {"LIGHTMODE"="ForwardBase"}

			CGPROGRAM

			#define Water2D_LIGHT_ON

			#include "UnityCG.cginc"
			#include "Game2DWaterKit.cginc"
			#include "Lighting.cginc"

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase noshadowmask nodynlightmap nodirlightmap noshadow


			#pragma shader_feature Water2D_Refraction
			#pragma shader_feature Water2D_Reflection
			#pragma shader_feature Water2D_WaterTexture
			#pragma shader_feature Water2D_WaterNoise
			#pragma shader_feature Water2D_Surface
			#pragma shader_feature Water2D_SurfaceTexture
			#pragma shader_feature Water2D_SurfaceNoise
			#pragma shader_feature Water2D_ColorGradient

			Water2D_VertexOutput vert (water2D_VertexInput v)
			{

				 //regular work
				  Water2D_VertexOutput o = Water2D_Vert(v);

				//lighting
				o.lightColor = ShadeSH9(float4(0.0,0.0,-1.0,1.0)); 
				o.lightColor += _LightColor0.rgb * max(0,-_WorldSpaceLightPos0.z);

				#if defined(LIGHTMAP_ON)
					o.lightmapCoord.xy = v.lightmapCoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				//Applying Fog
				UNITY_TRANSFER_FOG(o,o.pos);

				return o;
			}
			
			fixed4 frag (Water2D_VertexOutput i) : SV_Target
			{
				//Calculating fragment color
				fixed4 finalColor = Water2D_Frag(i);

				//Applying light
				#if defined(LIGHTMAP_ON)
					finalColor.rgb *= i.lightColor + DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,i.lightmapCoord));
				#else
					finalColor.rgb *= i.lightColor;
				#endif

				// Applying fog
				UNITY_APPLY_FOG(i.fogCoord, finalColor);

				return finalColor;
			}

			ENDCG
	}

	}

	CustomEditor "Game2DWaterKit.Game2DWaterShaderGUI"
}
