Shader "Custom/CircularRainbowPulse_URP"
{
    Properties
    {
        _Speed("Base Flow Speed", Float) = 1.0
        _Frequency("Wave Frequency", Float) = 8.0
        _WaveAmplitude("Wave Amplitude", Float) = 0.25
        _LineSharpness("Line Sharpness", Float) = 25.0
        _Alpha("Alpha", Range(0,1)) = 0.8
        _Intensity("Base Intensity", Range(0,3)) = 1.5
        _BeatPulse("Beat Pulse", Float) = 0
        _PulseStrength("Pulse Strength", Range(0,5)) = 1.0
        _FlowPulseStrength("Flow Pulse Strength", Range(0,5)) = 1.0
        _EdgeGlow("Edge Glow Strength", Range(0,5)) = 2.0
        _EdgeRadius("Edge Radius", Range(0,1)) = 0.9
        _EdgeSoftness("Edge Softness", Range(0.001,0.5)) = 0.1

        // Nuevo parámetro para activar color o gris
        _ActiveState("Active State (0 = Gray, 1 = Color)", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Unlit"
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

            float _Speed;
            float _Frequency;
            float _WaveAmplitude;
            float _LineSharpness;
            float _Alpha;
            float _Intensity;
            float _BeatPulse;
            float _PulseStrength;
            float _FlowPulseStrength;
            float _EdgeGlow;
            float _EdgeRadius;
            float _EdgeSoftness;
            float _ActiveState;

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1,311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                float2 u = f*f*(3.0-2.0*f);
                return lerp(a, b, u.x) + (c - a)*u.y*(1.0 - u.x) + (d - b)*u.x*u.y;
            }

            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
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
                float2 uv = IN.uv - 0.5; // centrar
                float r = length(uv) * 2.0; // radio
                float angle = atan2(uv.y, uv.x);

                // Variación dinámica de flujo con el beat
                float dynamicSpeed = _Speed * (1.0 + _BeatPulse * _FlowPulseStrength);
                float t = _Time.y * dynamicSpeed;

                // Ondas radiales que se abren y ramifican
                float wave = sin(r * _Frequency - t + noise(uv * 5.0 + t) * 2.0);
                float branch = sin(r * (_Frequency * 0.6) + angle * 4.0 + t * 0.8);
                float combined = wave + branch * 0.5;

                // Definir líneas y patrones circulares
                float linePattern = abs(sin((r + combined * _WaveAmplitude) * 3.14159 * _Frequency));
                float lines = exp(-_LineSharpness * linePattern);

                // Borde circular brillante
                float edgeMask = smoothstep(_EdgeRadius, _EdgeRadius - _EdgeSoftness, r);
                float edgePulse = 1.0 + (_BeatPulse * _PulseStrength);
                float edgeGlow = edgeMask * _EdgeGlow * edgePulse;

                // Pulso global
                float pulse = 1.0 + (_BeatPulse * _PulseStrength);
                float brightness = (lines + edgeGlow) * _Intensity * pulse;

                // Arcoíris circular animado
                float hue = frac(angle / 6.2831 + t * 0.1);
                float sat = 1.0 - r * 0.3;
                float val = 0.8 + 0.2 * sin(t + r * 4.0);
                float3 rgb = HSVtoRGB(float3(hue, sat, val));

                // Convertir a gris
                float gray = dot(rgb, float3(0.3, 0.59, 0.11));

                // Interpolación: gris (0) → color (1)
                float3 color = lerp(float3(gray, gray, gray), rgb, _ActiveState);

                // Fade fuera del borde
                float fade = smoothstep(1.0, 0.8, r);

                return half4(color * brightness * fade, brightness * _Alpha * fade);
            }
            ENDHLSL
        }
    }
}
