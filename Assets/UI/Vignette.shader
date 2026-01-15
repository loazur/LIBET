Shader "UI/Vignette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}   // ← OBLIGATOIRE POUR UI

        _Color ("Vignette Color", Color) = (0,0,0,1)
        _Intensity ("Intensity", Range(0,1)) = 0.5
        _Smoothness ("Smoothness", Range(0.1,2)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Intensity;
            float _Smoothness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 p = i.uv - 0.5;
                float d = length(p) * _Smoothness;

                float vignette = smoothstep(0.4, 0.9, d);

                return float4(_Color.rgb, vignette * _Intensity);
            }
            ENDHLSL
        }
    }
}
