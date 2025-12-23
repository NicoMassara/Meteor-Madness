// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Skin_Default_Earth_Unlit"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EarthMap_tIL( "EarthMap_tIL", 2D ) = "white" {}
		_AtmosphereSize( "AtmosphereSize", Range( 1, 10 ) ) = 2.11
		_FlowMap( "FlowMap", 2D ) = "white" {}
		_AtmosphereColor( "AtmosphereColor", Color ) = ( 0, 0.4000001, 1, 1 )
		_HealthAmount( "HealthAmount", Range( 0, 1 ) ) = 0
		_Water( "Water", 2D ) = "white" {}
		_Lava( "Lava", 2D ) = "white" {}
		_Surface( "Surface", 2D ) = "white" {}
		_Magma( "Magma", 2D ) = "white" {}
		_FlowIntensity( "Flow Intensity", Range( 0, 1 ) ) = 0
		_SolidTop( "_SolidTop", Range( 0, 1 ) ) = 0.1373401
		_WaterTiling1( "WaterTiling", Range( 0.1, 10 ) ) = 5.06
		_TopFadeLenght( "_TopFadeLenght", Range( 0, 1 ) ) = 0.3475586
		_Vector0( "Vector 0", Vector ) = ( 1, -1.31, 0, 0 )
		_BottomFadeLenght( "_BottomFadeLenght", Range( 0, 1 ) ) = 0
		_SolidBottom( "_SolidBottom", Range( 0, 1 ) ) = 0.3649352
		_MainTex( "_MainTex", 2D ) = "white" {}
		_Opacity( "Opacity", Range( 0, 1 ) ) = 0
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
		#define ASE_VERSION 19905
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Magma;
		uniform float2 _Vector0;
		uniform float _HealthAmount;
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
		uniform float4 _AtmosphereColor;
		uniform float _AtmosphereSize;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float mulTime203 = _Time.y * 0.1;
			float PannerTime280 = mulTime203;
			float HealthAmount128 = _HealthAmount;
			float2 PannerSpeed277 = ( _Vector0 *  (0.0 + ( HealthAmount128 - 0.0 ) * ( 1.0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float2 temp_cast_0 = (1.5).xx;
			float2 uv_TexCoord282 = i.uv_texcoord * temp_cast_0;
			float2 panner283 = ( PannerTime280 * PannerSpeed277 + uv_TexCoord282);
			float2 temp_cast_1 = (0.25).xx;
			float2 uv_TexCoord284 = i.uv_texcoord * temp_cast_1;
			float2 panner285 = ( PannerTime280 * ( PannerSpeed277 * 0.25 ) + uv_TexCoord284);
			float temp_output_265_0 = ( HealthAmount128 * 0.5 );
			float temp_output_2_0_g14 = ( 1.0 -  (0.0 + ( _SolidTop - 0.0 ) * ( temp_output_265_0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float smoothstepResult5_g14 = smoothstep( temp_output_2_0_g14 , ( temp_output_2_0_g14 +  (0.0 + ( _TopFadeLenght - 0.0 ) * ( 0.25 - 0.0 ) / ( 1.0 - 0.0 ) ) ) , i.uv_texcoord.y);
			float temp_output_2_0_g15 = ( 1.0 -  (0.0 + ( _SolidBottom - 0.0 ) * ( temp_output_265_0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float smoothstepResult5_g15 = smoothstep( temp_output_2_0_g15 , ( temp_output_2_0_g15 +  (0.0 + ( _BottomFadeLenght - 0.0 ) * ( 0.25 - 0.0 ) / ( 1.0 - 0.0 ) ) ) , ( 1.0 - i.uv_texcoord.y ));
			float IceMask263 = ( saturate( smoothstepResult5_g14 ) + saturate( smoothstepResult5_g15 ) );
			float2 uv_TexCoord38 = i.uv_texcoord * float2( 0.5,1 );
			float2 panner68 = ( PannerTime280 * PannerSpeed277 + uv_TexCoord38);
			float4 tex2DNode69 = tex2D( _EarthMap_tIL, panner68 );
			float LandMask146 = step( tex2DNode69.b , 0.35 );
			float WaterMask147 = step( tex2DNode69.g , 0.35 );
			float Ice250 = ( IceMask263 * saturate( ( ( LandMask146 * 1.0 ) + ( WaterMask147 * 0.5 ) ) ) );
			float4 temp_cast_2 = (Ice250).xxxx;
			float4 lerpResult324 = lerp( tex2D( _Surface, panner285 ) , temp_cast_2 , Ice250);
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
			float temp_output_6_0 = distance( ( i.uv_texcoord - float2( 0.5,0.5 ) ) , float2( 0,0 ) );
			float CircularMask102 = step( temp_output_6_0 , 0.5 );
			float AtmoshphereMask117 = ( CircularMask102 * ( pow( temp_output_6_0 , _AtmosphereSize ) * 5.0 ) );
			o.Emission = ( tex2D( _MainTex, uv_MainTex ) * saturate( ( saturate( ( SurfaceColor168 + WaterColor169 ) ) + ( ( _AtmosphereColor * AtmoshphereMask117 ) * HealthAmount128 ) ) ) ).rgb;
			o.Alpha = _Opacity;
			clip( CircularMask102 - _Cutoff );
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
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;207;-4791.579,-796.2318;Inherit;False;1164.251;503.9158;Comment;9;38;68;203;205;206;179;208;277;280;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;38;-4557.371,-743.4373;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.5,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;150;-3590.209,-812.4095;Inherit;False;933.5408;593.2576;Comment;5;146;147;163;162;69;Surface / Water Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-4135.483,-740.196;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;270;-4099.584,1132.89;Inherit;False;1458.345;1149.663;Comment;22;250;263;264;269;257;268;216;214;219;218;215;213;267;266;265;262;261;260;259;258;256;255;Ice Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;69;-3362.225,-700.2897;Inherit;True;Property;_EarthMap_tIL;EarthMap_tIL;1;0;Create;True;0;0;0;False;0;False;-1;d40864d99dd8121429c2fc384f12c7af;d40864d99dd8121429c2fc384f12c7af;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;162;-3053.985,-684.7157;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;163;-3053.985,-585.7157;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;255;-4049.584,1182.89;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;147;-2887.068,-750.4095;Inherit;True;WaterMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;146;-2897.869,-510.4095;Inherit;True;LandMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;260;-3822.292,1468.644;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;266;-3400.159,1837.46;Inherit;False;Constant;_SolidMax;_SolidMax;20;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;267;-3790.104,1785.679;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;127;-2064.659,-504.2914;Inherit;False;Property;_HealthAmount;HealthAmount;5;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;256;-3711.772,1304.103;Inherit;False;Property;_SolidTop;_SolidTop;11;0;Create;True;0;0;0;False;0;False;0.1373401;0.9415613;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;258;-3709.01,1556.352;Inherit;False;Property;_SolidBottom;_SolidBottom;16;0;Create;True;0;0;0;False;0;False;0.3649352;0.4006471;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;259;-3602.596,1471.023;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;261;-3693.856,1643.494;Inherit;False;Property;_BottomFadeLenght;_BottomFadeLenght;15;0;Create;True;0;0;0;False;0;False;0;0.8013729;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;265;-3417.126,1716.347;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;213;-4006.941,2063.29;Inherit;False;147;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;214;-4000.983,1964.936;Inherit;False;146;LandMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;257;-3717.702,1389.757;Inherit;False;Property;_TopFadeLenght;_TopFadeLenght;13;0;Create;True;0;0;0;False;0;False;0.3475586;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;128;-1769.658,-503.2914;Inherit;False;HealthAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;322;-5249.835,510.0308;Inherit;False;1662.074;549.343;Comment;11;308;309;310;313;314;315;316;317;320;321;318;Liquid Panner;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;179;-4780.465,-424.476;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;262;-3342.192,1228.543;Inherit;False;FadeMask;-1;;14;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;215;-3728.494,1967.64;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;216;-3732.494,2065.642;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;268;-3347.538,1518.198;Inherit;False;FadeMask;-1;;15;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;212;-4623.051,-159.9146;Inherit;False;1960.634;608.4853;Comment;18;251;292;285;290;284;279;289;282;278;283;274;149;138;174;153;157;168;324;Colored Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;205;-4705.272,-593.134;Inherit;False;Property;_Vector0;Vector 0;14;0;Create;True;0;0;0;False;0;False;1,-1.31;0.35,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;218;-3551.493,2001.64;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;315;-5084.729,765.4216;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;316;-5199.835,573.0679;Inherit;False;Property;_WaterTiling1;WaterTiling;12;0;Create;True;0;0;0;False;0;False;5.06;2.742433;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;264;-3061.276,1385.177;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;208;-4445.534,-469.0101;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;101;-3960.899,-1490.679;Inherit;False;1297.689;571.6741;Comment;10;109;102;117;114;112;108;8;6;4;3;Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;206;-4364.272,-606.134;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;203;-4171.127,-434.7295;Inherit;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;219;-3396.493,2003.64;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;308;-4836.333,738.1572;Inherit;True;Property;_FlowMap;FlowMap;3;0;Create;True;0;0;0;False;0;False;-1;None;a2567c79fcafff541bd097660979fea8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;309;-4800.4,561.8699;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;310;-4820.065,944.2155;Inherit;False;Property;_FlowIntensity;Flow Intensity;10;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;320;-4469.924,780.145;Inherit;False;277;PannerSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;290;-4486.895,292.2399;Inherit;False;Constant;_SurfaceTiling;SurfaceTiling;14;0;Create;True;0;0;0;False;0;False;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;263;-3160.184,1740.067;Inherit;True;IceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-3912.331,-1439.679;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;277;-4226.856,-567.7947;Inherit;False;PannerSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;280;-3935.543,-456.7681;Inherit;False;PannerTime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;313;-4410.098,560.0308;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;321;-4245.056,798.9892;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;318;-4236.718,920.2867;Inherit;False;280;PannerTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;289;-4528.895,73.23942;Inherit;False;Constant;_MagmaTiling;MagmaTiling;14;0;Create;True;0;0;0;False;0;False;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;284;-4538.311,164.16;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.25,0.25;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;292;-4218.637,250.0191;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;269;-3141.711,2010.491;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-3664.33,-1440.679;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;314;-4059.808,662.8994;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;278;-4316.438,6.591311;Inherit;False;277;PannerSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;282;-4548.611,-62.75487;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;279;-4315.879,104.3968;Inherit;False;280;PannerTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;285;-4036.556,162.9791;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;250;-2919.198,2042.594;Inherit;False;Ice;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;211;-3501.816,492.2413;Inherit;False;848.0939;582.665;Comment;7;276;148;172;173;152;156;169;Colored Water;1,1,1,1;0;0
Node;AmplifyShaderEditor.DistanceOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-3415.327,-1437.679;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;109;-3940.603,-1160.005;Inherit;False;Property;_AtmosphereSize;AtmosphereSize;2;0;Create;True;0;0;0;False;0;False;2.11;3.73;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;317;-3827.164,656.5814;Inherit;True;Panner;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;283;-4047.858,-65.93576;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;274;-3787.37,124.6347;Inherit;True;Property;_Surface;Surface;8;0;Create;True;0;0;0;False;0;False;-1;None;fcb3e019a33c36d41a3ebbc5b14f61a0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;251;-3691.63,352.9243;Inherit;False;250;Ice;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-3182.325,-1439.679;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;108;-3656.887,-1195.049;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;173;-3120.946,561.1038;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;148;-3422.753,561.2415;Inherit;True;Property;_Lava;Lava;7;0;Create;True;0;0;0;False;0;False;-1;34c34d8c64649484e858b94f4766f062;34c34d8c64649484e858b94f4766f062;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;276;-3416.479,762.5539;Inherit;True;Property;_Water;Water;6;0;Create;True;0;0;0;False;0;False;-1;6fe629e0670cf8c4d980fd5b34631dcb;6fe629e0670cf8c4d980fd5b34631dcb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;149;-3789.319,-88.47227;Inherit;True;Property;_Magma;Magma;9;0;Create;True;0;0;0;False;0;False;-1;eeb9d96c02a346c4884e4f7a9b316f92;eeb9d96c02a346c4884e4f7a9b316f92;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;138;-3159.674,-106.8675;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;324;-3350.89,116.2162;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;112;-3366.179,-1196.209;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;153;-2903.726,212.744;Inherit;False;146;LandMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;174;-3072.912,-11.44275;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;152;-2881.369,882.3578;Inherit;False;147;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;172;-3082.709,651.2108;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;102;-2912.179,-1428.21;Inherit;True;CircularMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;114;-3132.179,-1201.209;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;157;-2903.751,-12.26535;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;156;-2902.399,644.9897;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;168;-2892.922,-95.00111;Inherit;False;SurfaceColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;169;-2915.355,557.8383;Inherit;False;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;117;-2906.24,-1186.662;Inherit;False;AtmoshphereMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;171;-2035.877,-99.37349;Inherit;False;169;WaterColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;170;-2041.877,-239.3733;Inherit;False;168;SurfaceColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;116;-2236.873,84.10036;Inherit;False;Property;_AtmosphereColor;AtmosphereColor;4;0;Create;True;0;0;0;False;0;False;0,0.4000001,1,1;0,0.8062606,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;119;-2245.906,265.6833;Inherit;False;117;AtmoshphereMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;166;-1784.295,-181.7082;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;120;-1927.905,93.68326;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;129;-1941.179,245.2766;Inherit;False;128;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;167;-1636.684,-181.5861;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;123;-1661.214,105.5845;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;121;-1362.905,-14.31667;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;122;-1178.905,-20.31667;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;325;-1376.037,-332.8138;Inherit;True;Property;_MainTex;_MainTex;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;326;-1015.037,-248.8138;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;103;-1168,496;Inherit;True;102;CircularMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;327;-1248,256;Inherit;False;Property;_Opacity;Opacity;18;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;98;-640,-80;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;Mat_Skin_Default_Earth_Unlit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;False;0;False;;True;1;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;68;0;38;0
WireConnection;68;2;277;0
WireConnection;68;1;280;0
WireConnection;69;1;68;0
WireConnection;162;0;69;2
WireConnection;163;0;69;3
WireConnection;147;0;162;0
WireConnection;146;0;163;0
WireConnection;260;0;255;2
WireConnection;259;0;260;0
WireConnection;265;0;267;0
WireConnection;265;1;266;0
WireConnection;128;0;127;0
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
WireConnection;218;0;215;0
WireConnection;218;1;216;0
WireConnection;315;0;316;0
WireConnection;264;0;262;0
WireConnection;264;1;268;0
WireConnection;208;0;179;0
WireConnection;206;0;205;0
WireConnection;206;1;208;0
WireConnection;219;0;218;0
WireConnection;308;1;315;0
WireConnection;309;0;316;0
WireConnection;263;0;264;0
WireConnection;277;0;206;0
WireConnection;280;0;203;0
WireConnection;313;0;309;0
WireConnection;313;1;308;0
WireConnection;313;2;310;0
WireConnection;321;0;320;0
WireConnection;321;1;316;0
WireConnection;284;0;290;0
WireConnection;292;0;278;0
WireConnection;292;1;290;0
WireConnection;269;0;263;0
WireConnection;269;1;219;0
WireConnection;4;0;3;0
WireConnection;314;0;313;0
WireConnection;314;2;321;0
WireConnection;314;1;318;0
WireConnection;282;0;289;0
WireConnection;285;0;284;0
WireConnection;285;2;292;0
WireConnection;285;1;279;0
WireConnection;250;0;269;0
WireConnection;6;0;4;0
WireConnection;317;0;314;0
WireConnection;283;0;282;0
WireConnection;283;2;278;0
WireConnection;283;1;279;0
WireConnection;274;1;285;0
WireConnection;8;0;6;0
WireConnection;108;0;6;0
WireConnection;108;1;109;0
WireConnection;148;1;317;0
WireConnection;276;1;317;0
WireConnection;149;1;283;0
WireConnection;324;0;274;0
WireConnection;324;1;251;0
WireConnection;324;2;251;0
WireConnection;112;0;108;0
WireConnection;174;0;149;0
WireConnection;174;1;324;0
WireConnection;174;2;138;0
WireConnection;172;0;148;0
WireConnection;172;1;276;0
WireConnection;172;2;173;0
WireConnection;102;0;8;0
WireConnection;114;0;102;0
WireConnection;114;1;112;0
WireConnection;157;0;174;0
WireConnection;157;1;153;0
WireConnection;156;0;172;0
WireConnection;156;1;152;0
WireConnection;168;0;157;0
WireConnection;169;0;156;0
WireConnection;117;0;114;0
WireConnection;166;0;170;0
WireConnection;166;1;171;0
WireConnection;120;0;116;0
WireConnection;120;1;119;0
WireConnection;167;0;166;0
WireConnection;123;0;120;0
WireConnection;123;1;129;0
WireConnection;121;0;167;0
WireConnection;121;1;123;0
WireConnection;122;0;121;0
WireConnection;326;0;325;0
WireConnection;326;1;122;0
WireConnection;98;2;326;0
WireConnection;98;9;327;0
WireConnection;98;10;103;0
ASEEND*/
//CHKSM=947D04D0D71724F0C6CDA6EAC3648E6429B0BBE4