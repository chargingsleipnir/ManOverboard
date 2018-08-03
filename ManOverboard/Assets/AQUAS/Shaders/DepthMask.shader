Shader "AQUAS/Volume Mask" {

	SubShader{
		Tags{ "Queue" = "Geometry" }
		ColorMask 0
		ZWrite On
		Pass{Cull Off}
	}
}