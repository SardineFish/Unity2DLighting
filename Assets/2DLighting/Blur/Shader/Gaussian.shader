Shader "GaussianBlur/Blur"
{
	Properties
	{
		_MainTex("LightTexture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Gaussian("Gaussian", 2D) = "white" {}
	}

	SubShader
	{
        Tags { 
            "RenderType"="Opaque"
			"PreviewType"="Plane" 
        }
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
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _Gaussian;
            float4 _Gaussian_TexelSize;
            int _BlurRadius;
            float2 _BlurDirection;

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

            inline float gaussianWeight(int2 radius)
            {
                return tex2D(_Gaussian, radius.xy * _Gaussian_TexelSize.xy).r;
            }

            inline float3 gaussian(float2 uv, int r, int R)
            {
                if (r == 0)
                    return gaussianWeight(int2(r, R)) * tex2D(_MainTex, uv.xy);
                return gaussianWeight(int2(r, R)) * (
                    tex2D(_MainTex, uv.xy + float2(r, r) * _BlurDirection * _MainTex_TexelSize.xy) + 
                    tex2D(_MainTex, uv.xy - float2(r, r) * _BlurDirection * _MainTex_TexelSize.xy)
                );
            }

			fixed4 frag(v2f v) : SV_Target
			{
                float3 color;
                for(int i = 0; i < _BlurRadius; i++)
                {
                    color += gaussian(v.texcoord.xy, i, _BlurRadius - 1);
                }
				return fixed4(color, 1.0);
			}
		ENDCG
		}
	}
}