// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Skin_Pizza_Eath_Unlit"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_PinneapplePizzaTexture( "PinneapplePizzaTexture", 2D ) = "white" {}
		_HealthAmount( "HealthAmount", Range( 0, 1 ) ) = 0
		_PizzaTexture( "PizzaTexture", 2D ) = "white" {}
		_Opacity( "Opacity", Range( 0, 1 ) ) = 0
		_AtmosphereIntensity2( "AtmosphereIntensity", Range( 0, 2 ) ) = 0.5
		[HDR] _AtmosphereColor( "AtmosphereColor", Color ) = ( 0, 819.1997, 1024, 0 )
		[HDR] _AtmosphereColor_Damaged( "AtmosphereColor_Damaged", Color ) = ( 0, 819.1997, 1024, 0 )
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#define ASE_VERSION 19905
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _PinneapplePizzaTexture;
		uniform float _HealthAmount;
		uniform sampler2D _PizzaTexture;
		uniform float4 _AtmosphereColor_Damaged;
		uniform float4 _AtmosphereColor;
		uniform float _AtmosphereIntensity2;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord6_g4 = i.uv_texcoord + float2( 0.5,0.5 );
			float Health_Amount21 = _HealthAmount;
			float mulTime5_g4 = _Time.y *  (0.0 + ( Health_Amount21 - 0.0 ) * ( 0.5 - 0.0 ) / ( 1.0 - 0.0 ) );
			float cos2_g4 = cos( mulTime5_g4 );
			float sin2_g4 = sin( mulTime5_g4 );
			float2 rotator2_g4 = mul( ( uv_TexCoord6_g4 - float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) , float2x2( cos2_g4 , -sin2_g4 , sin2_g4 , cos2_g4 )) + float2( 0.5,0.5 );
			float2 Rotator32 = rotator2_g4;
			float2 uv_TexCoord2_g1 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 break35 =  (float2( -1,-1 ) + ( uv_TexCoord2_g1 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) );
			float RadialMask44 = step( frac( ( ( atan2( break35.x , break35.y ) / ( 2.0 * UNITY_PI ) ) + 0.75 ) ) , Health_Amount21 );
			float4 lerpResult22 = lerp( tex2D( _PinneapplePizzaTexture, Rotator32 ) , tex2D( _PizzaTexture, Rotator32 ) , RadialMask44);
			float4 Surface247 = lerpResult22;
			float2 uv_TexCoord2_g81 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 break2_g80 =  (float2( -1,-1 ) + ( uv_TexCoord2_g81 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) );
			float temp_output_5_0_g80 = ( ( break2_g80.x * break2_g80.x ) + ( break2_g80.y * break2_g80.y ) );
			float Depth14_g80 = saturate( ( sqrt( saturate( ( 1.0 - temp_output_5_0_g80 ) ) ) * 2.0 ) );
			float temp_output_1_15_g79 = temp_output_5_0_g80;
			float smoothstepResult3_g79 = smoothstep( 0.95 , 1.0 , temp_output_1_15_g79);
			float4 lerpResult213 = lerp( _AtmosphereColor_Damaged , _AtmosphereColor , Health_Amount21);
			float4 Atmosphere_Color214 = lerpResult213;
			float4 Emission76 = saturate( ( ( ( Surface247 * Depth14_g80 ) * ( 1.0 - smoothstepResult3_g79 ) ) + ( pow( temp_output_1_15_g79 , 4.0 ) * Atmosphere_Color214 * _AtmosphereIntensity2 * smoothstepResult3_g79 ) ) );
			o.Emission = Emission76.xyz;
			o.Alpha = _Opacity;
			float2 uv_TexCoord2_g63 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float smoothstepResult3_g62 = smoothstep( 1.0 , 0.98 , length(  (float2( -1,-1 ) + ( uv_TexCoord2_g63 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) ) ));
			float Circular_Mask7_g62 = sqrt( ( 1.0 - pow( ( 1.0 - smoothstepResult3_g62 ) , 2.0 ) ) );
			float Opacity_Mask134 = Circular_Mask7_g62;
			clip( Opacity_Mask134 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;24;-3264,1152;Inherit;False;1582.741;517.2915;Comment;11;44;43;42;41;40;39;38;37;36;35;193;Radial Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;193;-3232,1216;Inherit;False;UV_Centered;-1;;1;dafae9849f84fae499d6f673f49d4819;0;2;4;FLOAT2;0,0;False;5;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;19;-4032,560;Inherit;False;596;162.95;Comment;2;21;20;Health Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.BreakToComponentsNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;35;-2992,1216;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;20;-3984,608;Inherit;False;Property;_HealthAmount;HealthAmount;2;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ATan2OpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;36;-2848,1216;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;37;-2944,1456;Inherit;False;1;0;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;21;-3680,608;Inherit;False;Health Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;25;-2144,624;Inherit;False;799.6;419.1202;Comment;3;32;195;26;Rotator;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;38;-2688,1216;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-2768,1568;Inherit;False;Constant;_Offset;Offset;3;0;Create;True;0;0;0;False;0;False;0.75;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-2448,1216;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;11;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;26;-2112,736;Inherit;False;21;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;41;-2160,1216;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2304,1520;Inherit;False;21;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;195;-1872,736;Inherit;False;UV_Rotator;-1;;4;6ce75a8d0b5e99f4f87e5ff1c6b9be70;0;1;7;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;248;-3312,32;Inherit;False;1108;490.7;Comment;6;47;17;178;45;22;247;Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-1904,1216;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;32;-1600,736;Inherit;True;Rotator;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;209;-4256,848;Inherit;False;884;562.95;Comment;5;214;213;212;211;210;Atmosphere Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-1920,1520;Inherit;False;RadialMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;47;-3248,224;Inherit;True;32;Rotator;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;210;-4096,1312;Inherit;False;21;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;211;-4208,912;Inherit;False;Property;_AtmosphereColor_Damaged;AtmosphereColor_Damaged;7;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;1.849696,0,0.0362239,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;212;-4176,1104;Inherit;False;Property;_AtmosphereColor;AtmosphereColor;6;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;0.9895288,1,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-3008,96;Inherit;True;Property;_PinneapplePizzaTexture;PinneapplePizzaTexture;1;0;Create;True;0;0;0;False;0;False;-1;None;11bc1573a32bba14eb92bc29b790bc25;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;178;-3008,304;Inherit;True;Property;_PizzaTexture;PizzaTexture;3;0;Create;True;0;0;0;False;0;False;-1;None;1c85d5552e55b3747b6b79d1e7cfe17d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;45;-2704,336;Inherit;False;44;RadialMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;213;-3808,1024;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;22;-2688,96;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;208;-3328,608;Inherit;False;1072.535;450.9606;Comment;5;76;250;249;251;253;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;214;-3648,1024;Inherit;True;Atmosphere_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;247;-2432,96;Inherit;True;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;156;-1456,1168;Inherit;False;985;306.95;Comment;2;134;194;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;249;-3280,672;Inherit;True;247;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;250;-3296,864;Inherit;True;214;Atmosphere_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;251;-2976,912;Inherit;False;Property;_AtmosphereIntensity2;AtmosphereIntensity;5;0;Create;True;0;0;0;False;0;False;0.5;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;194;-1392,1232;Inherit;True;UV_CircularMask;-1;;62;28af572f72fc00741ad2033d2531b438;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;253;-2944,720;Inherit;False;DepthSphereWithAtmosphere;-1;;79;1d41c5ee03fdd57478f8fecfd7e709ed;0;3;23;FLOAT4;0,0,0,0;False;14;COLOR;1,0,0,1;False;15;FLOAT;0;False;1;FLOAT4;10
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;134;-1056,1232;Inherit;True;Opacity Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;76;-2512,704;Inherit;True;Emission;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;184;-288,1024;Inherit;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;191;-272,800;Inherit;True;76;Emission;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;192;-272,1184;Inherit;True;134;Opacity Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;189;288,864;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;Mat_Skin_Pizza_Eath_Unlit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;True;1;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;193;0
WireConnection;36;0;35;0
WireConnection;36;1;35;1
WireConnection;21;0;20;0
WireConnection;38;0;36;0
WireConnection;38;1;37;0
WireConnection;40;0;38;0
WireConnection;40;1;39;0
WireConnection;41;0;40;0
WireConnection;195;7;26;0
WireConnection;43;0;41;0
WireConnection;43;1;42;0
WireConnection;32;0;195;0
WireConnection;44;0;43;0
WireConnection;17;1;47;0
WireConnection;178;1;47;0
WireConnection;213;0;211;0
WireConnection;213;1;212;0
WireConnection;213;2;210;0
WireConnection;22;0;17;0
WireConnection;22;1;178;0
WireConnection;22;2;45;0
WireConnection;214;0;213;0
WireConnection;247;0;22;0
WireConnection;253;23;249;0
WireConnection;253;14;250;0
WireConnection;253;15;251;0
WireConnection;134;0;194;0
WireConnection;76;0;253;10
WireConnection;189;2;191;0
WireConnection;189;9;184;0
WireConnection;189;10;192;0
ASEEND*/
//CHKSM=D8830D35551EE034D7F17465A2595D0C17212094