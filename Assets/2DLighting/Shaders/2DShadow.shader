// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lighting2D/Shadow"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
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
		Blend One OneMinusSrcAlpha

        // #0 Hard shadow
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};

            sampler2D _MainTex;

			v2f vert(appdata_base IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
                float3 color = float3(1, 1, 1);
				return fixed4(color, 1.0);
			}
		ENDCG
		}

		// Graph
		// https://www.geogebra.org/graphing/ysvegxsz 
		// #1 Volumn light shadow
		Pass
		{
			BlendOp Add
			Blend One One

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#define PI 3.14159265358979323846264338327950288419716939937510

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 edgeA : TEXCOORD0;
				float2 edgeB : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 edgeA : TEXCOORD0;
				float2 edgeB : TEXCOORD1;
				float2 pos: TEXCOORD2;
			};

            sampler2D _MainTex;
			float _LightSize;


			v2f vert(appdata_t i)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.edgeA = i.edgeA;
				o.edgeB = i.edgeB;
				o.pos = i.vertex;
				return o;
			}

			inline float2 rotate(float2 v, float ang)
			{
				float2 r = float2(cos(ang), sin(ang));
// return new Vector2(rx * v.x - ry * v.y, rx * v.y + ry * v.x);
				return float2(r.x * v.x - r.y * v.y, r.x * v.y + r.y * v.x);
			}

			inline float cross2(float2 u, float2 v)
			{
				return cross(float3(u, 0), float3(v, 0)).z;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float d = distance(float2(0, 0), i.pos);
				float ang = asin(_LightSize / d);
				float2 left = normalize(rotate(-i.pos, ang));
				float2 right = normalize(rotate(-i.pos, -ang));
				float2 u = normalize(i.edgeA.xy - i.pos);
				float2 v = normalize(i.edgeB.xy - i.pos);
				if (cross2(v, u) < 0)
				{
					float2 t = u;
					u = v;
					v = t;
				}
				float leftLeak = saturate(sign(cross2(u, left))) *  acos(dot(left, u));
				float rightLeak = saturate(sign(cross2(right, v))) * acos(dot(right, v));
				float total = acos(dot(right, left));

				float3 color = saturate((leftLeak + rightLeak) / total);

				return fixed4(1 - color, 1.0);
			}
		ENDCG
		}
	}
}