Shader "Danzombies/ZombieZoneShader"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}

        _LineCount("Line Count", Float) = 18
        _LineThickness("Line Thickness", Float) = 0.006

        _WaveAmplitude("Wave Amplitude", Float) = 0.035
        _WaveFrequency("Wave Frequency (Integer Recommended)", Float) = 9
        _WaveSpeed("Wave Speed", Float) = 1.2

        _Randomness("Randomness", Float) = 0.6

        _BeatPulse("Beat Pulse", Float) = 0
        _PulseStrength("Pulse Strength", Float) = 1

        _Alpha("Alpha", Range(0,1)) = 1
        _BackgroundStrength("Background Strength", Range(0,1)) = 0.5
        _ActiveState("Active (0 Inactive / 1 Active)", Range(0,1)) = 1
        
        _RainbowColor("Active Color", Color) = (1,1,1,1)
        _InactiveColor("Inactive Color", Color) = (0.4,0.4,0.4,1)

        _Tiling("Tiling XY", Vector) = (1,1,0,0)

        _UseSpriteColor("Use Sprite Color", Range(0,1)) = 0
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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

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
            float _ActiveState;

            float4 _RainbowColor;
            float4 _InactiveColor;
            float4 _Tiling;

            float _UseSpriteColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // SAMPLE DEL SPRITE
                float4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // RECORTE POR ALPHA (clave)
                clip(sprite.a - 0.01);

                float2 uv = IN.uv * _Tiling.xy;
                uv = frac(uv);

                float t = _Time.y;

                // ===== COLOR BASE =====
                float3 activeColor = _RainbowColor.rgb;
                float3 inactiveColor = _InactiveColor.rgb;
                float3 baseColor = lerp(inactiveColor, activeColor, _ActiveState);

                float3 background = baseColor * _BackgroundStrength;

                // ===== LÍNEAS =====
                float lineIntensity = 0;

                float wave = sin(uv.y * _WaveFrequency * 6.2831853 + t * _WaveSpeed);
                float offset = wave * _WaveAmplitude;

                for (int i = 0; i < 32; i++)
                {
                    if (i >= _LineCount) break;

                    float baseX = (i + 0.5) / _LineCount;
                    float distortedX = baseX + offset;

                    float dist = abs(uv.x - distortedX);

                    float thickness = (_LineThickness / _Tiling.x) * (1.0 + _BeatPulse * 0.25);

                    float lineVal = 1.0 - smoothstep(0.0, thickness, dist);
                    lineVal *= 1.0 + _BeatPulse * _PulseStrength;

                    lineIntensity += lineVal;
                }

                lineIntensity = saturate(lineIntensity);

                float3 lineColor = float3(1.0, 1.0, 1.0);

                float3 finalColor = background * (1.0 - lineIntensity)
                                  + lineColor * lineIntensity;

                // OPCIONAL: mezclar con color del sprite
                if (_UseSpriteColor > 0.5)
                {
                    finalColor *= sprite.rgb;
                }

                float finalAlpha = saturate((_BackgroundStrength + lineIntensity) * _Alpha) * sprite.a;

                return half4(finalColor, finalAlpha);
            }

            ENDHLSL
        }
    }
}