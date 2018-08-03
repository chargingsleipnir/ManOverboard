// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.21 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.21;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:3138,x:34051,y:33008,varname:node_3138,prsc:2|diff-3680-OUT,spec-2023-OUT,gloss-2899-OUT,normal-6242-OUT,emission-4333-OUT,lwrap-6415-OUT,alpha-2487-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32329,y:32712,ptovrint:True,ptlb:Deep Water Color,ptin:_DeepWaterColor,varname:_DeepWaterColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.345098,c3:0.7372549,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:4848,x:33507,y:33448,varname:node_4848,prsc:2|DIST-8178-OUT;n:type:ShaderForge.SFN_ScreenPos,id:5608,x:31536,y:31970,varname:node_5608,prsc:2,sctp:0;n:type:ShaderForge.SFN_Tex2d,id:511,x:32142,y:32252,ptovrint:True,ptlb:Reflection Tex,ptin:_ReflectionTex,varname:_ReflectionTex,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4032-OUT;n:type:ShaderForge.SFN_Slider,id:2438,x:31997,y:32484,ptovrint:True,ptlb:Reflection intensity,ptin:_Reflectionintensity,varname:_Reflectionintensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Lerp,id:4298,x:32585,y:32477,varname:node_4298,prsc:2|A-511-RGB,B-7241-RGB,T-5720-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:56,x:32852,y:32555,ptovrint:True,ptlb:Enable Reflections,ptin:_UseReflections,varname:_UseReflections,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-7241-RGB,B-4298-OUT;n:type:ShaderForge.SFN_OneMinus,id:5720,x:32329,y:32496,varname:node_5720,prsc:2|IN-2438-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8178,x:33318,y:33482,ptovrint:True,ptlb:Depth Transparency,ptin:_DepthTransparency,varname:_DepthTransparency,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.5;n:type:ShaderForge.SFN_RemapRange,id:4032,x:31923,y:32252,varname:_remap,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-830-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5268,x:33478,y:32631,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:_Specular,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2899,x:33473,y:32952,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.7;n:type:ShaderForge.SFN_ValueProperty,id:6415,x:33507,y:33380,ptovrint:False,ptlb:Light Wrapping,ptin:_LightWrapping,varname:_LightWrapping,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Tex2dAsset,id:3437,x:32615,y:32818,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:_Normal,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7678,x:32861,y:32980,varname:_texture1,prsc:2,ntxv:0,isnm:False|UVIN-8234-OUT,TEX-3437-TEX;n:type:ShaderForge.SFN_Lerp,id:6242,x:33301,y:33077,varname:_lerp,prsc:2|A-7848-OUT,B-6766-OUT,T-369-OUT;n:type:ShaderForge.SFN_Vector3,id:7848,x:32861,y:32874,varname:node_7848,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:369,x:32861,y:33405,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:_Refraction,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.67,max:1;n:type:ShaderForge.SFN_Tex2d,id:3400,x:32861,y:33189,varname:_texture2,prsc:2,ntxv:0,isnm:False|UVIN-5165-OUT,TEX-3437-TEX;n:type:ShaderForge.SFN_TexCoord,id:7305,x:31951,y:33061,varname:node_7305,prsc:2,uv:0;n:type:ShaderForge.SFN_Rotator,id:7319,x:32233,y:32997,varname:_rotator1,prsc:2|UVIN-7305-UVOUT,ANG-7516-OUT;n:type:ShaderForge.SFN_Vector1,id:7516,x:31951,y:33218,varname:node_7516,prsc:2,v1:1.5708;n:type:ShaderForge.SFN_Subtract,id:6766,x:33062,y:33077,varname:_subtractor1,prsc:2|A-7678-RGB,B-3400-RGB;n:type:ShaderForge.SFN_ValueProperty,id:6070,x:31557,y:33187,ptovrint:False,ptlb:Wave Speed,ptin:_WaveSpeed,varname:_WaveSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_Vector1,id:6764,x:31781,y:33339,varname:node_6764,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:4815,x:31951,y:33277,varname:node_4815,prsc:2|A-3559-OUT,B-6764-OUT;n:type:ShaderForge.SFN_Time,id:3083,x:31781,y:33406,varname:_timer2,prsc:2;n:type:ShaderForge.SFN_Divide,id:5782,x:31951,y:33437,varname:node_5782,prsc:2|A-3083-TSL,B-1298-OUT;n:type:ShaderForge.SFN_Vector1,id:1298,x:31781,y:33560,varname:node_1298,prsc:2,v1:100;n:type:ShaderForge.SFN_Multiply,id:9833,x:32136,y:33334,varname:_multiplier4,prsc:2|A-4815-OUT,B-5782-OUT;n:type:ShaderForge.SFN_Add,id:7317,x:32431,y:33158,varname:node_7317,prsc:2|A-7305-UVOUT,B-9833-OUT;n:type:ShaderForge.SFN_Add,id:7008,x:32431,y:32997,varname:node_7008,prsc:2|A-7319-UVOUT,B-9833-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4588,x:31169,y:32088,varname:_componentMask,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6766-OUT;n:type:ShaderForge.SFN_Append,id:5662,x:31326,y:32122,varname:node_5662,prsc:2|A-4588-R,B-4588-G;n:type:ShaderForge.SFN_Multiply,id:9765,x:31536,y:32122,varname:node_9765,prsc:2|A-5662-OUT,B-7795-OUT;n:type:ShaderForge.SFN_Slider,id:7795,x:31169,y:32304,ptovrint:False,ptlb:Distortion,ptin:_Distortion,varname:_Distortion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:2;n:type:ShaderForge.SFN_Add,id:830,x:31728,y:32122,varname:node_830,prsc:2|A-5608-UVOUT,B-9765-OUT;n:type:ShaderForge.SFN_Color,id:2084,x:33478,y:32711,ptovrint:False,ptlb:Specular Color,ptin:_SpecularColor,varname:_SpecularColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:2023,x:33672,y:32682,varname:node_2023,prsc:2|A-5268-OUT,B-2084-RGB;n:type:ShaderForge.SFN_ObjectScale,id:611,x:31781,y:33610,varname:node_611,prsc:2,rcp:False;n:type:ShaderForge.SFN_ComponentMask,id:5184,x:31951,y:33610,varname:node_5184,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-611-XYZ;n:type:ShaderForge.SFN_Multiply,id:4502,x:32169,y:33684,varname:node_4502,prsc:2|A-5184-OUT,B-8712-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8712,x:31951,y:33786,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:_Tiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:8234,x:32642,y:32997,varname:_multiplier2,prsc:2|A-7008-OUT,B-5979-OUT;n:type:ShaderForge.SFN_Multiply,id:5165,x:32642,y:33158,varname:_multiplier3,prsc:2|A-7317-OUT,B-5979-OUT;n:type:ShaderForge.SFN_Divide,id:3559,x:31781,y:33187,varname:node_3559,prsc:2|A-6070-OUT,B-5979-OUT;n:type:ShaderForge.SFN_Color,id:4722,x:33473,y:33137,ptovrint:False,ptlb:Emissive Color,ptin:_EmissiveColor,varname:_EmissiveColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1496,x:33473,y:33303,ptovrint:False,ptlb:Emission Intensity,ptin:_EmissionIntensity,varname:_EmissionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4333,x:33651,y:33195,varname:_multiplier1,prsc:2|A-4722-RGB,B-1496-OUT;n:type:ShaderForge.SFN_Blend,id:8515,x:33834,y:33133,varname:node_8515,prsc:2,blmd:3,clmp:True|SRC-6242-OUT,DST-4333-OUT;n:type:ShaderForge.SFN_Power,id:9998,x:33692,y:33475,varname:node_9998,prsc:2|VAL-4848-OUT,EXP-5061-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5061,x:33490,y:33657,ptovrint:False,ptlb:Shore Fade,ptin:_ShoreFade,varname:_ShoreFade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_ValueProperty,id:1830,x:33437,y:33826,ptovrint:False,ptlb:Shore Transparency,ptin:_ShoreTransparency,varname:_ShoreTransparency,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.04;n:type:ShaderForge.SFN_DepthBlend,id:1510,x:33665,y:33743,varname:node_1510,prsc:2|DIST-1830-OUT;n:type:ShaderForge.SFN_Multiply,id:2487,x:33850,y:33579,varname:node_2487,prsc:2|A-9998-OUT,B-1510-OUT;n:type:ShaderForge.SFN_Lerp,id:3680,x:33155,y:32443,varname:node_3680,prsc:2|A-56-OUT,B-2433-OUT,T-8955-OUT;n:type:ShaderForge.SFN_Multiply,id:2433,x:32934,y:32078,varname:node_2433,prsc:2|A-3099-OUT,B-3099-OUT;n:type:ShaderForge.SFN_Multiply,id:3099,x:32753,y:32078,varname:_multiplier8,prsc:2|A-4558-OUT,B-8741-OUT;n:type:ShaderForge.SFN_RemapRange,id:8741,x:32564,y:32078,varname:node_8741,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-6622-OUT;n:type:ShaderForge.SFN_DepthBlend,id:6622,x:32391,y:32078,varname:node_6622,prsc:2|DIST-7338-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7338,x:32225,y:32112,ptovrint:False,ptlb:Foam Blend,ptin:_FoamBlend,varname:_FoamBlend,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.15;n:type:ShaderForge.SFN_Slider,id:8955,x:32706,y:32359,ptovrint:False,ptlb:Foam Visibility,ptin:_FoamVisibility,varname:_FoamVisibility,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:1;n:type:ShaderForge.SFN_Multiply,id:4558,x:32563,y:31701,varname:node_4558,prsc:2|A-485-OUT,B-7737-OUT;n:type:ShaderForge.SFN_Multiply,id:485,x:32392,y:31593,varname:node_485,prsc:2|A-6543-OUT,B-9405-RGB;n:type:ShaderForge.SFN_Multiply,id:7737,x:32392,y:31729,varname:node_7737,prsc:2|A-6489-OUT,B-620-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:6543,x:32193,y:31507,varname:node_6543,prsc:2|IN-6001-OUT,IMIN-969-OUT,IMAX-2513-OUT,OMIN-8935-OUT,OMAX-8695-OUT;n:type:ShaderForge.SFN_Color,id:9405,x:32193,y:31653,ptovrint:False,ptlb:Foam Color,ptin:_FoamColor,varname:_FoamColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7647059,c2:0.7647059,c3:0.7647059,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:6489,x:32193,y:31831,ptovrint:False,ptlb:Foam Intensity,ptin:_FoamIntensity,varname:_FoamIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Vector1,id:620,x:32193,y:31939,varname:node_620,prsc:2,v1:-1;n:type:ShaderForge.SFN_Desaturate,id:6001,x:31964,y:31392,varname:node_6001,prsc:2|COL-7498-OUT;n:type:ShaderForge.SFN_Slider,id:969,x:31807,y:31543,ptovrint:False,ptlb:Foam Contrast,ptin:_FoamContrast,varname:_FoamContrast,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:0.5;n:type:ShaderForge.SFN_OneMinus,id:2513,x:31964,y:31653,varname:node_2513,prsc:2|IN-969-OUT;n:type:ShaderForge.SFN_Vector1,id:8935,x:31964,y:31797,varname:_value,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:8695,x:31964,y:31909,varname:node_8695,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:7498,x:31807,y:31392,varname:node_7498,prsc:2|A-4759-RGB,B-9071-RGB;n:type:ShaderForge.SFN_Tex2dAsset,id:8822,x:31365,y:31096,ptovrint:False,ptlb:Foam Texture,ptin:_FoamTexture,varname:_FoamTexture,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4759,x:31556,y:31275,varname:_texture3,prsc:2,ntxv:0,isnm:False|UVIN-1516-OUT,TEX-8822-TEX;n:type:ShaderForge.SFN_Tex2d,id:9071,x:31556,y:31447,varname:_texture4,prsc:2,ntxv:0,isnm:False|UVIN-8278-OUT,TEX-8822-TEX;n:type:ShaderForge.SFN_Multiply,id:1516,x:31365,y:31275,varname:_multiplier6,prsc:2|A-3769-OUT,B-2635-OUT;n:type:ShaderForge.SFN_Multiply,id:8278,x:31365,y:31447,varname:_multiplier5,prsc:2|A-4563-OUT,B-2635-OUT;n:type:ShaderForge.SFN_Add,id:3769,x:31163,y:31323,varname:node_3769,prsc:2|A-1862-UVOUT,B-6328-OUT;n:type:ShaderForge.SFN_Add,id:4563,x:31163,y:31447,varname:node_4563,prsc:2|A-4410-UVOUT,B-6328-OUT;n:type:ShaderForge.SFN_Rotator,id:1862,x:30915,y:31195,varname:_rotator2,prsc:2|UVIN-4410-UVOUT,ANG-6681-OUT;n:type:ShaderForge.SFN_TexCoord,id:4410,x:30637,y:31235,varname:node_4410,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:6681,x:30637,y:31388,varname:node_6681,prsc:2,v1:1.5708;n:type:ShaderForge.SFN_Append,id:1327,x:30637,y:31457,varname:node_1327,prsc:2|A-7045-OUT,B-8230-OUT;n:type:ShaderForge.SFN_Divide,id:1391,x:30637,y:31587,varname:node_1391,prsc:2|A-4387-TSL,B-8517-OUT;n:type:ShaderForge.SFN_Multiply,id:6328,x:30834,y:31520,varname:_multiplier7,prsc:2|A-1327-OUT,B-1391-OUT;n:type:ShaderForge.SFN_Divide,id:7045,x:30429,y:31388,varname:node_7045,prsc:2|A-8815-OUT,B-2635-OUT;n:type:ShaderForge.SFN_Vector1,id:8230,x:30429,y:31520,varname:node_8230,prsc:2,v1:0;n:type:ShaderForge.SFN_Time,id:4387,x:30429,y:31587,varname:_timer1,prsc:2;n:type:ShaderForge.SFN_Vector1,id:8517,x:30429,y:31779,varname:node_8517,prsc:2,v1:100;n:type:ShaderForge.SFN_ValueProperty,id:8815,x:30200,y:31388,ptovrint:False,ptlb:Foam Speed,ptin:_FoamSpeed,varname:_FoamSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:120;n:type:ShaderForge.SFN_ObjectScale,id:8069,x:30637,y:31747,varname:node_8069,prsc:2,rcp:False;n:type:ShaderForge.SFN_ComponentMask,id:3396,x:30834,y:31747,varname:node_3396,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-8069-XYZ;n:type:ShaderForge.SFN_Multiply,id:87,x:31005,y:31747,varname:node_87,prsc:2|A-3396-OUT,B-4184-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4184,x:30834,y:31917,ptovrint:False,ptlb:Foam Tiling,ptin:_FoamTiling,varname:_FoamTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Divide,id:5979,x:32409,y:33664,varname:_division1,prsc:2|A-4502-OUT,B-2715-OUT;n:type:ShaderForge.SFN_Vector1,id:2715,x:32167,y:33831,varname:node_2715,prsc:2,v1:1000;n:type:ShaderForge.SFN_Divide,id:2635,x:31182,y:31747,varname:_division2,prsc:2|A-87-OUT,B-9561-OUT;n:type:ShaderForge.SFN_Vector1,id:9561,x:31003,y:31909,varname:node_9561,prsc:2,v1:1000;proporder:3437-8712-7241-8178-5061-1830-511-56-2438-7795-5268-2084-2899-6415-369-6070-4722-1496-8822-4184-7338-8955-9405-6489-969-8815;pass:END;sub:END;*/

