﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Telexistence/Demo/WebCamShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass{
			LOD 200
			
			Lighting Off
			ZTest On
			ZWrite Off
			Cull Off
			Fog { Mode off }

				// Only render pixels whose value in the stencil buffer equals 1.
			Stencil {
			  Ref 0
			  Comp Equal
			}
			CGPROGRAM
			
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			sampler2D _MainTex;


			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			};
			v2f vert(appdata_base  v) {
			    v2f o;
			    o.pos = UnityObjectToClipPos(v.vertex);
			    o.uv = v.texcoord;
				//o.uv.y = 1 - o.uv.y;
			    return o;
			}
			half4 frag(v2f_img IN) : SV_Target {
				float4 c;
				c.rgb = tex2D (_MainTex, IN.uv).rgb;
				c.a=1;
				return c;
			}

	        ENDCG
		}
	} 
}
