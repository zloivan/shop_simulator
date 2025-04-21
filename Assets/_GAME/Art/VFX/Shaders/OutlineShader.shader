Shader "Custom/UniversalOutline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0,1,0,1)
        _OutlineWidth("Outline Width", Range(0.0, 10.0)) = 2.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" "RenderPipeline"="UniversalPipeline" }
        
        // Первый проход для рисования контура
        Pass
        {
            Name "Outline"
            Cull Front
            ZTest Always // Важно! Рисуем контур даже если он закрыт другими объектами
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                float3 normalOS = input.normalOS;
                float3 positionOS = input.positionOS.xyz + normalOS * _OutlineWidth * 0.01;
                output.positionCS = TransformObjectToHClip(positionOS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}