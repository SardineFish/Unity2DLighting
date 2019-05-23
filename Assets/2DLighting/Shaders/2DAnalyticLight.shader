// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lighting2D/AnalyticLight"
{
	Properties
	{
        _Color("Color", Color) = (1,1,1,1)
		_Scale("Scale", Range(0, 1)) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One One

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
                float4 worldPos:TEXCOORD1;
				float4 shadowUV: TEXCOORD2;
			};
			
			fixed4 _Color;

			float4 _2DLightPos;
            float _LightRange;
            float _Intensity;
			float _Scale;
			sampler2D _ShadowMap;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color * _Color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.shadowUV = ComputeGrabScreenPos(o.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float scale = 1 / _Scale - 1;
                float d = distance(i.texcoord, float2(.5, .5));
				d /= .5;
                d = saturate(d);
                float3 illum = exp(-d * scale) - exp(-scale) * d;
				illum *= _Intensity * _Color;
                float3 color = illum;
				i.shadowUV.xy /= i.shadowUV.w;
				color = color * tex2D(_ShadowMap, i.shadowUV).rgb;
				return fixed4(color, 1.0);
			}
		ENDCG
		}
	}
}