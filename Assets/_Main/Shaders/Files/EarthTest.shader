// Made with Amplify Shader Editor v1.9.9.7
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Skin_Default_Earth_Unlit"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EarthMap_tIL( "EarthMap_tIL", 2D ) = "white" {}
		_FlowMap( "FlowMap", 2D ) = "white" {}
		_HealthAmount( "HealthAmount", Range( 0, 1 ) ) = 0
		_Water( "Water", 2D ) = "white" {}
		_AtmosphereIntensity2( "AtmosphereIntensity", Range( 0, 2 ) ) = 0.5
		_Lava( "Lava", 2D ) = "white" {}
		_Surface( "Surface", 2D ) = "white" {}
		_Magma( "Magma", 2D ) = "white" {}
		_FlowIntensity( "Flow Intensity", Range( 0, 1 ) ) = 0
		_SolidTop( "_SolidTop", Range( 0, 1 ) ) = 0.1373401
		_WaterTiling1( "WaterTiling", Range( 0.1, 10 ) ) = 5.06
		_Opacity( "_Opacity", Range( 0, 1 ) ) = 0
		_TopFadeLenght( "_TopFadeLenght", Range( 0, 1 ) ) = 0.3475586
		_Vector0( "Vector 0", Vector ) = ( 1, -1.31, 0, 0 )
		_BottomFadeLenght( "_BottomFadeLenght", Range( 0, 1 ) ) = 0
		_MagmaTiling( "MagmaTiling", Float ) = 1.5
		_SolidBottom( "_SolidBottom", Range( 0, 1 ) ) = 0.3649352
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
		
		ColorMask RGB
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19907
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Magma;
		uniform float2 _Vector0;
		uniform float _HealthAmount;
		uniform float _MagmaTiling;
		uniform sampler2D _Surface;
		uniform float _SolidTop;
		uniform float _TopFadeLenght;
		uniform float _SolidBottom;
		uniform float _BottomFadeLenght;
		uniform sampler2D _EarthMap_tIL;
		uniform sampler2D _Lava;
		uniform float _WaterTiling1;
		uniform sampler2D _FlowMap;
		uniform float _FlowIntensity;
		uniform sampler2D _Water;
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
			float mulTime203 = _Time.y * 0.1;
			float PannerTime280 = mulTime203;
			float HealthAmount128 = _HealthAmount;
			float2 PannerSpeed277 = ( _Vector0 *  (0.0 + ( HealthAmount128 - 0.0 ) * ( 1.0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float2 temp_cast_0 = (_MagmaTiling).xx;
			float2 uv_TexCoord282 = i.uv_texcoord * temp_cast_0;
			float2 panner283 = ( PannerTime280 * PannerSpeed277 + uv_TexCoord282);
			float2 temp_cast_1 = (_MagmaTiling).xx;
			float2 uv_TexCoord284 = i.uv_texcoord * temp_cast_1;
			float2 panner285 = ( PannerTime280 * ( PannerSpeed277 * 0.25 ) + uv_TexCoord284);
			float temp_output_265_0 = ( HealthAmount128 * 0.5 );
			float temp_output_2_0_g53 = ( 1.0 -  (0.0 + ( _SolidTop - 0.0 ) * ( temp_output_265_0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float smoothstepResult5_g53 = smoothstep( temp_output_2_0_g53 , ( temp_output_2_0_g53 +  (0.0 + ( _TopFadeLenght - 0.0 ) * ( 0.25 - 0.0 ) / ( 1.0 - 0.0 ) ) ) , i.uv_texcoord.y);
			float temp_output_2_0_g54 = ( 1.0 -  (0.0 + ( _SolidBottom - 0.0 ) * ( temp_output_265_0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float smoothstepResult5_g54 = smoothstep( temp_output_2_0_g54 , ( temp_output_2_0_g54 +  (0.0 + ( _BottomFadeLenght - 0.0 ) * ( 0.25 - 0.0 ) / ( 1.0 - 0.0 ) ) ) , ( 1.0 - i.uv_texcoord.y ));
			float IceMask263 = ( saturate( smoothstepResult5_g53 ) + saturate( smoothstepResult5_g54 ) );
			float2 uv_TexCoord893 = i.uv_texcoord * float2( 0.5,1 );
			float2 panner68 = ( PannerTime280 * PannerSpeed277 + ( uv_TexCoord893 + float2( 0,0 ) ));
			float4 tex2DNode69 = tex2D( _EarthMap_tIL, panner68 );
			float LandMask146 = step( tex2DNode69.b , 0.35 );
			float WaterMask147 = step( tex2DNode69.g , 0.35 );
			float Ice769 = ( IceMask263 * saturate( ( ( LandMask146 * 1.0 ) + ( WaterMask147 * 0.5 ) ) ) );
			float4 temp_cast_2 = (Ice769).xxxx;
			float4 lerpResult324 = lerp( tex2D( _Surface, panner285 ) , temp_cast_2 , Ice769);
			float4 lerpResult174 = lerp( tex2D( _Magma, panner283 ) , lerpResult324 , HealthAmount128);
			float4 SurfaceColor168 = ( lerpResult174 * LandMask146 );
			float2 temp_cast_3 = (_WaterTiling1).xx;
			float2 uv_TexCoord309 = i.uv_texcoord * temp_cast_3;
			float2 temp_cast_5 = (_WaterTiling1).xx;
			float2 uv_TexCoord315 = i.uv_texcoord * temp_cast_5;
			float4 lerpResult313 = lerp( float4( uv_TexCoord309, 0.0 , 0.0 ) , tex2D( _FlowMap, uv_TexCoord315 ) , _FlowIntensity);
			float2 panner314 = ( PannerTime280 * ( PannerSpeed277 * _WaterTiling1 ) + lerpResult313.rg);
			float2 Panner317 = panner314;
			float4 lerpResult172 = lerp( tex2D( _Lava, Panner317 ) , tex2D( _Water, Panner317 ) , HealthAmount128);
			float4 WaterColor169 = ( lerpResult172 * WaterMask147 );
			float4 Surface908 = ( SurfaceColor168 + WaterColor169 );
			float2 uv_TexCoord2_g78 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 break2_g77 =  (float2( -1,-1 ) + ( uv_TexCoord2_g78 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) );
			float temp_output_5_0_g77 = ( ( break2_g77.x * break2_g77.x ) + ( break2_g77.y * break2_g77.y ) );
			float Depth14_g77 = saturate( ( sqrt( saturate( ( 1.0 - temp_output_5_0_g77 ) ) ) * 2.0 ) );
			float temp_output_1_15_g76 = temp_output_5_0_g77;
			float smoothstepResult3_g76 = smoothstep( 0.95 , 1.0 , temp_output_1_15_g76);
			float4 lerpResult652 = lerp( _AtmosphereColor_Damaged , _AtmosphereColor , HealthAmount128);
			float4 Atmosphere_Color650 = lerpResult652;
			float4 Emission373 = saturate( ( ( ( Surface908 * Depth14_g77 ) * ( 1.0 - smoothstepResult3_g76 ) ) + ( pow( temp_output_1_15_g76 , 4.0 ) * Atmosphere_Color650 * _AtmosphereIntensity2 * smoothstepResult3_g76 ) ) );
			o.Emission = Emission373.xyz;
			o.Alpha = _Opacity;
			float2 uv_TexCoord2_g61 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float smoothstepResult3_g60 = smoothstep( 1.0 , 0.98 , length(  (float2( -1,-1 ) + ( uv_TexCoord2_g61 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) ) ));
			float Circular_Mask7_g60 = sqrt( ( 1.0 - pow( ( 1.0 - smoothstepResult3_g60 ) , 2.0 ) ) );
			float CircularMask527 = Circular_Mask7_g60;
			clip( CircularMask527 - _Cutoff );
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
			#pragma target 3.0
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
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;207;-4864,-784;Inherit;False;1164.251;503.9158;Comment;11;68;203;205;206;179;208;277;280;892;893;894;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;894;-4816,-720;Inherit;False;Constant;_Vector1;Vector 1;22;0;Create;True;0;0;0;False;0;False;0.5,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;893;-4592,-720;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;892;-4160,-736;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;150;-3632,-832;Inherit;False;933.5408;593.2576;Comment;5;146;147;163;162;69;Surface / Water Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-3952,-720;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;270;-4099.584,1132.89;Inherit;False;1656.345;1173.663;Comment;22;269;263;219;264;218;268;216;215;262;257;214;213;265;261;259;258;256;267;266;260;255;769;Ice Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;69;-3408,-720;Inherit;True;Property;_EarthMap_tIL;EarthMap_tIL;1;0;Create;True;0;0;0;False;0;False;-1;d40864d99dd8121429c2fc384f12c7af;d40864d99dd8121429c2fc384f12c7af;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;492;-4736,-1424;Inherit;False;580;162.95;Comment;2;127;128;Health Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;162;-3088,-704;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;163;-3088,-608;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;255;-4064,1184;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;127;-4688,-1376;Inherit;False;Property;_HealthAmount;HealthAmount;3;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;147;-2928,-768;Inherit;True;WaterMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;146;-2928,-528;Inherit;True;LandMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;260;-3822.292,1468.644;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;266;-3400.159,1837.46;Inherit;False;Constant;_SolidMax;_SolidMax;20;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;267;-3790.104,1785.679;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;128;-4400,-1376;Inherit;False;HealthAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;256;-3711.772,1304.103;Inherit;False;Property;_SolidTop;_SolidTop;10;0;Create;True;0;0;0;False;0;False;0.1373401;0.9415613;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;258;-3709.01,1556.352;Inherit;False;Property;_SolidBottom;_SolidBottom;17;0;Create;True;0;0;0;False;0;False;0.3649352;0.4006471;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;259;-3602.596,1471.023;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;261;-3693.856,1643.494;Inherit;False;Property;_BottomFadeLenght;_BottomFadeLenght;15;0;Create;True;0;0;0;False;0;False;0;0.8013729;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;265;-3417.126,1716.347;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;213;-4006.941,2063.29;Inherit;False;147;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;214;-4000.983,1964.936;Inherit;False;146;LandMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;257;-3717.702,1389.757;Inherit;False;Property;_TopFadeLenght;_TopFadeLenght;13;0;Create;True;0;0;0;False;0;False;0.3475586;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;322;-5249.835,510.0308;Inherit;False;1662.074;549.343;Comment;11;308;309;310;313;314;315;316;317;320;321;318;Liquid Panner;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;262;-3342.192,1228.543;Inherit;False;FadeMask;-1;;53;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;215;-3728.494,1967.64;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;216;-3732.494,2065.642;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;268;-3347.538,1518.198;Inherit;False;FadeMask;-1;;54;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;179;-4848,-416;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;316;-5248,576;Inherit;False;Property;_WaterTiling1;WaterTiling;11;0;Create;True;0;0;0;False;0;False;5.06;2.742433;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;315;-5120,752;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;212;-4677.651,-159.9146;Inherit;False;2015.234;604.5853;Comment;18;285;283;282;278;279;292;284;289;290;168;157;153;174;138;149;324;251;274;Colored Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;208;-4512,-464;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;218;-3551.493,2001.64;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;264;-3061.276,1385.177;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;205;-4816,-544;Inherit;False;Property;_Vector0;Vector 0;14;0;Create;True;0;0;0;False;0;False;1,-1.31;0.35,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;308;-4836.333,738.1572;Inherit;True;Property;_FlowMap;FlowMap;2;0;Create;True;0;0;0;False;0;False;-1;None;a2567c79fcafff541bd097660979fea8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;309;-4800.4,561.8699;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;310;-4820.065,944.2155;Inherit;False;Property;_FlowIntensity;Flow Intensity;9;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;320;-4469.924,780.145;Inherit;False;277;PannerSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;219;-3396.493,2003.64;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;263;-3160.184,1740.067;Inherit;True;IceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;206;-4544,-560;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;203;-4272,-416;Inherit;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;290;-4496,304;Inherit;False;Constant;_SurfaceTiling;SurfaceTiling;14;0;Create;True;0;0;0;False;0;False;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;313;-4410.098,560.0308;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;321;-4245.056,798.9892;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;318;-4236.718,920.2867;Inherit;False;280;PannerTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;280;-4000,-448;Inherit;False;PannerTime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;269;-3141.711,2010.491;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;277;-4320,-608;Inherit;False;PannerSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;284;-4544,176;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.25,0.25;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;292;-4224,272;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;289;-4544,96;Inherit;False;Property;_MagmaTiling;MagmaTiling;16;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;314;-4059.808,662.8994;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;769;-2720,2016;Inherit;True;Ice;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;279;-4320,128;Inherit;False;280;PannerTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;278;-4320,32;Inherit;False;277;PannerSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;282;-4560,-48;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;285;-4048,176;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;211;-3501.816,492.2413;Inherit;False;848.0939;582.665;Comment;7;276;148;172;173;152;156;169;Colored Water;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;274;-3787.37,124.6347;Inherit;True;Property;_Surface;Surface;7;0;Create;True;0;0;0;False;0;False;-1;None;fcb3e019a33c36d41a3ebbc5b14f61a0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;251;-3760,352;Inherit;False;769;Ice;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;283;-4064,-48;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;317;-3840,672;Inherit;False;Panner;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;324;-3488,208;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;149;-3789.319,-88.47227;Inherit;True;Property;_Magma;Magma;8;0;Create;True;0;0;0;False;0;False;-1;eeb9d96c02a346c4884e4f7a9b316f92;eeb9d96c02a346c4884e4f7a9b316f92;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;138;-3200,80;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;173;-3120.946,561.1038;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;148;-3422.753,561.2415;Inherit;True;Property;_Lava;Lava;6;0;Create;True;0;0;0;False;0;False;-1;34c34d8c64649484e858b94f4766f062;34c34d8c64649484e858b94f4766f062;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;276;-3416.479,762.5539;Inherit;True;Property;_Water;Water;4;0;Create;True;0;0;0;False;0;False;-1;6fe629e0670cf8c4d980fd5b34631dcb;6fe629e0670cf8c4d980fd5b34631dcb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;174;-3168,-80;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;153;-2944,224;Inherit;True;146;LandMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;152;-2881.369,882.3578;Inherit;False;147;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;172;-3082.709,651.2108;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;157;-2944,0;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;156;-2902.399,644.9897;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;654;-5632,-144;Inherit;False;884;562.95;Comment;5;652;648;615;653;650;Atmosphere Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;909;-4690,-1954;Inherit;False;708;469;Comment;4;170;166;171;908;Surfacer;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;168;-2892.922,-95.00111;Inherit;False;SurfaceColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;169;-2915.355,557.8383;Inherit;False;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;648;-5584,-96;Inherit;False;Property;_AtmosphereColor_Damaged;AtmosphereColor_Damaged;19;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;1.849696,0,0.0362239,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;615;-5520,96;Inherit;False;Property;_AtmosphereColor;AtmosphereColor;18;1;[HDR];Create;True;0;0;0;False;0;False;0,819.1997,1024,0;0,0.7798894,1.385459,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;653;-5472,304;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;170;-4640,-1904;Inherit;True;168;SurfaceColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;171;-4640,-1712;Inherit;True;169;WaterColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;652;-5184,16;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;166;-4368,-1872;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;371;-4096,-1440;Inherit;False;1088.463;538.3311;Comment;5;910;373;906;907;905;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;650;-5024,16;Inherit;True;Atmosphere_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;908;-4224,-1872;Inherit;True;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;905;-3808,-1168;Inherit;False;Property;_AtmosphereIntensity2;AtmosphereIntensity;5;0;Create;True;0;0;0;False;0;False;0.5;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;907;-4048,-1168;Inherit;True;650;Atmosphere_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;910;-4048,-1376;Inherit;True;908;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;537;-4736,-1200;Inherit;False;484;303.95;Comment;1;527;Circular Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;886;-4704,-1152;Inherit;False;UV_CircularMask;-1;;60;28af572f72fc00741ad2033d2531b438;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;906;-3776,-1360;Inherit;False;DepthSphereWithAtmosphere;-1;;76;1d41c5ee03fdd57478f8fecfd7e709ed;0;3;23;FLOAT4;0,0,0,0;False;14;COLOR;1,0,0,1;False;15;FLOAT;0;False;1;FLOAT4;10
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;527;-4480,-1152;Inherit;True;CircularMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;373;-3328,-1360;Inherit;True;Emission;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;536;-1280,240;Inherit;True;527;CircularMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;372;-1136,-128;Inherit;True;373;Emission;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;911;-1136,112;Inherit;False;Property;_Opacity;_Opacity;12;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;98;-640,-112;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;Mat_Skin_Default_Earth_Unlit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;0;False;;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;False;0;False;;True;1;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;893;0;894;0
WireConnection;892;0;893;0
WireConnection;68;0;892;0
WireConnection;68;2;277;0
WireConnection;68;1;280;0
WireConnection;69;1;68;0
WireConnection;162;0;69;2
WireConnection;163;0;69;3
WireConnection;147;0;162;0
WireConnection;146;0;163;0
WireConnection;260;0;255;2
WireConnection;128;0;127;0
WireConnection;259;0;260;0
WireConnection;265;0;267;0
WireConnection;265;1;266;0
WireConnection;262;8;255;2
WireConnection;262;9;256;0
WireConnection;262;10;257;0
WireConnection;262;12;265;0
WireConnection;215;0;214;0
WireConnection;216;0;213;0
WireConnection;268;8;259;0
WireConnection;268;9;258;0
WireConnection;268;10;261;0
WireConnection;268;12;265;0
WireConnection;315;0;316;0
WireConnection;208;0;179;0
WireConnection;218;0;215;0
WireConnection;218;1;216;0
WireConnection;264;0;262;0
WireConnection;264;1;268;0
WireConnection;308;1;315;0
WireConnection;309;0;316;0
WireConnection;219;0;218;0
WireConnection;263;0;264;0
WireConnection;206;0;205;0
WireConnection;206;1;208;0
WireConnection;313;0;309;0
WireConnection;313;1;308;0
WireConnection;313;2;310;0
WireConnection;321;0;320;0
WireConnection;321;1;316;0
WireConnection;280;0;203;0
WireConnection;269;0;263;0
WireConnection;269;1;219;0
WireConnection;277;0;206;0
WireConnection;284;0;289;0
WireConnection;292;0;278;0
WireConnection;292;1;290;0
WireConnection;314;0;313;0
WireConnection;314;2;321;0
WireConnection;314;1;318;0
WireConnection;769;0;269;0
WireConnection;282;0;289;0
WireConnection;285;0;284;0
WireConnection;285;2;292;0
WireConnection;285;1;279;0
WireConnection;274;1;285;0
WireConnection;283;0;282;0
WireConnection;283;2;278;0
WireConnection;283;1;279;0
WireConnection;317;0;314;0
WireConnection;324;0;274;0
WireConnection;324;1;251;0
WireConnection;324;2;251;0
WireConnection;149;1;283;0
WireConnection;148;1;317;0
WireConnection;276;1;317;0
WireConnection;174;0;149;0
WireConnection;174;1;324;0
WireConnection;174;2;138;0
WireConnection;172;0;148;0
WireConnection;172;1;276;0
WireConnection;172;2;173;0
WireConnection;157;0;174;0
WireConnection;157;1;153;0
WireConnection;156;0;172;0
WireConnection;156;1;152;0
WireConnection;168;0;157;0
WireConnection;169;0;156;0
WireConnection;652;0;648;0
WireConnection;652;1;615;0
WireConnection;652;2;653;0
WireConnection;166;0;170;0
WireConnection;166;1;171;0
WireConnection;650;0;652;0
WireConnection;908;0;166;0
WireConnection;906;23;910;0
WireConnection;906;14;907;0
WireConnection;906;15;905;0
WireConnection;527;0;886;0
WireConnection;373;0;906;10
WireConnection;98;2;372;0
WireConnection;98;9;911;0
WireConnection;98;10;536;0
ASEEND*/
//CHKSM=0F40A2ED6D429A2FE28A067C842331F26A86DE1B