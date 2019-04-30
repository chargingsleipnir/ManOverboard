Shader "Unlit/LineZOff"
{
	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			Fog { Mode Off }
			BindChannels
			{
				Bind "vertex", vertex
				Bind "color", color
			}
		}
	}
}