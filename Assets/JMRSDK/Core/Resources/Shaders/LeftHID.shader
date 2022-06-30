// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "JMRSDK/LeftHID"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		ipdOffset("ipdOffset",Vector) = (0,0,0,0)
	}

	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Fog{ Mode off }
			
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			fixed4 ipdOffset;

			float4 frag(v2f i) : SV_Target
			{
				float2 disp = i.uv + float2(ipdOffset.x,ipdOffset.y);

				if (disp.x < 0 || disp.x > 1.0 || disp.y <0 || disp.y > 1.0) 
                   return fixed4(0., 0., 0., 1.);

				return tex2D(_MainTex, disp);
			}
			ENDCG
		}
	}
}
