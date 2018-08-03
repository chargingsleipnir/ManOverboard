// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "AQUAS/Camera Effects/Under Water" {
    Properties {
        _DistortionTexture ("Distortion Texture", 2D) = "bump" {}
        _DistortionIntensity ("Distortion Intensity", Range(0, 1)) = 0
        _DistortionSpeed ("Distortion Speed", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _DistortionTexture; uniform float4 _DistortionTexture_ST;
            uniform float _DistortionIntensity;
            uniform float _DistortionSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 node_8772 = _Time + _TimeEditor;
                float _rotation_ang = node_8772.g;
                float _rotation_spd = _DistortionSpeed;
                float _rotation_cos = cos(_rotation_spd*_rotation_ang);
                float _rotation_sin = sin(_rotation_spd*_rotation_ang);
                float2 _rotation_piv = float2(0.5,0.5);
                float2 _rotation = (mul(i.uv0-_rotation_piv,float2x2( _rotation_cos, -_rotation_sin, _rotation_sin, _rotation_cos))+_rotation_piv);
                float3 _DistortionTexture_var = UnpackNormal(tex2D(_DistortionTexture,TRANSFORM_TEX(_rotation, _DistortionTexture)));
                float2 _componentMask = abs(i.screenPos.rg).rg;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (float2(_DistortionTexture_var.r,_DistortionTexture_var.g)*(1.0 - pow(max(_componentMask.r,_componentMask.g),3.0))*(_DistortionIntensity*0.2));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
/////// Vectors:
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
