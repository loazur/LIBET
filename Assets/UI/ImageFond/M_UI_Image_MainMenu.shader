Shader "URP/OrganicGlobalBlur"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}

        _NoiseScale ("Noise Scale", Float) = 2.0
        _NoiseSpeed ("Noise Speed", Float) = 0.1
        _Distortion ("Distortion Strength", Float) = 0.05

        _BlurRadius ("Blur Radius", Float) = 0.02
        _BlurSamples ("Blur Samples", Range(4,32)) = 16

        _ColorA ("Color A", Color) = (0.1,0.2,0.6,1)
        _ColorB ("Color B", Color) = (0.9,0.9,1,1)
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

            float _NoiseScale;
            float _NoiseSpeed;
            float _Distortion;
            float _BlurRadius;
            int _BlurSamples;
            float4 _ColorA;
            float4 _ColorB;

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

                return lerp(a,b,u.x) +
                       (c-a)*u.y*(1.0-u.x) +
                       (d-b)*u.x*u.y;
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            // Floute le noise en échantillonnant plusieurs positions voisines
            float blurredNoise(float2 p)
            {
                float total = 0;
                float weightSum = 0;
                
                for (int i = 0; i < _BlurSamples; i++)
                {
                    float a = (i / (float)_BlurSamples) * 6.2831853 * 3.0; // Plus de rotations
                    float r = (i + 0.5) / _BlurSamples; // Distribution linéaire (plus étalée)
                    
                    float2 offset = float2(cos(a), sin(a)) * r * _BlurRadius * 50.0;
                    float w = exp(-r * r * 2.0); // Poids plus doux
                    
                    total += noise(p + offset) * w;
                    weightSum += w;
                }
                
                return total / weightSum;
            }

            // Floute la texture de base
            float4 blurTexture(float2 uv)
            {
                float4 col = 0;
                float weightSum = 0;

                for (int i = 0; i < _BlurSamples; i++)
                {
                    float a = (i / (float)_BlurSamples) * 6.2831853;
                    float r = sqrt((i + 0.5) / _BlurSamples);

                    float2 offset = float2(cos(a), sin(a)) * r * _BlurRadius;
                    float w = exp(-r * r * 4.0);

                    col += tex2D(_MainTex, uv + offset) * w;
                    weightSum += w;
                }

                return col / weightSum;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 centered = uv - 0.5;

                // Noise flouté pour un effet plus doux
                float2 noiseUV = centered * _NoiseScale + _Time.y * _NoiseSpeed;
                float n = blurredNoise(noiseUV);

                float2 flow = normalize(centered + 0.0001);
                uv += flow * n * _Distortion;

                // Texture de base floutée
                float4 texColor = blurTexture(uv);

                float t = saturate(texColor.r);
                float3 color = lerp(_ColorA.rgb, _ColorB.rgb, t);

                return float4(color, texColor.a);
            }

            ENDHLSL
        }
    }
}
