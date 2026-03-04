Shader "Custom/RainbowSpritePainter_URP"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Speed ("Rainbow Speed", Float) = 1
        _Intensity ("Rainbow Intensity", Float) = 1
        [Toggle] _RainbowEnabled ("Enable Rainbow", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;
            float _Speed;
            float _Intensity;
            float _RainbowEnabled;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            float3 HSVtoRGB(float3 hsv)
            {
                float3 rgb = saturate(abs(frac(hsv.x + float3(0,2.0/3.0,1.0/3.0)) * 6.0 - 3.0) - 1.0);
                return hsv.z * lerp(float3(1,1,1), rgb, hsv.y);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                float vertical = IN.uv.y;
                float timeOffset = _Time.y * _Speed;

                // Flujo hacia abajo (invertido como lo pediste antes)
                float hue = frac(vertical - timeOffset);
                float3 rainbow = HSVtoRGB(float3(hue, 1, 1));

                // Color blanco cuando está desactivado
                float3 baseColor = float3(1,1,1);

                // Lerp según toggle (0 = blanco, 1 = rainbow)
                float3 finalEffect = lerp(baseColor, rainbow * _Intensity, _RainbowEnabled);

                float3 finalColor = sprite.rgb * finalEffect;

                return float4(finalColor, sprite.a) * IN.color;
            }

            ENDHLSL
        }
    }
}