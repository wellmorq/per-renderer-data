Shader "Custom/URP/Simple"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white"{}
		[PerRendererData] _ScaleOffset ("Texture Scale Offset", Vector) = (1,1,0,0)
		[PerRenderer] _Color ("Color", Color) = (0,0,0,0)
		[PerRendererData] _Power ("Power", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "LightMode" = "SRPDefaultUnlit" "DisableBatching" = "True"}
        LOD 100

        Pass
        {
            HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma multi_compile_instancing
            
            #pragma vertex VertexPass
            #pragma fragment FragmentPass
            
            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            
            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
                UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float, _Power)
                UNITY_DEFINE_INSTANCED_PROP(half4, _ScaleOffset)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)
            
            #define GET(name) UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, name)

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 baseUV : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 baseUV : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varyings VertexPass (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(positionWS);
                half4 scaleOffset = GET(_ScaleOffset);
                output.baseUV = input.baseUV * scaleOffset.xy + scaleOffset.zw;
                
                return output;
            }
            
            half4 FragmentPass (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.baseUV);
                return baseMap * GET(_Color) * GET(_Color) * GET(_Power);
            }
            ENDHLSL
        }
    }
}