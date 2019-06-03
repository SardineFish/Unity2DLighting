
sampler2D _ShadowMap;

#define SAMPLE_SHADOW_2D(uv) (1 - tex2D(_ShadowMap, (uv).xy))