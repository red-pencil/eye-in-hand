// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Leon/EyeBlender" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Strength ("",Range (0.0, 1.0))= 0.5 
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Pass{
			LOD 200
			
			Lighting Off
			ZWrite Off
			Cull Off
			Fog { Mode off }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			sampler2D _MainTex;
			half _Focus;
			half _Size;
			half _flip;

			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			};
			v2f vert(appdata_base  v) {
			    v2f o;
			    o.pos = UnityObjectToClipPos(v.vertex);
			    o.uv = v.texcoord;
			    return o;
			}

			float Eval(float x)
			{
				float a=smoothstep(_Focus-_Size,_Focus+_Size,x);

				return clamp(a,0,1);
			}

			half4 frag(v2f_img IN) : SV_Target {
				float4 c;

				c.rgb = tex2D (_MainTex, IN.uv).rgb;
				if(_flip<0.5f)
					c.a=Eval(IN.uv.x);
				else 
					c.a=1-Eval(IN.uv.x);	
				return c;
			}

	        ENDCG
		}

	} 
}
