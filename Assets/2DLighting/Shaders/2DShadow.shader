// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lighting2D/Shadow"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
	}

	// #0 Mix light map
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

			sampler2D _Diffuse;
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
                float3 color = float3(0, 0, 0);
				return fixed4(color, 1.0);
			}
		ENDCG
		}
	}
}