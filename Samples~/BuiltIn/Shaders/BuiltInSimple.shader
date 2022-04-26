Shader "Custom/BuiltIn/Simple"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white"{}
		[PerRendererData] _ScaleOffset ("Texture Scale Offset", Vector) = (1,1,0,0)
		[PerRenderer] _Color ("Color", Color) = (0,0,0,0)
		[PerRendererData] _Power ("Power", Float) = 1
		
		_Shared("Shared Mat Value", Vector) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		ZWrite On
		ZTest LEqual
		Blend One Zero

		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"
			
			UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float, _Power)
                UNITY_DEFINE_INSTANCED_PROP(half4, _ScaleOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

			#define GET(name) UNITY_ACCESS_INSTANCED_PROP(Props, name)

			sampler2D _MainTex;
			float _Shared; 

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				half4 scaleOffset = GET(_ScaleOffset);
				o.uv = v.uv.xy * scaleOffset.xy + scaleOffset.zw;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				
				half4 col = tex2D(_MainTex, i.uv) * GET(_Color) * GET(_Power);
				return col;
			}
			ENDHLSL
		}
	}
}