Shader "AQUAS/Mobile/Single-Colored Foamy" {
    Properties {
        [NoScaleOffset]_Normal ("Normal", 2D) = "white" {}
        _Tiling ("Tiling", Float ) = 1
        _DeepWaterColor ("Deep Water Color", Color) = (0,0.345098,0.7372549,1)
        _DepthTransparency ("Depth Transparency", Float ) = 1.5
        _ShoreFade ("Shore Fade", Float ) = 0.3
        _ShoreTransparency ("Shore Transparency", Float ) = 0.04
        [HideInInspector]_ReflectionTex ("Reflection Tex", 2D) = "white" {}
        [MaterialToggle] _UseReflections ("Enable Reflections", Float ) = 0.5
        _Reflectionintensity ("Reflection intensity", Range(0, 1)) = 0.5
        _Distortion ("Distortion", Range(0, 2)) = 0.3
        _Specular ("Specular", Float ) = 1
        _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Gloss ("Gloss", Float ) = 0.7
        _LightWrapping ("Light Wrapping", Float ) = 2
        _Refraction ("Refraction", Range(0, 1)) = 0.67
        _WaveSpeed ("Wave Speed", Float ) = 40
        _EmissiveColor ("Emissive Color", Color) = (0.5,0.5,0.5,1)
        _EmissionIntensity ("Emission Intensity", Float ) = 0
        [NoScaleOffset]_FoamTexture ("Foam Texture", 2D) = "white" {}
        _FoamTiling ("Foam Tiling", Float ) = 3
        _FoamBlend ("Foam Blend", Float ) = 0.15
        _FoamVisibility ("Foam Visibility", Range(0, 1)) = 0.3
        _FoamColor ("Foam Color", Color) = (0.7647059,0.7647059,0.7647059,1)
        _FoamIntensity ("Foam Intensity", Float ) = 5
        _FoamContrast ("Foam Contrast", Range(0, 0.5)) = 0.25
        _FoamSpeed ("Foam Speed", Float ) = 120
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
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Refraction;
            uniform float _WaveSpeed;
            uniform float _Distortion;
            uniform float4 _SpecularColor;
            uniform float _Tiling;
            uniform float4 _EmissiveColor;
            uniform float _EmissionIntensity;
            uniform float _ShoreFade;
            uniform float _ShoreTransparency;
            uniform float _FoamBlend;
            uniform float _FoamVisibility;
            uniform float4 _FoamColor;
            uniform float _FoamIntensity;
            uniform float _FoamContrast;
            uniform sampler2D _FoamTexture; uniform float4 _FoamTexture_ST;
            uniform float _FoamSpeed;
            uniform float _FoamTiling;
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
                float _rotator1_ang = 1.5708;
                float _rotator1_spd = 1.0;
                float _rotator1_cos = cos(_rotator1_spd*_rotator1_ang);
                float _rotator1_sin = sin(_rotator1_spd*_rotator1_ang);
                float2 _rotator1_piv = float2(0.5,0.5);
                float2 _rotator1 = (mul(i.uv0-_rotator1_piv,float2x2( _rotator1_cos, -_rotator1_sin, _rotator1_sin, _rotator1_cos))+_rotator1_piv);
                float2 _division1 = ((objScale.rb*_Tiling)/1000.0);
                float4 _timer2 = _Time + _TimeEditor;
                float3 _multiplier4 = (float3((_WaveSpeed/_division1),0.0)*(_timer2.r/100.0));
                float2 _multiplier2 = ((_rotator1+_multiplier4)*_division1);
                float4 _texture1 = tex2D(_Normal,TRANSFORM_TEX(_multiplier2, _Normal));
                float2 _multiplier3 = ((i.uv0+_multiplier4)*_division1);
                float4 _texture2 = tex2D(_Normal,TRANSFORM_TEX(_multiplier3, _Normal));
                float3 _subtractor1 = (_texture1.rgb-_texture2.rgb);
                float3 _lerp = lerp(float3(0,0,1),_subtractor1,_Refraction);
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
                float2 _componentMask = _subtractor1.rg;
                float2 _remap = ((i.screenPos.rg+(float2(_componentMask.r,_componentMask.g)*_Distortion))*0.5+0.5);
                float4 _ReflectionTex_var = tex2D(_ReflectionTex,TRANSFORM_TEX(_remap, _ReflectionTex));
                float _rotator2_ang = 1.5708;
                float _rotator2_spd = 1.0;
                float _rotator2_cos = cos(_rotator2_spd*_rotator2_ang);
                float _rotator2_sin = sin(_rotator2_spd*_rotator2_ang);
                float2 _rotator2_piv = float2(0.5,0.5);
                float2 _rotator2 = (mul(i.uv0-_rotator2_piv,float2x2( _rotator2_cos, -_rotator2_sin, _rotator2_sin, _rotator2_cos))+_rotator2_piv);
                float2 _division2 = ((objScale.rb*_FoamTiling)/1000.0);
                float4 _timer1 = _Time + _TimeEditor;
                float3 _multiplier7 = (float3((_FoamSpeed/_division2),0.0)*(_timer1.r/100.0));
                float2 _multiplier6 = ((_rotator2+_multiplier7)*_division2);
                float4 _texture3 = tex2D(_FoamTexture,TRANSFORM_TEX(_multiplier6, _FoamTexture));
                float2 _multiplier5 = ((i.uv0+_multiplier7)*_division2);
                float4 _texture4 = tex2D(_FoamTexture,TRANSFORM_TEX(_multiplier5, _FoamTexture));
                float _value = 0.0;
                float3 _multiplier8 = ((((_value + ( (dot((_texture3.rgb-_texture4.rgb),float3(0.3,0.59,0.11)) - _FoamContrast) * (1.0 - _value) ) / ((1.0 - _FoamContrast) - _FoamContrast))*_FoamColor.rgb)*(_FoamIntensity*(-1.0)))*(saturate((sceneZ-partZ)/_FoamBlend)*-1.0+1.0));
                float3 diffuseColor = lerp(lerp( _DeepWaterColor.rgb, lerp(_ReflectionTex_var.rgb,_DeepWaterColor.rgb,(1.0 - _Reflectionintensity)), _UseReflections ),(_multiplier8*_multiplier8),_FoamVisibility);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 _multiplier1 = (_EmissiveColor.rgb*_EmissionIntensity);
                float3 emissive = _multiplier1;
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
