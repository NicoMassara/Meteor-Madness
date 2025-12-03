// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_DeepSpace"
{
	Properties
	{
		_MainTex( "_MainTex", 2D ) = "white" {}
		_StarsSize( "StarsSize", Float ) = 0
		_StarsAmount( "StarsAmount", Range( 0, 1 ) ) = 0
		_NebulaColor( "NebulaColor", Color ) = ( 0.4500256, 0, 1, 0 )
		_NebulaNoiseIntensity( "NebulaNoiseIntensity", Range( 0, 1 ) ) = 3.3
		_NebulaIntensity( "NebulaIntensity", Range( 0, 1 ) ) = 0.5
		_MaskSize( "MaskSize", Range( 0, 1 ) ) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		

		Tags { "RenderType"="Opaque" }

	LOD 100

		

		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		

		CGINCLUDE
			#pragma target 3.0

			float4 ComputeClipSpacePosition( float2 screenPosNorm, float deviceDepth )
			{
				float4 positionCS = float4( screenPosNorm * 2.0 - 1.0, deviceDepth, 1.0 );
			#if UNITY_UV_STARTS_AT_TOP
				positionCS.y = -positionCS.y;
			#endif
				return positionCS;
			}
		ENDCG

		
		Pass
		{
			Name "Unlit"

			CGPROGRAM
				#define ASE_VERSION 19905

				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
					#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#include "UnityCG.cginc"

				#define ASE_NEEDS_TEXTURE_COORDINATES0


				struct appdata
				{
					float4 vertex : POSITION;
					float4 ase_texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float4 ase_texcoord : TEXCOORD0;
					float4 ase_texcoord1 : TEXCOORD1;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float _StarsSize;
				uniform float _StarsAmount;
				uniform float _NebulaNoiseIntensity;
				uniform float _MaskSize;
				uniform float4 _NebulaColor;
				uniform float _NebulaIntensity;


				float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
				float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
				float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
				float snoise( float2 v )
				{
					const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
					float2 i = floor( v + dot( v, C.yy ) );
					float2 x0 = v - i + dot( i, C.xx );
					float2 i1;
					i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
					float4 x12 = x0.xyxy + C.xxzz;
					x12.xy -= i1;
					i = mod2D289( i );
					float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
					float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
					m = m * m;
					m = m * m;
					float3 x = 2.0 * frac( p * C.www ) - 1.0;
					float3 h = abs( x ) - 0.5;
					float3 ox = floor( x + 0.5 );
					float3 a0 = x - ox;
					m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
					float3 g;
					g.x = a0.x * x0.x + h.x * x0.y;
					g.yz = a0.yz * x12.xz + h.yz * x12.yw;
					return 130.0 * dot( m, g );
				}
				

				v2f vert ( appdata v )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID( v );
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
					UNITY_TRANSFER_INSTANCE_ID( v, o );

					float3 ase_positionWS = mul( unity_ObjectToWorld, float4( ( v.vertex ).xyz, 1 ) ).xyz;
					o.ase_texcoord1.xyz = ase_positionWS;
					
					o.ase_texcoord.xy = v.ase_texcoord.xy;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord.zw = 0;
					o.ase_texcoord1.w = 0;

					float3 vertexValue = float3( 0, 0, 0 );
					#if ASE_ABSOLUTE_VERTEX_POS
						vertexValue = v.vertex.xyz;
					#endif
					vertexValue = vertexValue;
					#if ASE_ABSOLUTE_VERTEX_POS
						v.vertex.xyz = vertexValue;
					#else
						v.vertex.xyz += vertexValue;
					#endif

					o.pos = UnityObjectToClipPos( v.vertex );
					return o;
				}

				half4 frag( v2f IN  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( IN );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
					half4 finalColor;

					float4 ScreenPosNorm = float4( IN.pos.xy * ( _ScreenParams.zw - 1.0 ), IN.pos.zw );
					float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, IN.pos.z ) * IN.pos.w;
					float4 ScreenPos = ComputeScreenPos( ClipPos );

					float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float3 ase_positionWS = IN.ase_texcoord1.xyz;
					float simplePerlin2D346 = snoise( ase_positionWS.xy*( _StarsSize / 2.0 ) );
					simplePerlin2D346 = simplePerlin2D346*0.5 + 0.5;
					float temp_output_339_0 =  (-15.0 + ( _StarsAmount - 0.0 ) * ( -8.0 - -15.0 ) / ( 1.0 - 0.0 ) );
					float simplePerlin2D337 = snoise( ase_positionWS.xy*_StarsSize );
					simplePerlin2D337 = simplePerlin2D337*0.5 + 0.5;
					float Stars453 = max(  (temp_output_339_0 + ( simplePerlin2D346 - 0.0 ) * ( 1.0 - temp_output_339_0 ) / ( 1.0 - 0.0 ) ) ,  (temp_output_339_0 + ( simplePerlin2D337 - 0.0 ) * ( 1.0 - temp_output_339_0 ) / ( 1.0 - 0.0 ) ) );
					float4 temp_cast_2 = (Stars453).xxxx;
					float simplePerlin2D301 = snoise( ase_positionWS.xy*25.08 );
					simplePerlin2D301 = simplePerlin2D301*0.5 + 0.5;
					float Nebula312 =  (0.0 + ( ( -simplePerlin2D301 * _NebulaNoiseIntensity ) - 0.1 ) * ( 1.0 - 0.0 ) / ( 1.0 - 0.1 ) );
					float temp_output_470_0 =  (3.0 + ( _MaskSize - 0.0 ) * ( 15.0 - 3.0 ) / ( 1.0 - 0.0 ) );
					float smoothstepResult461 = smoothstep( temp_output_470_0 , 1.0 , ase_positionWS.y);
					float smoothstepResult458 = smoothstep( ( temp_output_470_0 * -1.0 ) , 1.0 , ase_positionWS.y);
					float NebulaMask460 = ( smoothstepResult461 * smoothstepResult458 );
					float4 MaskedNebula238 = ( saturate( ( ( Nebula312 + NebulaMask460 ) * _NebulaColor ) ) * _NebulaIntensity );
					

					finalColor = ( tex2D( _MainTex, uv_MainTex ) * max( temp_cast_2 , MaskedNebula238 ) );

					return finalColor;
				}
			ENDCG
		}
	}
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;314;-4192,608;Inherit;False;1570.792;402.5822;Comment;7;312;323;305;308;311;301;457;Nebula;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;472;-4208,1056;Inherit;False;1556;554.9;Comment;8;469;455;471;470;461;458;467;460;Nebula Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.PositionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;457;-4160,688;Inherit;False;1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;469;-4160,1360;Inherit;False;Property;_MaskSize;MaskSize;6;0;Create;True;0;0;0;False;0;False;1;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;301;-3888,656;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;25.08;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;311;-3584,656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;471;-3552,1472;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;470;-3856,1360;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;3;False;4;FLOAT;15;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;308;-3584,816;Inherit;False;Property;_NebulaNoiseIntensity;NebulaNoiseIntensity;4;0;Create;True;0;0;0;False;0;False;3.3;0.6743378;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PositionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;455;-3856,1152;Inherit;False;1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;305;-3376,656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;461;-3344,1152;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;458;-3344,1280;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-0.19;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;467;-3104,1216;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;323;-3136,672;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;352;-4199.339,-793.3651;Inherit;False;1194.373;766.2482;Comment;11;353;350;351;338;339;346;337;334;335;453;456;Stars;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;270;-4192,48;Inherit;False;1313.903;517.8859;Comment;9;238;309;310;251;297;212;303;313;459;Masked Nebula;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;312;-2896,672;Inherit;True;Nebula;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;460;-2896,1216;Inherit;True;NebulaMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;335;-4110.757,-302.7006;Inherit;False;Property;_StarsSize;StarsSize;1;0;Create;True;0;0;0;False;0;False;0;213.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;459;-4176,352;Inherit;True;460;NebulaMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;313;-4176,144;Inherit;True;312;Nebula;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;334;-4112.757,-191.7006;Inherit;False;Property;_StarsAmount;StarsAmount;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;353;-4098.085,-704.4132;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PositionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;456;-4128,-512;Inherit;False;1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;303;-3872,144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;212;-3840,320;Inherit;False;Property;_NebulaColor;NebulaColor;3;0;Create;True;0;0;0;False;0;False;0.4500256,0,1,0;0.4221258,0,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.NoiseGeneratorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;337;-3834.758,-497.7006;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;346;-3831.34,-743.3651;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;339;-3723.791,-213.9169;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-15;False;4;FLOAT;-8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;297;-3648,144;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;338;-3463.16,-491.7006;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;350;-3464.085,-697.4132;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;251;-3472,144;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;310;-3536,304;Inherit;False;Property;_NebulaIntensity;NebulaIntensity;5;0;Create;True;0;0;0;False;0;False;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;351;-3162.085,-560.4132;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;309;-3312,144;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;454;-2584.909,-759.9319;Inherit;False;914.0962;505.4064;Comment;6;318;264;192;1;240;326;Out;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;453;-3392.766,-235.4042;Inherit;False;Stars;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;238;-3168,144;Inherit;True;MaskedNebula;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;192;-2520.599,-478.4512;Inherit;False;453;Stars;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;240;-2516.379,-369.6855;Inherit;False;238;MaskedNebula;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;264;-2265.49,-449.1932;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2534.909,-709.9319;Inherit;True;Property;_MainTex;_MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;318;-2069.47,-594.9627;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;326;-1897.173,-592.7801;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;100;5;Mat_DeepSpace;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position;1;0;0;1;True;False;;False;0
WireConnection;301;0;457;0
WireConnection;311;0;301;0
WireConnection;471;0;470;0
WireConnection;470;0;469;0
WireConnection;305;0;311;0
WireConnection;305;1;308;0
WireConnection;461;0;455;2
WireConnection;461;1;470;0
WireConnection;458;0;455;2
WireConnection;458;1;471;0
WireConnection;467;0;461;0
WireConnection;467;1;458;0
WireConnection;323;0;305;0
WireConnection;312;0;323;0
WireConnection;460;0;467;0
WireConnection;353;0;335;0
WireConnection;303;0;313;0
WireConnection;303;1;459;0
WireConnection;337;0;456;0
WireConnection;337;1;335;0
WireConnection;346;0;456;0
WireConnection;346;1;353;0
WireConnection;339;0;334;0
WireConnection;297;0;303;0
WireConnection;297;1;212;0
WireConnection;338;0;337;0
WireConnection;338;3;339;0
WireConnection;350;0;346;0
WireConnection;350;3;339;0
WireConnection;251;0;297;0
WireConnection;351;0;350;0
WireConnection;351;1;338;0
WireConnection;309;0;251;0
WireConnection;309;1;310;0
WireConnection;453;0;351;0
WireConnection;238;0;309;0
WireConnection;264;0;192;0
WireConnection;264;1;240;0
WireConnection;318;0;1;0
WireConnection;318;1;264;0
WireConnection;326;0;318;0
ASEEND*/
//CHKSM=A5DBF3ABEBD69C0FDA3A78E373054CB775BB61C6