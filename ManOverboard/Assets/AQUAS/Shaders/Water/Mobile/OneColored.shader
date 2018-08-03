// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.21 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.21;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:14,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:3138,x:34051,y:33008,varname:node_3138,prsc:2|diff-56-OUT,spec-2023-OUT,gloss-2899-OUT,normal-6242-OUT,emission-8515-OUT,lwrap-6415-OUT,alpha-1207-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32329,y:32712,ptovrint:True,ptlb:Deep Water Color,ptin:_DeepWaterColor,varname:_DeepWaterColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.04313726,c2:0.2392157,c3:0.4627451,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:4848,x:33330,y:33493,varname:node_4848,prsc:2|DIST-8178-OUT;n:type:ShaderForge.SFN_ScreenPos,id:5608,x:31536,y:31970,varname:node_5608,prsc:2,sctp:0;n:type:ShaderForge.SFN_Tex2d,id:511,x:32142,y:32252,ptovrint:True,ptlb:Reflection Tex,ptin:_ReflectionTex,varname:_ReflectionTex,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4032-OUT;n:type:ShaderForge.SFN_Slider,id:2438,x:31997,y:32484,ptovrint:True,ptlb:Reflection intensity,ptin:_Reflectionintensity,varname:_Reflectionintensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Lerp,id:4298,x:32585,y:32477,varname:node_4298,prsc:2|A-511-RGB,B-7241-RGB,T-5720-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:56,x:32852,y:32555,ptovrint:True,ptlb:Enable Reflections,ptin:_UseReflections,varname:_UseReflections,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-7241-RGB,B-4298-OUT;n:type:ShaderForge.SFN_OneMinus,id:5720,x:32329,y:32496,varname:node_5720,prsc:2|IN-2438-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8178,x:33154,y:33493,ptovrint:True,ptlb:Depth Transparency,ptin:_DepthTransparency,varname:_DepthTransparency,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.5;n:type:ShaderForge.SFN_RemapRange,id:4032,x:31923,y:32252,varname:_remap,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-830-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5268,x:33478,y:32631,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:_Specular,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2899,x:33473,y:32952,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.7;n:type:ShaderForge.SFN_ValueProperty,id:6415,x:33507,y:33380,ptovrint:False,ptlb:Light Wrapping,ptin:_LightWrapping,varname:_LightWrapping,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Tex2dAsset,id:3437,x:32615,y:32818,ptovrint:False,ptlb:Normal Texture,ptin:_NormalTexture,varname:_NormalTexture,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7678,x:32861,y:32980,varname:_texture1,prsc:2,ntxv:0,isnm:False|UVIN-8234-OUT,TEX-3437-TEX;n:type:ShaderForge.SFN_Lerp,id:6242,x:33286,y:33056,varname:_lerp,prsc:2|A-7848-OUT,B-6766-OUT,T-369-OUT;n:type:ShaderForge.SFN_Vector3,id:7848,x:32861,y:32874,varname:node_7848,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:369,x:32861,y:33405,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:_Refraction,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.67,max:1;n:type:ShaderForge.SFN_Tex2d,id:3400,x:32861,y:33189,varname:_texture2,prsc:2,ntxv:0,isnm:False|UVIN-5165-OUT,TEX-3437-TEX;n:type:ShaderForge.SFN_TexCoord,id:7305,x:31951,y:33061,varname:node_7305,prsc:2,uv:0;n:type:ShaderForge.SFN_Rotator,id:7319,x:32233,y:32997,varname:_rotator,prsc:2|UVIN-7305-UVOUT,ANG-7516-OUT;n:type:ShaderForge.SFN_Vector1,id:7516,x:31951,y:33218,varname:node_7516,prsc:2,v1:1.5708;n:type:ShaderForge.SFN_Subtract,id:6766,x:33062,y:33077,varname:_subtractor,prsc:2|A-7678-RGB,B-3400-RGB;n:type:ShaderForge.SFN_ValueProperty,id:6070,x:31557,y:33187,ptovrint:False,ptlb:Wave Speed,ptin:_WaveSpeed,varname:_WaveSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_Vector1,id:6764,x:31781,y:33339,varname:node_6764,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:4815,x:31951,y:33277,varname:node_4815,prsc:2|A-3559-OUT,B-6764-OUT;n:type:ShaderForge.SFN_Time,id:3083,x:31781,y:33406,varname:_timer,prsc:2;n:type:ShaderForge.SFN_Divide,id:5782,x:31951,y:33437,varname:node_5782,prsc:2|A-3083-TSL,B-1298-OUT;n:type:ShaderForge.SFN_Vector1,id:1298,x:31781,y:33564,varname:node_1298,prsc:2,v1:100;n:type:ShaderForge.SFN_Multiply,id:9833,x:32136,y:33334,varname:_multiplier1,prsc:2|A-4815-OUT,B-5782-OUT;n:type:ShaderForge.SFN_Add,id:7317,x:32431,y:33158,varname:node_7317,prsc:2|A-7305-UVOUT,B-9833-OUT;n:type:ShaderForge.SFN_Add,id:7008,x:32431,y:32997,varname:node_7008,prsc:2|A-7319-UVOUT,B-9833-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4588,x:31169,y:32122,varname:_componentMask,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6766-OUT;n:type:ShaderForge.SFN_Append,id:5662,x:31326,y:32122,varname:node_5662,prsc:2|A-4588-R,B-4588-G;n:type:ShaderForge.SFN_Multiply,id:9765,x:31536,y:32122,varname:node_9765,prsc:2|A-5662-OUT,B-7795-OUT;n:type:ShaderForge.SFN_Slider,id:7795,x:31169,y:32304,ptovrint:False,ptlb:Distortion,ptin:_Distortion,varname:_Distortion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:2;n:type:ShaderForge.SFN_Add,id:830,x:31728,y:32122,varname:node_830,prsc:2|A-5608-UVOUT,B-9765-OUT;n:type:ShaderForge.SFN_Color,id:2084,x:33478,y:32711,ptovrint:False,ptlb:Specular Color,ptin:_SpecularColor,varname:_SpecularColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6470588,c2:0.6518166,c3:0.6518166,c4:1;n:type:ShaderForge.SFN_Multiply,id:2023,x:33672,y:32682,varname:node_2023,prsc:2|A-5268-OUT,B-2084-RGB;n:type:ShaderForge.SFN_ObjectScale,id:611,x:31781,y:33616,varname:node_611,prsc:2,rcp:False;n:type:ShaderForge.SFN_ComponentMask,id:5184,x:31951,y:33616,varname:node_5184,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-611-XYZ;n:type:ShaderForge.SFN_Multiply,id:4502,x:32153,y:33691,varname:node_4502,prsc:2|A-5184-OUT,B-8712-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8712,x:31951,y:33800,ptovrint:False,ptlb:Normal Tiling,ptin:_NormalTiling,varname:_NormalTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:8234,x:32642,y:32997,varname:_multiplier3,prsc:2|A-7008-OUT,B-2744-OUT;n:type:ShaderForge.SFN_Multiply,id:5165,x:32642,y:33158,varname:_multiplier2,prsc:2|A-7317-OUT,B-2744-OUT;n:type:ShaderForge.SFN_Divide,id:3559,x:31781,y:33187,varname:node_3559,prsc:2|A-6070-OUT,B-2744-OUT;n:type:ShaderForge.SFN_Color,id:4722,x:33473,y:33137,ptovrint:False,ptlb:Emissive Color,ptin:_EmissiveColor,varname:_EmissiveColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1496,x:33473,y:33303,ptovrint:False,ptlb:Emission Intensity,ptin:_EmissionIntensity,varname:_EmissionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4333,x:33651,y:33195,varname:node_4333,prsc:2|A-4722-RGB,B-1496-OUT;n:type:ShaderForge.SFN_Blend,id:8515,x:33834,y:33133,varname:node_8515,prsc:2,blmd:3,clmp:True|SRC-6242-OUT,DST-4333-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9873,x:33330,y:33663,ptovrint:False,ptlb:Shore Fade,ptin:_ShoreFade,varname:_ShoreFade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_ValueProperty,id:3627,x:33330,y:33776,ptovrint:False,ptlb:Shore Transparency,ptin:_ShoreTransparency,varname:_ShoreTransparency,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Power,id:9267,x:33545,y:33561,varname:node_9267,prsc:2|VAL-4848-OUT,EXP-9873-OUT;n:type:ShaderForge.SFN_DepthBlend,id:1288,x:33545,y:33742,varname:node_1288,prsc:2|DIST-3627-OUT;n:type:ShaderForge.SFN_Multiply,id:1207,x:33771,y:33634,varname:node_1207,prsc:2|A-9267-OUT,B-1288-OUT;n:type:ShaderForge.SFN_Divide,id:2744,x:32364,y:33660,varname:_division,prsc:2|A-4502-OUT,B-2520-OUT;n:type:ShaderForge.SFN_Vector1,id:2520,x:32126,y:33852,varname:node_2520,prsc:2,v1:1000;proporder:3437-8712-7241-8178-9873-3627-511-56-2438-7795-5268-2084-2899-6415-369-6070-4722-1496;pass:END;sub:END;*/

