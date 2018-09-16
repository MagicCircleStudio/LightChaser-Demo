Shader "Custom/FogOfWarMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _TintColor ("Tint Color", Color) = (0.0, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Blend DstColor Zero
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 maskColor = tex2D(_MainTex, i.uv);
                maskColor += _TintColor;
                // apply fog
                // fixed4 col = _TintColor;
                // maskColor.a -= maskColor.r;
                
                return maskColor;
            }
            ENDCG
        }
    }
}
