Shader "Image/FlipCoord_Hor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		TextureRect ("Texture Clip Rect",Vector) = (0,0,1,1)
	}
	SubShader {
		Pass{
			ZTest Always Cull Off ZWrite Off
			Fog{ Mode off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;


			float4 frag(v2f_img IN) :COLOR{
				return float4(tex2D(_MainTex, float2(1- IN.uv.x,IN.uv.y)).rgb,1);
			}
			ENDCG
		}
	} 
}
