Shader "Custom/Radial Falloff"
{
    Properties
    {
        _FalloffStart ("Falloff Start", Range(0, 1)) = 0
        _FalloffEnd ("Falloff End", Range(0, 1)) = 1
        _Intensity ("Intensity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Blend One One
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
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _FalloffStart;
            float _FalloffEnd;
            float _Intensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float distToCenter = distance(i.uv, float2(0.5f, 0.5f)) * 2.0f;

                // I(d) = I0 / (d ^ 2)
                // float atten = _Intensity / (distToCenter * distToCenter);

                float atten = saturate((_FalloffEnd - distToCenter) / (_FalloffEnd - _FalloffStart));
                atten = atten * atten;
                // float4 maskColor = float4(1.0f, 1.0f, 1.0f, atten * _Intensity);
                float4 maskColor = float4(atten * _Intensity, atten * _Intensity, atten * _Intensity, 1.0f);

                return maskColor;
            }
            ENDCG
        }
    }
}
