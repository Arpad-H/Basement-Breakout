Shader "URPWater/VFX/Mask" {
	Properties
	{
	}

	SubShader{
		Tags { "Queue" = "Transparent-1" "RenderPipeline" = "UniversalPipeline"  }
		ColorMask 0
		ZWrite On
		
		Pass {
			Name "URPWaterMask"
			
			HLSLPROGRAM
			#pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
			#pragma multi_compile_instancing
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"   
			
			struct Attributes
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
			
			#pragma vertex vert
            #pragma fragment frag

			Varyings vert(Attributes i)
            {
                Varyings o = (Varyings)0;
				
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_TRANSFER_INSTANCE_ID(i, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = TransformObjectToHClip(i.vertex.xyz);

                return o;
            }
			
			half4 frag() : SV_Target 
			{ 
				return 0; 
			}
			
			ENDHLSL
		}
	}
}