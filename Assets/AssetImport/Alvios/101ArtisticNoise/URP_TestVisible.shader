Shader "URP/SpiralOrganicArtistic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // Mouvement
        _SpiralStrength ("Spiral Strength", Float) = 2.0
        _NoiseScale ("Noise Scale", Float) = 4.0
        _NoiseSpeed ("Noise Speed", Float) = 0.3
        _DistortionAmount ("Distortion Amount", Float) = 0.05

        // Couleurs
        _ColorA ("Color A", Color) = (0.2,0.3,0.8,1)
        _ColorB ("Color B", Color) = (0.9,0.4,0.6,1)
        _ColorIntensity ("Color Intensity", Float) = 1.0

        // Flou
        _BlurAmount ("Blur Amount", Float) = 0.003

        // Artistique
        _Contrast ("Contrast", Float) = 1.2
        _Vignette ("Vignette", Float) = 0.6
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _SpiralStrength;
            float _NoiseScale;
            float _NoiseSpeed;
            float _DistortionAmount;

            float4 _ColorA;
            float4 _ColorB;
            float _ColorIntensity;

            float _BlurAmount;
            float _Contrast;
            float _Vignette;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1,311.7))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));
                float2 u = f*f*(3.0-2.0*f);
                return lerp(a,b,u.x) + (c-a)*u.y*(1.0-u.x) + (d-b)*u.x*u.y;
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float4 SampleBlur(sampler2D tex, float2 uv, float amount)
            {
                float4 c = tex2D(tex, uv);
                c += tex2D(tex, uv + float2(amount,0));
                c += tex2D(tex, uv + float2(-amount,0));
                c += tex2D(tex, uv + float2(0,amount));
                c += tex2D(tex, uv + float2(0,-amount));
                return c / 5.0;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 centered = uv - 0.5;

                float radius = length(centered);
                float angle = atan2(centered.y, centered.x);

                float time = _Time.y;

                angle += radius * _SpiralStrength + time * 0.5;

                float n = noise(float2(radius * _NoiseScale, angle + time * _NoiseSpeed));

                float2 distortion = float2(cos(angle), sin(angle)) * n;
                uv += distortion * _DistortionAmount;

                // Flou
                float4 tex = SampleBlur(_MainTex, uv, _BlurAmount);

                // Couleur procédurale
                float t = saturate(n * _ColorIntensity);
                float4 col = lerp(_ColorA, _ColorB, t);
                tex.rgb *= col.rgb;

                // Contraste
                tex.rgb = pow(tex.rgb, 1.0 / _Contrast);

                // Vignette
                float vignette = smoothstep(0.8, _Vignette, radius);
                tex.rgb *= vignette;

                return tex;
            }
            ENDHLSL
        }
    }
}
