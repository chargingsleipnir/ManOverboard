// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.21 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.21;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:14,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:3138,x:34049,y:33004,varname:node_3138,prsc:2|diff-56-OUT,spec-2023-OUT,gloss-2899-OUT,normal-6242-OUT,lwrap-6415-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31704,y:32400,ptovrint:True,ptlb:Water Color,ptin:_DeepWaterColor,varname:_DeepWaterColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_ScreenPos,id:5608,x:31536,y:31970,varname:node_5608,prsc:2,sctp:0;n:type:ShaderForge.SFN_Tex2d,id:511,x:32142,y:32252,ptovrint:True,ptlb:Reflection Tex,ptin:_ReflectionTex,varname:_ReflectionTex,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4032-OUT;n:type:ShaderForge.SFN_Slider,id:2438,x:31997,y:32484,ptovrint:True,ptlb:Reflection intensity,ptin:_Reflectionintensity,varname:_Reflectionintensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Lerp,id:4298,x:32585,y:32477,varname:node_4298,prsc:2|A-511-RGB,B-7241-RGB,T-5720-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:56,x:32839,y:32405,ptovrint:True,ptlb:Enable Reflections,ptin:_UseReflections,varname:_UseReflections,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-7241-RGB,B-4298-OUT;n:type:ShaderForge.SFN_OneMinus,id:5720,x:32355,y:32538,varname:node_5720,prsc:2|IN-2438-OUT;n:type:ShaderForge.SFN_RemapRange,id:4032,x:31943,y:32226,varname:_remap,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-830-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5268,x:33471,y:32867,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:_Specular,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2899,x:33485,y:33180,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.8;n:type:ShaderForge.SFN_ValueProperty,id:6415,x:33485,y:33272,ptovrint:False,ptlb:Light Wrapping,ptin:_LightWrapping,varname:_LightWrapping,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.5;n:type:ShaderForge.SFN_Tex2dAsset,id:3437,x:32615,y:32818,ptovrint:False,ptlb:Normal Texture,ptin:_NormalTexture,varname:_NormalTexture,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7678,x:32861,y:32997,varname:_texture1,prsc:2,ntxv:0,isnm:False|UVIN-8234-OUT,TEX-3437-TEX;n:type:ShaderForge.SFN_Lerp,id:6242,x:33301,y:33077,varname:node_6242,prsc:2|A-7848-OUT,B-6766-OUT,T-369-OUT;n:type:ShaderForge.SFN_Vector3,id:7848,x:32861,y:32878,varname:node_7848,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:369,x:32832,y:33366,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:_Refraction,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2d,id:3400,x:32861,y:33144,varname:_texture2,prsc:2,ntxv:0,isnm:False|UVIN-5165-OUT,TEX-3437-TEX;n:type:ShaderForge.SFN_TexCoord,id:7305,x:31951,y:33061,varname:node_7305,prsc:2,uv:0;n:type:ShaderForge.SFN_Rotator,id:7319,x:32233,y:32997,varname:_rotator1,prsc:2|UVIN-7305-UVOUT,ANG-7516-OUT;n:type:ShaderForge.SFN_Vector1,id:7516,x:31951,y:33218,varname:node_7516,prsc:2,v1:1.5708;n:type:ShaderForge.SFN_Subtract,id:6766,x:33062,y:33077,varname:_subtractor1,prsc:2|A-7678-RGB,B-3400-RGB;n:type:ShaderForge.SFN_ValueProperty,id:6070,x:31557,y:33187,ptovrint:False,ptlb:Wave Speed,ptin:_WaveSpeed,varname:_WaveSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_Vector1,id:6764,x:31781,y:33339,varname:node_6764,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:4815,x:31951,y:33277,varname:node_4815,prsc:2|A-3559-OUT,B-6764-OUT;n:type:ShaderForge.SFN_Time,id:3083,x:31781,y:33406,varname:_timer1,prsc:2;n:type:ShaderForge.SFN_Divide,id:5782,x:31951,y:33437,varname:node_5782,prsc:2|A-3083-TSL,B-1298-OUT;n:type:ShaderForge.SFN_Vector1,id:1298,x:31781,y:33552,varname:node_1298,prsc:2,v1:100;n:type:ShaderForge.SFN_Multiply,id:9833,x:32136,y:33334,varname:_multiplier3,prsc:2|A-4815-OUT,B-5782-OUT;n:type:ShaderForge.SFN_Add,id:7317,x:32431,y:33158,varname:node_7317,prsc:2|A-7305-UVOUT,B-9833-OUT;n:type:ShaderForge.SFN_Add,id:7008,x:32431,y:32997,varname:node_7008,prsc:2|A-7319-UVOUT,B-9833-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4588,x:31169,y:32122,varname:_componentMask,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6766-OUT;n:type:ShaderForge.SFN_Append,id:5662,x:31326,y:32122,varname:node_5662,prsc:2|A-4588-R,B-4588-G;n:type:ShaderForge.SFN_Multiply,id:9765,x:31536,y:32122,varname:node_9765,prsc:2|A-5662-OUT,B-7795-OUT;n:type:ShaderForge.SFN_Slider,id:7795,x:31169,y:32304,ptovrint:False,ptlb:Distortion,ptin:_Distortion,varname:_Distortion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:2;n:type:ShaderForge.SFN_Add,id:830,x:31728,y:32122,varname:node_830,prsc:2|A-5608-UVOUT,B-9765-OUT;n:type:ShaderForge.SFN_Color,id:2084,x:33471,y:32944,ptovrint:False,ptlb:Specular Color,ptin:_SpecularColor,varname:_SpecularColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:2023,x:33679,y:32944,varname:node_2023,prsc:2|A-5268-OUT,B-2084-RGB;n:type:ShaderForge.SFN_ObjectScale,id:611,x:31781,y:33600,varname:node_611,prsc:2,rcp:False;n:type:ShaderForge.SFN_ComponentMask,id:5184,x:31951,y:33600,varname:node_5184,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-611-XYZ;n:type:ShaderForge.SFN_Multiply,id:4502,x:32144,y:33686,varname:node_4502,prsc:2|A-5184-OUT,B-8712-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8712,x:31951,y:33785,ptovrint:False,ptlb:Normal Tiling,ptin:_NormalTiling,varname:_NormalTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:8234,x:32642,y:32997,varname:_multiplier1,prsc:2|A-7008-OUT,B-5826-OUT;n:type:ShaderForge.SFN_Multiply,id:5165,x:32642,y:33158,varname:_multiplier2,prsc:2|A-7317-OUT,B-5826-OUT;n:type:ShaderForge.SFN_Divide,id:3559,x:31781,y:33187,varname:node_3559,prsc:2|A-6070-OUT,B-5826-OUT;n:type:ShaderForge.SFN_Divide,id:5826,x:32344,y:33686,varname:_division1,prsc:2|A-4502-OUT,B-5041-OUT;n:type:ShaderForge.SFN_Vector1,id:5041,x:32144,y:33825,varname:node_5041,prsc:2,v1:1000;proporder:3437-8712-7241-511-56-2438-7795-5268-2084-2899-6415-369-6070;pass:END;sub:END;*/

