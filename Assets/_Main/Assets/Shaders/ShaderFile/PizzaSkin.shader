// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Skin_Pizza_Eath_Unlit"
{
	Properties
	{
		_NormalPizza("NormalPizza", 2D) = "white" {}
		_PineapplePizza("PineapplePizza", 2D) = "white" {}
		_HealthAmount("HealthAmount", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _PineapplePizza;
			uniform float4 _PineapplePizza_ST;
			uniform sampler2D _NormalPizza;
			uniform float4 _NormalPizza_ST;
			uniform float _HealthAmount;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 uv_PineapplePizza = i.ase_texcoord1.xy * _PineapplePizza_ST.xy + _PineapplePizza_ST.zw;
				float2 uv_NormalPizza = i.ase_texcoord1.xy * _NormalPizza_ST.xy + _NormalPizza_ST.zw;
				float2 texCoord44 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break59 = ( texCoord44 - float2( 0.5,0.5 ) );
				float RadialMask67 = step( frac( ( ( atan2( break59.x , break59.y ) / ( 2.0 * UNITY_PI ) ) + 0.75 ) ) , _HealthAmount );
				float4 lerpResult7 = lerp( tex2D( _PineapplePizza, uv_PineapplePizza ) , tex2D( _NormalPizza, uv_NormalPizza ) , RadialMask67);
				
				
				finalColor = lerpResult7;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;66;-3136.447,408.2521;Inherit;False;2071.433;522.9302;Comment;12;44;45;59;56;60;61;63;51;64;62;65;67;Radial Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;2;-1499.861,-310.1058;Inherit;True;Property;_PineapplePizza;PineapplePizza;1;0;Create;True;0;0;0;False;0;False;-1;11bc1573a32bba14eb92bc29b790bc25;11bc1573a32bba14eb92bc29b790bc25;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;7;-757.918,-191.4592;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1502.775,-88.19492;Inherit;True;Property;_NormalPizza;NormalPizza;0;0;Create;True;0;0;0;False;0;False;-1;1c85d5552e55b3747b6b79d1e7cfe17d;1c85d5552e55b3747b6b79d1e7cfe17d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-3086.447,474.2678;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;45;-2822.448,473.2678;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;59;-2594.689,472.2269;Inherit;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ATan2OpNode;56;-2355.69,468.2269;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;60;-2079.69,463.2269;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;61;-2328.69,705.2274;Inherit;False;1;0;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;63;-1557.691,461.2269;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;64;-1300.164,458.2521;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-1834.69,462.2269;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;11;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-2156.933,816.0224;Inherit;False;Constant;_Offset;Offset;3;0;Create;True;0;0;0;False;0;False;0.75;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-1309.645,755.3663;Inherit;False;RadialMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-1063.017,-7.351945;Inherit;False;67;RadialMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1687.213,743.4583;Inherit;False;Property;_HealthAmount;HealthAmount;2;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;-483.269,-193.3502;Float;False;True;-1;2;ASEMaterialInspector;100;5;Mat_Skin_Pizza_Eath_Unlit;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;7;0;2;0
WireConnection;7;1;1;0
WireConnection;7;2;68;0
WireConnection;45;0;44;0
WireConnection;59;0;45;0
WireConnection;56;0;59;0
WireConnection;56;1;59;1
WireConnection;60;0;56;0
WireConnection;60;1;61;0
WireConnection;63;0;62;0
WireConnection;64;0;63;0
WireConnection;64;1;51;0
WireConnection;62;0;60;0
WireConnection;62;1;65;0
WireConnection;67;0;64;0
WireConnection;6;0;7;0
ASEEND*/
//CHKSM=F3E16113C5D6764B20CD0D51FF655E8A0CA143F7