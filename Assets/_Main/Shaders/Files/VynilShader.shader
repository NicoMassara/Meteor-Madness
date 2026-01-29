// Made with Amplify Shader Editor v1.9.9.7
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Skin_Vinyl_Earth_Unlit"
{
	Properties
	{
		_Texture( "_Texture", 2D ) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_HealthAmount( "HealthAmount", Range( 0, 1 ) ) = 0
		_Opacity( "Opacity", Range( 0, 1 ) ) = 0
		_AtmosphereIntensity1( "AtmosphereIntensity", Range( 0, 2 ) ) = 0.5
		[HDR] _AtmosphereColor( "AtmosphereColor", Color ) = ( 0, 819.1997, 1024, 0 )
		[HDR] _AtmosphereColor_Damaged( "AtmosphereColor_Damaged", Color ) = ( 0, 819.1997, 1024, 0 )
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
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

		uniform sampler2D _Texture;
		uniform float _HealthAmount;
		uniform float4 _AtmosphereColor_Damaged;
		uniform float4 _AtmosphereColor;
		uniform float _AtmosphereIntensity1;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord6_g1 = i.uv_texcoord + float2( 0.5,0.5 );
			float Health_Amount14 = _HealthAmount;
			float mulTime5_g1 = _Time.y *  (0.0 + ( Health_Amount14 - 0.0 ) * ( 0.5 - 0.0 ) / ( 1.0 - 0.0 ) );
			float cos2_g1 = cos( mulTime5_g1 );
			float sin2_g1 = sin( mulTime5_g1 );
			float2 rotator2_g1 = mul( ( uv_TexCoord6_g1 - float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) , float2x2( cos2_g1 , -sin2_g1 , sin2_g1 , cos2_g1 )) + float2( 0.5,0.5 );
			float2 Rotator28 = rotator2_g1;
			float4 tex2DNode1 = tex2D( _Texture, Rotator28 );
			float4 color17 = IsGammaSpace() ? float4( 1, 0.0990566, 0.0990566, 1 ) : float4( 1, 0.009877041, 0.009877041, 1 );
			float4 lerpResult16 = lerp( ( tex2DNode1 * color17 ) , tex2DNode1 , Health_Amount14);
			float4 Surface10 = lerpResult16;
			float2 uv_TexCoord2_g75 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 break2_g74 =  (float2( -1,-1 ) + ( uv_TexCoord2_g75 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) );
			float temp_output_5_0_g74 = ( ( break2_g74.x * break2_g74.x ) + ( break2_g74.y * break2_g74.y ) );
			float Depth14_g74 = saturate( ( sqrt( saturate( ( 1.0 - temp_output_5_0_g74 ) ) ) * 2.0 ) );
			float temp_output_1_15_g73 = temp_output_5_0_g74;
			float smoothstepResult3_g73 = smoothstep( 0.95 , 1.0 , temp_output_1_15_g73);
			float4 lerpResult60 = lerp( _AtmosphereColor_Damaged , _AtmosphereColor , Health_Amount14);
			float4 Atmosphere_Color62 = lerpResult60;
			float4 Emission81 = saturate( ( ( ( Surface10 * Depth14_g74 ) * ( 1.0 - smoothstepResult3_g73 ) ) + ( pow( temp_output_1_15_g73 , 4.0 ) * Atmosphere_Color62 * _AtmosphereIntensity1 * smoothstepResult3_g73 ) ) );
			o.Emission = Emission81.xyz;
			o.Alpha = _Opacity;
			float2 uv_TexCoord2_g3 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float smoothstepResult3_g2 = smoothstep( 1.0 , 0.98 , length(  (float2( -1,-1 ) + ( uv_TexCoord2_g3 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) ) ));
			float Circular_Mask7_g2 = sqrt( ( 1.0 - pow( ( 1.0 - smoothstepResult3_g2 ) , 2.0 ) ) );
			float Opacity_Mask7 = Circular_Mask7_g2;
			clip( Opacity_Mask7 - _Cutoff );
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
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;12;-2768,0;Inherit;False;596;162.95;Comment;2;14;13;Health Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;13;-2720,48;Inherit;False;Property;_HealthAmount;HealthAmount;2;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;21;-3424,240;Inherit;False;757.144;327.1145;Comment;3;28;32;22;Rotator;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;14;-2416,48;Inherit;False;Health Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;22;-3392,320;Inherit;False;14;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;32;-3168,320;Inherit;False;UV_Rotator;-1;;1;6ce75a8d0b5e99f4f87e5ff1c6b9be70;0;1;7;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;20;-2112,32;Inherit;False;1124;538.7;Comment;7;1;18;19;16;10;17;29;Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;28;-2896,320;Inherit;True;Rotator;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;29;-2080,288;Inherit;False;28;Rotator;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-2128,656;Inherit;False;884;562.95;Comment;5;62;60;56;55;54;Atmosphere Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-1968,368;Inherit;False;Constant;_Color0;Color 0;3;0;Create;True;0;0;0;False;0;False;1,0.0990566,0.0990566,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2048,80;Inherit;True;Property;_Texture;_Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;5215cc37195cb314a89d24bdb64c53e3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;19;-1632,416;Inherit;False;14;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;18;-1680,256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-1968,1120;Inherit;False;14;Health Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;55;-2080,720;Inherit;False;Property;_AtmosphereColor_Damaged;AtmosphereColor_Damaged;6;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;1.849696,0,0.0362239,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;56;-2080,912;Inherit;False;Property;_AtmosphereColor;AtmosphereColor;5;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;0.9771981,0.5098926,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-1440,80;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;60;-1680,832;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;-3264,640;Inherit;False;1085.279;486.656;Comment;5;81;87;86;71;88;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;-1216,80;Inherit;True;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;62;-1520,832;Inherit;True;Atmosphere_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-2640,240;Inherit;False;516;310.95;Comment;2;33;7;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;88;-3232,912;Inherit;True;62;Atmosphere_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-3232,704;Inherit;True;10;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;87;-2912,912;Inherit;False;Property;_AtmosphereIntensity1;AtmosphereIntensity;4;0;Create;True;0;0;0;False;0;False;0.5;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;33;-2608,304;Inherit;False;UV_CircularMask;-1;;2;28af572f72fc00741ad2033d2531b438;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;86;-2864,720;Inherit;False;DepthSphereWithAtmosphere;-1;;73;1d41c5ee03fdd57478f8fecfd7e709ed;0;3;23;FLOAT4;0,0,0,0;False;14;COLOR;1,0,0,1;False;15;FLOAT;0;False;1;FLOAT4;10
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-2384,304;Inherit;True;Opacity Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;81;-2416,704;Inherit;True;Emission;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;9;-336,320;Inherit;True;7;Opacity Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;31;-464,208;Inherit;False;Property;_Opacity;Opacity;3;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;35;-480,-16;Inherit;True;81;Emission;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;0,-48;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;Mat_Skin_Vinyl_Earth_Unlit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;0;False;;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;True;1;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;13;0
WireConnection;32;7;22;0
WireConnection;28;0;32;0
WireConnection;1;1;29;0
WireConnection;18;0;1;0
WireConnection;18;1;17;0
WireConnection;16;0;18;0
WireConnection;16;1;1;0
WireConnection;16;2;19;0
WireConnection;60;0;55;0
WireConnection;60;1;56;0
WireConnection;60;2;54;0
WireConnection;10;0;16;0
WireConnection;62;0;60;0
WireConnection;86;23;71;0
WireConnection;86;14;88;0
WireConnection;86;15;87;0
WireConnection;7;0;33;0
WireConnection;81;0;86;10
WireConnection;0;2;35;0
WireConnection;0;9;31;0
WireConnection;0;10;9;0
ASEEND*/
//CHKSM=2510755B817A523C75550199E0F779AFDFF95794