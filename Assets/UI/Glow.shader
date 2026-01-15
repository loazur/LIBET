Shader "UI/DiffuseGlowImage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 1
        _GlowRadius ("Glow Radius", Range(0,0.05)) = 0.015
        _GlowSamples ("Glow Samples", Range(4,32)) = 16
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
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowRadius;
            int _GlowSamples;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv);

                float alpha = baseCol.a;
                float glow = 0;

                for (int s = 0; s < _GlowSamples; s++)
                {
                    float angle = 6.28318 * (s / (float)_GlowSamples);
                    float2 offset = float2(cos(angle), sin(angle)) * _GlowRadius;
                    glow += tex2D(_MainTex, i.uv + offset).a;
                }

                glow /= _GlowSamples;
                glow *= _GlowIntensity;

                fixed4 glowCol = _GlowColor;
                glowCol.a *= glow;

                fixed4 finalCol = baseCol + glowCol * (1 - alpha);
                return finalCol;
            }
            ENDCG
        }
    }
}
