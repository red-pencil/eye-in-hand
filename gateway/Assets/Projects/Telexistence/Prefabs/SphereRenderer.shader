﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright (c) 2015 Nora
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php
Shader "Theta/Renderer"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Overlay" "Queue" = "Overlay" "ForceNoShadowCasting" = "True" }

		ZTest Always
		Cull Off
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv.y=1-o.uv.y;
				return o;
			}
			
			half4 frag(v2f i) : SV_Target
			{ 
				half4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
