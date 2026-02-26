Shader "Custom/DanceZoneSplashCurvado_URP"
{
    Properties
    {
        _LineCount("Line Count", Int) = 3
        _LineSharpness("Line Sharpness", Float) = 60
        _DeformAmplitude("Splash Amplitude", Float) = 0.05
        _DeformFrequency("Splash Frequency", Float) = 5
        _DeformSpeed("Splash Speed", Float) = 3

        _BackgroundStrength("Background Strength", Range(0,2)) = 0.6
        _BackgroundFade("Background Fade", Float) = 2.5

        _Alpha("Alpha", Range(0,1)) = 0.9
        _BeatPulse("Beat Pulse", Float) = 0
        _PulseStrength("Pulse Strength", Float) = 1

        _ActiveState("Active (0 Gray / 1 Color)", Range(0,1)) = 0
        _Shape("Shape (0 Oval / 1 Rect)", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            int _LineCount;
            float _LineSharpness;
            float _DeformAmplitude;
            float _DeformFrequency;
            float _DeformSpeed;

            float _BackgroundStrength;
            float _BackgroundFade;

            float _Alpha;
            float _BeatPulse;
            float _PulseStrength;
            float _ActiveState;
            float _Shape; // 0 = oval, 1 = rect

            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1., 2./3., 1./3., 3.);
                float3 p = abs(frac(c.xxx + K.xyz) * 6. - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv * 2 - 1; // Transformar a [-1,1]
                float t = _Time.y;

                // Background semirectangular y suave
                float2 fadeUV = abs(uv);
                float bgFade = pow(1.0 - fadeUV.x, _BackgroundFade) * pow(1.0 - fadeUV.y, _BackgroundFade);
                float3 bgRainbow = HSVtoRGB(float3(frac(atan2(uv.y, uv.x)/6.2831), 1, 1));
                float gray = dot(bgRainbow, float3(0.3,0.59,0.11));
                float3 bgColor = lerp(float3(gray,gray,gray), bgRainbow, _ActiveState);
                float3 background = bgColor * _BackgroundStrength * bgFade;

                // Líneas blancas con ondulación tipo splash en el perímetro (borde)
                float lineIntensity = 0;

                for (int i = 0; i < 10; i++)
                {
                    if (i >= _LineCount) break;

                    float lineOffset = i / (float)_LineCount; // Distribución uniforme
                    float baseRadius = 0.3 + lineOffset * 0.6;

                    // Curvatura del perímetro (deformación sinusoidal solo en el borde)
                    float deformX = sin(uv.y * _DeformFrequency + t * _DeformSpeed + i * 10) * _DeformAmplitude;
                    float deformY = cos(uv.x * _DeformFrequency + t * _DeformSpeed + i * 15) * _DeformAmplitude;
                    float2 deformedUV = uv + float2(deformX, deformY);

                    // Mantener el centro fijo, deformar solo el perímetro
                    float distOval = length(deformedUV); // Óvalo
                    float distRect = max(abs(deformedUV.x), abs(deformedUV.y)); // Rectángulo
                    float dist = lerp(distOval, distRect, _Shape); // Interpolamos entre óvalo y rectángulo

                    // Calcular la diferencia de distancias y dibujar el perímetro
                    float ringPattern = abs(dist - baseRadius);
                    float lineVal = exp(-_LineSharpness * ringPattern);
                    lineVal *= 1.0 + _BeatPulse * _PulseStrength;

                    lineIntensity += lineVal;
                }

                float3 finalColor = background + float3(1,1,1) * lineIntensity;
                float finalAlpha = saturate((_BackgroundStrength * bgFade + lineIntensity) * _Alpha);

                return half4(finalColor, finalAlpha);
            }

            ENDHLSL
        }
    }
}