Shader "AQUAS/Mobile/Opaque" {
    Properties {
        [NoScaleOffset]_NormalTexture ("Normal Texture", 2D) = "white" {}
        _NormalTiling ("Normal Tiling", Float ) = 1
        _DeepWaterColor ("Water Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        [HideInInspector]_ReflectionTex ("Reflection Tex", 2D) = "white" {}
        [MaterialToggle] _UseReflections ("Enable Reflections", Float ) = 0.5392157
        _Reflectionintensity ("Reflection intensity", Range(0, 1)) = 0.5
        _Distortion ("Distortion", Range(0, 2)) = 0.3
        _Specular ("Specular", Float ) = 1
        _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Gloss ("Gloss", Float ) = 0.8
        _LightWrapping ("Light Wrapping", Float ) = 1.5
        _Refraction ("Refraction", Range(0, 1)) = 0.5
        _WaveSpeed ("Wave Speed", Float ) = 40
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float4 _DeepWaterColor;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform float _Reflectionintensity;
            uniform fixed _UseReflections;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float _LightWrapping;
            uniform sampler2D _NormalTexture; uniform float4 _NormalTexture_ST;
            uniform float _Refraction;
            uniform float _WaveSpeed;
            uniform float _Distortion;
            uniform float4 _SpecularColor;
            uniform float _NormalTiling;
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
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
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
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float _rotator1_ang = 1.5708;
                float _rotator1_spd = 1.0;
                float _rotator1_cos = cos(_rotator1_spd*_rotator1_ang);
                float _rotator1_sin = sin(_rotator1_spd*_rotator1_ang);
                float2 _rotator1_piv = float2(0.5,0.5);
                float2 _rotator1 = (mul(i.uv0-_rotator1_piv,float2x2( _rotator1_cos, -_rotator1_sin, _rotator1_sin, _rotator1_cos))+_rotator1_piv);
                float2 _division1 = ((objScale.rb*_NormalTiling)/1000.0);
                float4 _timer1 = _Time + _TimeEditor;
                float3 _multiplier3 = (float3((_WaveSpeed/_division1),0.0)*(_timer1.r/100.0));
                float2 _multiplier1 = ((_rotator1+_multiplier3)*_division1);
                float4 _texture1 = tex2D(_NormalTexture,TRANSFORM_TEX(_multiplier1, _NormalTexture));
                float2 _multiplier2 = ((i.uv0+_multiplier3)*_division1);
                float4 _texture2 = tex2D(_NormalTexture,TRANSFORM_TEX(_multiplier2, _NormalTexture));
                float3 _subtractor1 = (_texture1.rgb-_texture2.rgb);
                float3 normalLocal = lerp(float3(0,0,1),_subtractor1,_Refraction);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
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
                float2 _componentMask = _subtractor1.rg;
                float2 _remap = ((i.screenPos.rg+(float2(_componentMask.r,_componentMask.g)*_Distortion))*0.5+0.5);
                float4 _ReflectionTex_var = tex2D(_ReflectionTex,TRANSFORM_TEX(_remap, _ReflectionTex));
                float3 diffuseColor = lerp( _DeepWaterColor.rgb, lerp(_ReflectionTex_var.rgb,_DeepWaterColor.rgb,(1.0 - _Reflectionintensity)), _UseReflections );
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
