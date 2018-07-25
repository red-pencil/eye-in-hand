// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Telexistence/Demo/ImageInterpolate" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		Pass{
			LOD 200

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Off
			Fog { Mode Off }
			Offset -1, -1
			//Blend SrcAlpha DstAlpha
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			sampler2D _MainTex;
			sampler2D _MainTex2;
			float Interpolation;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};
			v2f vert(appdata_t  v) {
			    v2f o;
			    o.pos = UnityObjectToClipPos(v.vertex);
			    o.uv = v.texcoord;
			    o.color=v.color;
			    return o;
			}
			half4 frag(v2f IN) : SV_Target {
				float4 c;
				c= lerp(tex2D (_MainTex, IN.uv),tex2D (_MainTex2, IN.uv),Interpolation)*IN.color;
				return c;
			}

	        ENDCG
		}
	} 
}
