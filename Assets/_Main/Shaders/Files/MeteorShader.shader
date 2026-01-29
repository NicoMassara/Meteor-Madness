// Made with Amplify Shader Editor v1.9.9.7
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MeteorShader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MeteorTexture1( "MeteorTexture", 2D ) = "white" {}
		_HealthAmount( "HealthAmount", Range( 0, 1 ) ) = 0
		_Opacity( "Opacity", Range( 0, 1 ) ) = 0
		[HDR] _AtmosphereColor( "AtmosphereColor", Color ) = ( 0, 819.1997, 1024, 0 )
		_AtmosphereIntensity( "AtmosphereIntensity", Range( 0, 2 ) ) = 0.5
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
		#define ASE_VERSION 19907
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MeteorTexture1;
		uniform float _HealthAmount;
		uniform float4 _AtmosphereColor_Damaged;
		uniform float4 _AtmosphereColor;
		uniform float _AtmosphereIntensity;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 color48 = IsGammaSpace() ? float4( 1, 0, 0, 0 ) : float4( 1, 0, 0, 0 );
			float2 uv_TexCoord6_g62 = i.uv_texcoord + float2( 0.5,0.5 );
			float Health_Amount41 = _HealthAmount;
			float mulTime5_g62 = _Time.y *  (0.0 + ( Health_Amount41 - 0.0 ) * ( 0.5 - 0.0 ) / ( 1.0 - 0.0 ) );
			float cos2_g62 = cos( mulTime5_g62 );
			float sin2_g62 = sin( mulTime5_g62 );
			float2 rotator2_g62 = mul( ( uv_TexCoord6_g62 - float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) , float2x2( cos2_g62 , -sin2_g62 , sin2_g62 , cos2_g62 )) + float2( 0.5,0.5 );
			float2 Rotator38 = rotator2_g62;
			float4 tex2DNode27 = tex2D( _MeteorTexture1, Rotator38 );
			float4 lerpResult45 = lerp( ( color48 * tex2DNode27 ) , tex2DNode27 , Health_Amount41);
			float4 Surface29 = lerpResult45;
			float2 uv_TexCoord2_g75 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 break2_g74 =  (float2( -1,-1 ) + ( uv_TexCoord2_g75 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) );
			float temp_output_5_0_g74 = ( ( break2_g74.x * break2_g74.x ) + ( break2_g74.y * break2_g74.y ) );
			float Depth14_g74 = saturate( ( sqrt( saturate( ( 1.0 - temp_output_5_0_g74 ) ) ) * 2.0 ) );
			float temp_output_1_15_g73 = temp_output_5_0_g74;
			float smoothstepResult3_g73 = smoothstep( 0.95 , 1.0 , temp_output_1_15_g73);
			float4 lerpResult69 = lerp( _AtmosphereColor_Damaged , _AtmosphereColor , Health_Amount41);
			float4 Atmosphere_Color70 = lerpResult69;
			float4 Emission14 = saturate( ( ( ( Surface29 * Depth14_g74 ) * ( 1.0 - smoothstepResult3_g73 ) ) + ( pow( temp_output_1_15_g73 , 4.0 ) * Atmosphere_Color70 * _AtmosphereIntensity * smoothstepResult3_g73 ) ) );
			o.Emission = Emission14.xyz;
			o.Alpha = _Opacity;
			float2 uv_TexCoord2_g66 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float smoothstepResult3_g65 = smoothstep( 1.0 , 0.98 , length(  (float2( -1,-1 ) + ( uv_TexCoord2_g66 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) ) ));
			float Circular_Mask7_g65 = sqrt( ( 1.0 - pow( ( 1.0 - smoothstepResult3_g65 ) , 2.0 ) ) );
			float CircularMask73 = Circular_Mask7_g65;
			clip( CircularMask73 - _Cutoff );
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
Version=19907
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-2000,-256;Inherit;False;596;162.95;Comment;2;41;40;Health Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-1968,-208;Inherit;False;Property;_HealthAmount;HealthAmount;2;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;31;-2000,-48;Inherit;False;808.0366;379.3143;Comment;3;74;32;38;Rotator;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;41;-1648,-208;Inherit;False;Health Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;32;-1968,80;Inherit;False;41;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;74;-1712,80;Inherit;False;UV_Rotator;-1;;62;6ce75a8d0b5e99f4f87e5ff1c6b9be70;0;1;7;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2016,752;Inherit;False;1208.257;634.6272;Comment;7;29;45;44;27;43;47;48;Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;38;-1440,80;Inherit;True;Rotator;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-1984,1056;Inherit;True;38;Rotator;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;-2928,-224;Inherit;False;884;562.95;Comment;5;70;69;68;67;66;Atmosphere Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;48;-1792,832;Inherit;False;Constant;_DamageColor;DamageColor;4;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;27;-1776,1040;Inherit;True;Property;_MeteorTexture1;MeteorTexture;1;0;Create;True;0;0;0;False;0;False;-1;54c9e9baffb09864a83d438f96841dfd;54c9e9baffb09864a83d438f96841dfd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-1744,1248;Inherit;False;41;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;47;-1456,864;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;66;-2768,240;Inherit;False;41;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;67;-2880,-160;Inherit;False;Property;_AtmosphereColor_Damaged;AtmosphereColor_Damaged;6;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;1.849696,0,0.0362239,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-2880,32;Inherit;False;Property;_AtmosphereColor;AtmosphereColor;4;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;0.9771981,0.5098926,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;45;-1216,944;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;69;-2480,-48;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;91;-3296,432;Inherit;False;1232.003;488.3707;Comment;5;97;98;30;99;14;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;29;-1056,944;Inherit;True;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;70;-2320,-48;Inherit;True;Atmosphere_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-2000,384;Inherit;False;484;303.95;Comment;2;73;72;Circular Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;30;-3280,496;Inherit;True;29;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;97;-2912,688;Inherit;False;Property;_AtmosphereIntensity;AtmosphereIntensity;5;0;Create;True;0;0;0;False;0;False;0.5;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;98;-3280,688;Inherit;True;70;Atmosphere_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;-1968,432;Inherit;False;UV_CircularMask;-1;;65;28af572f72fc00741ad2033d2531b438;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;99;-2864,496;Inherit;False;DepthSphereWithAtmosphere;-1;;73;1d41c5ee03fdd57478f8fecfd7e709ed;0;3;23;FLOAT4;0,0,0,0;False;14;COLOR;1,0,0,1;False;15;FLOAT;0;False;1;FLOAT4;10
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;73;-1744,432;Inherit;True;CircularMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;14;-2272,496;Inherit;True;Emission;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;12;-640,464;Inherit;True;73;CircularMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;50;-720,368;Inherit;False;Property;_Opacity;Opacity;3;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-656,144;Inherit;True;14;Emission;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;94;-16,112;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;MeteorShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;0;False;;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;True;1;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;41;0;40;0
WireConnection;74;7;32;0
WireConnection;38;0;74;0
WireConnection;27;1;43;0
WireConnection;47;0;48;0
WireConnection;47;1;27;0
WireConnection;45;0;47;0
WireConnection;45;1;27;0
WireConnection;45;2;44;0
WireConnection;69;0;67;0
WireConnection;69;1;68;0
WireConnection;69;2;66;0
WireConnection;29;0;45;0
WireConnection;70;0;69;0
WireConnection;99;23;30;0
WireConnection;99;14;98;0
WireConnection;99;15;97;0
WireConnection;73;0;72;0
WireConnection;14;0;99;10
WireConnection;94;2;15;0
WireConnection;94;9;50;0
WireConnection;94;10;12;0
ASEEND*/
//CHKSM=9E4A6A62C132351F28E9B3D61C395FED93EE631A