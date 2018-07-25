// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Telexistence/Demo/Teleport_Doors" {
	SubShader {
	Tags {  "RenderType"="Opaque"  "Queue" = "Geometry-1" }  // Write to the stencil buffer before drawing any geometry to the screen
	Pass{
        Stencil {
            Ref 1
            Comp always
            Pass replace
            ZFail decrWrap
        }
        
		ColorMask 0 // Don't write to any colour channels
		ZWrite Off // Don't write to the Depth buffer
		Cull Off
		Lighting Off
		Fog { Mode off }
		
		
	    CGPROGRAM
	    #pragma vertex vert
	    #pragma fragment frag
	    struct appdata {
	        float4 vertex : POSITION;
	    };
	    struct v2f {
	        float4 pos : SV_POSITION;
	    };
	    v2f vert(appdata v) {
	        v2f o;
	        o.pos = UnityObjectToClipPos(v.vertex);
	        return o;
	    }
	    half4 frag(v2f i) : SV_Target {
	        return half4(1,0,0,1);
	    }
	    ENDCG
	   }
	}
}


