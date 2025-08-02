Shader "GrayscaleFade"
{
    Properties
    {
        _FadeAmount ("Fade Amount", Range(0, 1)) = 0
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            Name "Grayscale"
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _FadeAmount;

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

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = lerp(col.rgb, float3(gray, gray, gray), saturate(_FadeAmount));
                return col;
            }

            ENDHLSL
        }
    }
}
