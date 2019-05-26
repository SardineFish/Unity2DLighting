// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lighting2D/DeferredLighting"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_TestTex ("Test Texture", 2D) = "white" {}
	}

	// #0 Deferred lighting
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
		Blend One Zero

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
			float4 _MainTex_TexelSize;
			sampler2D _LightMap;

			float _ExposureLimit;

			int _UseMSAA;
			int _SceneView;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.texcoord;
				
			#if SHADER_API_D3D11
				if(!_SceneView && !_UseMSAA)
				 	uv.y = 1 - uv.y;
			#endif

				float3 ambient = UNITY_LIGHTMODEL_AMBIENT;
				float3 light = ambient + tex2D(_LightMap, i.texcoord).rgb;
				if(_ExposureLimit >= 0)
					light = clamp(light, 0, _ExposureLimit);
                float3 color = light * tex2D(_MainTex, uv).rgb;
				return fixed4(color, 1.0);
			}
		ENDCG
		}

		// #1 Gaussian blur
		Pass {
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
			float2 _MainTex_TexelSize;

			v2f vert(appdata_base IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			inline float3 filter(float2 uv, float2 d)
			{
				return tex2D(_MainTex, uv + d.xy * _MainTex_TexelSize.x).rgb 
					+ tex2D(_MainTex, uv - d.xy * _MainTex_TexelSize.x).rgb 
					+ tex2D(_MainTex, uv + d.yx * _MainTex_TexelSize.y).rgb
					+ tex2D(_MainTex, uv - d.yx * _MainTex_TexelSize.y).rgb;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 uv = IN.texcoord;
				float3 col = 0.29234 * tex2D(_MainTex, uv).rgb;
				col += 0.111768 * filter(uv, float2(1, 0));
				col += 0.0499491 * filter(uv, float2(2, 0));
				col += 0.013032 * filter(uv, float2(3, 0));
				col += 0.00198168 * filter(uv, float2(4, 0));
				return fixed4(col, 1.0);
			}
		ENDCG
		}

		// #2 Blit Copy
		Pass{
            ZTest Always Cull Off ZWrite Off
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler _MainTex;
            uniform float4 _MainTex_ST;
			float2 _MainTex_TexelSize;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.texcoord);
            }

		ENDCG
		}
	}
}