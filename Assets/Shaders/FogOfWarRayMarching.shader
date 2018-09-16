Shader "Custom/Fog of War (Raymarching)"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _FogColor ("Fog Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _FogStart ("Fog Start", Range(0, 1)) = 0.0
        _FogEnd ("Fog End", Range(0, 1)) = 1.0
        _FogIntensity ("Fog Intensity", Float) = 1.0
        _ExtinctionCoef ("Extinction Coefficent", Float) = 0.5
        // TODO: make this a constant value
        _RayMarchSteps("Ray March Steps", Int) = 128
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        Cull Back
        ZWrite Off
        ZTest Always

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            
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
                float4 clipPos : TEXCOORD1;
                float3 interpolatedRay : TEXCOORD2;
                float4 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float4 _FogColor;

            float _FogStart;
            float _FogEnd;
            float _FogIntensity;
            float _ExtinctionCoef;
            int _RayMarchSteps;

            sampler2D _CameraDepthTexture;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.clipPos = UnityObjectToClipPos(v.vertex);
                o.interpolatedRay = -normalize(WorldSpaceViewDir(v.vertex));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float2 WorldToDensityMapUv(float3 worldPos)
            {
                float4 objectPos = mul(unity_WorldToObject, float4(worldPos, 1.0f));
                float2 uv = objectPos.xz + float2(0.5f, 0.5f);

                // uv = saturate(uv);

                #if UNITY_UV_STARTS_AT_TOP
                    uv.v = 1.0f - uv.v;
                #endif
                return uv;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float2 screenUv = i.clipPos.xy / i.clipPos.w / 2.0f + float2(0.5f, 0.5f);
                // float4 maskColor = float4(screenUv, 0.0f, 1.0f);
                #if UNITY_UV_STARTS_AT_TOP
                    screenUv.v = 1.0f - screenUv.v;
                #endif

                float4 maskColor = float4(screenUv, 0.0f, 1.0f);
                // sample the depth texture
                float normalizedDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUv));
                float linearDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUv));

                // Linear Fog:
                float fogIntensity = (normalizedDepth - _FogStart) / (_FogEnd - _FogStart);
                fogIntensity = saturate(fogIntensity * _FogIntensity);
                maskColor = float4(_FogColor.rgb, fogIntensity);

                // Exponential Fog:
                // float fogIntensity = exp(-_FogIntensity * (1.0 - normalizedDepth));

                // Ray Marching
                float3 worldEndPos = _WorldSpaceCameraPos + linearDepth * i.interpolatedRay;
                float layerThickness = normalizedDepth / _RayMarchSteps;

                float2 startDensityUv = WorldToDensityMapUv(i.worldPos);
                float2 endDensityUv = WorldToDensityMapUv(worldEndPos);
                float2 currentDensityUv = startDensityUv;

                // maskColor = float4(endDensityUv, 0.0f, 1.0f);
                float transmittance = 1.0f;
                float2 densityUvStep = (endDensityUv - startDensityUv) / _RayMarchSteps;

                for(int marchIndex = 0; marchIndex < _RayMarchSteps; marchIndex++)
                {
                    float density = 1.0f - tex2D(_MainTex, currentDensityUv).r;
                    float extinction = _ExtinctionCoef * density;
                    transmittance *= exp(-extinction * layerThickness);

                    currentDensityUv += densityUvStep;
                }
                transmittance = saturate(transmittance);

                maskColor = float4(_FogColor.rgb, 1.0f - transmittance);
                return maskColor;
            }
            ENDCG
        }
    }
}