Shader "AQUAS/Mobile/Single-Colored" {
    Properties {
        [NoScaleOffset]_NormalTexture ("Normal Texture", 2D) = "white" {}
        _NormalTiling ("Normal Tiling", Float ) = 1
        _DeepWaterColor ("Deep Water Color", Color) = (0.04313726,0.2392157,0.4627451,1)
        _DepthTransparency ("Depth Transparency", Float ) = 1.5
        _ShoreFade ("Shore Fade", Float ) = 0.3
        _ShoreTransparency ("Shore Transparency", Float ) = 0
        [HideInInspector]_ReflectionTex ("Reflection Tex", 2D) = "white" {}
        [MaterialToggle] _UseReflections ("Enable Reflections", Float ) = 0.5215687
        _Reflectionintensity ("Reflection intensity", Range(0, 1)) = 0.5
        _Distortion ("Distortion", Range(0, 2)) = 0.3
        _Specular ("Specular", Float ) = 1
        _SpecularColor ("Specular Color", Color) = (0.6470588,0.6518166,0.6518166,1)
        _Gloss ("Gloss", Float ) = 0.7
        _LightWrapping ("Light Wrapping", Float ) = 2
        _Refraction ("Refraction", Range(0, 1)) = 0.67
        _WaveSpeed ("Wave Speed", Float ) = 40
        _EmissiveColor ("Emissive Color", Color) = (0.5,0.5,0.5,1)
        _EmissionIntensity ("Emission Intensity", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _DeepWaterColor;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform float _Reflectionintensity;
            uniform fixed _UseReflections;
            uniform float _DepthTransparency;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float _LightWrapping;
            uniform sampler2D _NormalTexture; uniform float4 _NormalTexture_ST;
            uniform float _Refraction;
            uniform float _WaveSpeed;
            uniform float _Distortion;
            uniform float4 _SpecularColor;
            uniform float _NormalTiling;
            uniform float4 _EmissiveColor;
            uniform float _EmissionIntensity;
            uniform float _ShoreFade;
            uniform float _ShoreTransparency;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float _rotator_ang = 1.5708;
                float _rotator_spd = 1.0;
                float _rotator_cos = cos(_rotator_spd*_rotator_ang);
                float _rotator_sin = sin(_rotator_spd*_rotator_ang);
                float2 _rotator_piv = float2(0.5,0.5);
                float2 _rotator = (mul(i.uv0-_rotator_piv,float2x2( _rotator_cos, -_rotator_sin, _rotator_sin, _rotator_cos))+_rotator_piv);
                float2 _division = ((objScale.rb*_NormalTiling)/1000.0);
                float4 _timer = _Time + _TimeEditor;
                float3 _multiplier1 = (float3((_WaveSpeed/_division),0.0)*(_timer.r/100.0));
                float2 _multiplier3 = ((_rotator+_multiplier1)*_division);
                float4 _texture1 = tex2D(_NormalTexture,TRANSFORM_TEX(_multiplier3, _NormalTexture));
                float2 _multiplier2 = ((i.uv0+_multiplier1)*_division);
                float4 _texture2 = tex2D(_NormalTexture,TRANSFORM_TEX(_multiplier2, _NormalTexture));
                float3 _subtractor = (_texture1.rgb-_texture2.rgb);
                float3 _lerp = lerp(float3(0,0,1),_subtractor,_Refraction);
                float3 normalLocal = _lerp;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = (_Specular*_SpecularColor.rgb);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(_LightWrapping,_LightWrapping,_LightWrapping)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = forwardLight * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float2 _componentMask = _subtractor.rg;
                float2 _remap = ((i.screenPos.rg+(float2(_componentMask.r,_componentMask.g)*_Distortion))*0.5+0.5);
                float4 _ReflectionTex_var = tex2D(_ReflectionTex,TRANSFORM_TEX(_remap, _ReflectionTex));
                float3 diffuseColor = lerp( _DeepWaterColor.rgb, lerp(_ReflectionTex_var.rgb,_DeepWaterColor.rgb,(1.0 - _Reflectionintensity)), _UseReflections );
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = saturate((_lerp+(_EmissiveColor.rgb*_EmissionIntensity)-1.0));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,(pow(saturate((sceneZ-partZ)/_DepthTransparency),_ShoreFade)*saturate((sceneZ-partZ)/_ShoreTransparency)));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
