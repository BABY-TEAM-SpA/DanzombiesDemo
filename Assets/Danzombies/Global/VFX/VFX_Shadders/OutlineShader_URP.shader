Shader "Danzombies/SpriteOutlineInner"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0,10)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        // =========================
        // INTERNAL OUTLINE
        // =========================
        Pass
        {
            Name "InternalOutline"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment fragOutline

            #include "UnityCG.cginc"


            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 _OutlineColor;
            float _OutlineSize;


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


            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }


            float Alpha(float2 uv)
            {
                return tex2D(_MainTex, uv).a;
            }


            fixed4 fragOutline(v2f i) : SV_Target
            {
                float alpha = Alpha(i.uv);

                float2 pixel = _MainTex_TexelSize.xy * _OutlineSize;

                float surrounding = 0;

                surrounding += Alpha(i.uv + float2(pixel.x,0));
                surrounding += Alpha(i.uv + float2(-pixel.x,0));
                surrounding += Alpha(i.uv + float2(0,pixel.y));
                surrounding += Alpha(i.uv + float2(0,-pixel.y));

                surrounding += Alpha(i.uv + float2(pixel.x,pixel.y));
                surrounding += Alpha(i.uv + float2(-pixel.x,pixel.y));
                surrounding += Alpha(i.uv + float2(pixel.x,-pixel.y));
                surrounding += Alpha(i.uv + float2(-pixel.x,-pixel.y));

                surrounding /= 8;


                // Borde interno:
                // dentro del sprite pero cerca de transparencia
                float outline = step(0.01, alpha) * (1 - step(0.99, surrounding));


                return float4(_OutlineColor.rgb, outline);
            }

            ENDCG
        }



        // =========================
        // SPRITE ORIGINAL
        // =========================
        Pass
        {
            Name "Sprite"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment fragSprite

            #include "UnityCG.cginc"


            sampler2D _MainTex;


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


            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }


            fixed4 fragSprite(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }

            ENDCG
        }
    }
}