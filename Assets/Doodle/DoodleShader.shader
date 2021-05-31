Shader "Doodle"
{
	Properties
	{
		_Color("Tint", Color) = (0, 0, 0, 1)

		[Space(15)] _MainTex("Texture", 2D) = "white" {}

		[Space(15)] _MaxOffset("Max Offset", vector) = (0.005, 0.005, 0, 0)

		[Space(15)] _FrameTime("Frame Time", Float) = 0.24

		[Space(15)] _FrameCount("Frame Count", Int) = 24

		[Space(15)] _NoiseScale("Noise Scale", vector) = (35, 35, 1, 1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"

			#include "DoodleUtil.cginc" 

			#pragma vertex vert

			#pragma fragment frag

			sampler2D _MainTex;

			float4 _MainTex_ST;

			fixed4 _Color;

			struct appdata 
			{
				float4 vertex : POSITION;

				float2 uv : TEXCOORD0;
			};

			struct v2f 
			{
				float4 position : SV_POSITION;

				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) 
			{
				v2f o;

				o.position = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			float2 _MaxOffset;

			float _FrameTime;

			int _FrameCount;

			float2 _NoiseScale;

			fixed4 frag(v2f i) : SV_TARGET
			{
				float2 offset = 0.0;

				offset = DoodleTextureOffset(i.uv, _MaxOffset, _Time.y, _FrameTime, _FrameCount,_NoiseScale); 

				fixed4 col = tex2D(_MainTex, i.uv + offset);

				col *= _Color;

				return col;
			}

			ENDCG
		}
	}
}