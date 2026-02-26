Shader "Custom/CurvedWaveLines_URP"
{
    Properties
    {
        _LineCount("Line Count", Float) = 18
        _LineThickness("Line Thickness", Float) = 0.006

        _WaveAmplitude("Wave Amplitude", Float) = 0.035
        _WaveFrequency("Wave Frequency", Float) = 9
        _WaveSpeed("Wave Speed", Float) = 1.2

        _Randomness("Randomness", Float) = 0.6

        _BeatPulse("Beat Pulse", Float) = 0
        _PulseStrength("Pulse Strength", Float) = 1

        _Alpha("Alpha", Range(0,1)) = 1
        _BackgroundStrength("Background Strength", Range(0,1)) = 0.5
        _EdgeFade("Edge Fade Power", Float) = 1.5
        _ActiveState("Active (0 Gray/1 Color)", Range(0,1)) = 1
        
        // Nueva propiedad para elegir el fondo
        _RainbowColor("Rainbow Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

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

            float _LineCount;
            float _LineThickness;

            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _Randomness;

            float _BeatPulse;
            float _PulseStrength;

            float _Alpha;
            float _BackgroundStrength;
            float _EdgeFade;
            float _ActiveState;

            // Nuevo color del rainbow
            float4 _RainbowColor;

            float hash(float n) { return frac(sin(n) * 43758.5453123); }

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
                float2 uv = IN.uv;
                float t = _Time.y;

                // Usamos el color rainbow pasado por el script solo si el jugador está dentro
                float3 bgColor = _RainbowColor.rgb;

                // ===== Fade en los 4 bordes =====
                float distX = abs(uv.x - 0.5) * 2.0;
                float distY = abs(uv.y - 0.5) * 2.0;

                float fadeX = 1.0 - distX;
                float fadeY = 1.0 - distY;

                float edgeFade = pow(saturate(fadeX * fadeY), _EdgeFade);

                // Fondo final con intensidad de color y fade
                float3 background = bgColor * _BackgroundStrength * edgeFade * (1.0 + _BeatPulse * 0.2);

                // ===== Líneas =====
                float lineIntensity = 0;

                for (int i = 0; i < 32; i++)
                {
                    if (i >= _LineCount) break;

                    float lineID = i;
                    float baseX = (lineID + 0.5) / _LineCount;

                    float phase = hash(lineID) * 6.2831853 * _Randomness;
                    float speed = lerp(0.8, 1.8, hash(lineID + 5.7));
                    float offset = sin(uv.y * _WaveFrequency + t * _WaveSpeed * speed + phase) * _WaveAmplitude;

                    float distortedX = baseX + offset;
                    float dist = abs(uv.x - distortedX);

                    float thickness = _LineThickness * (1.0 + _BeatPulse * 0.25);
                    float lineVal = 1.0 - smoothstep(0.0, thickness, dist);

                    lineVal *= 1.0 + _BeatPulse * _PulseStrength;
                    lineIntensity += lineVal;
                }

                float3 finalColor = lerp(background, float3(1,1,1), lineIntensity);
                float finalAlpha = saturate((_BackgroundStrength * edgeFade + lineIntensity) * _Alpha);

                return half4(finalColor, finalAlpha);
            }

            ENDHLSL
        }
    }
}