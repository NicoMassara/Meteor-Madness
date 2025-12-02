// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Super_Shield_Lit"
{
	Properties
	{
		_DefaultColor( "DefaultColor", Color ) = ( 0, 0.9388051, 1, 1 )
		[HDR] _EmissionColor( "EmissionColor", Color ) = ( 0, 16, 15.68666, 1 )
		_ColorMultipier( "ColorMultipier", Range( 0, 1 ) ) = 1
		_Opacity( "Opacity", Range( 0, 1 ) ) = 1
		_RotationSpeed( "RotationSpeed", Range( 0, 10 ) ) = 10
		_LinesSpeed( "LinesSpeed", Range( 0, 1 ) ) = 0
		_CrossOffset( "CrossOffset", Range( 0, 1 ) ) = 0
		_CrossIntensity( "CrossIntensity", Range( 0, 1 ) ) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
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

		uniform float4 _DefaultColor;
		uniform float _RotationSpeed;
		uniform float _CrossOffset;
		uniform float _CrossIntensity;
		uniform float _ColorMultipier;
		uniform float _LinesSpeed;
		uniform float4 _EmissionColor;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (0.5).xx;
			float2 uv_TexCoord527 = i.uv_texcoord + temp_cast_0;
			float2 temp_cast_1 = (0.5).xx;
			float mulTime113 = _Time.y * _RotationSpeed;
			float RotationTimeSpeed300 = mulTime113;
			float cos526 = cos( RotationTimeSpeed300 );
			float sin526 = sin( RotationTimeSpeed300 );
			float2 rotator526 = mul( ( uv_TexCoord527 - float2( 0.5,0.5 ) ) - temp_cast_1 , float2x2( cos526 , -sin526 , sin526 , cos526 )) + temp_cast_1;
			float2 break521 = rotator526;
			float temp_output_7_0_g1 = break521.x;
			float temp_output_11_0_g1 =  (0.51 + ( _CrossOffset - 0.0 ) * ( 1.0 - 0.51 ) / ( 1.0 - 0.0 ) );
			float temp_output_7_0_g2 = break521.y;
			float temp_output_11_0_g2 =  (0.51 + ( _CrossOffset - 0.0 ) * ( 1.0 - 0.51 ) / ( 1.0 - 0.0 ) );
			float CrossMask538 = ( 1.0 - ( saturate( ( step( temp_output_7_0_g1 , temp_output_11_0_g1 ) * ( 1.0 - step( temp_output_7_0_g1 , abs( ( temp_output_11_0_g1 - 1.0 ) ) ) ) ) ) + saturate( ( step( temp_output_7_0_g2 , temp_output_11_0_g2 ) * ( 1.0 - step( temp_output_7_0_g2 , abs( ( temp_output_11_0_g2 - 1.0 ) ) ) ) ) ) ) );
			float4 lerpResult544 = lerp( _DefaultColor , _DefaultColor , ( CrossMask538 * _CrossIntensity ));
			float4 Albedo415 = lerpResult544;
			o.Albedo = Albedo415.rgb;
			float mulTime166 = _Time.y *  (0.01 + ( _LinesSpeed - 0.0 ) * ( 0.05 - 0.01 ) / ( 1.0 - 0.0 ) );
			float Lines132 = saturate( sin( ( ( length( ( i.uv_texcoord - float2( 0.5,0.5 ) ) ) + mulTime166 ) * 500.0 ) ) );
			float4 Emission56 = ( saturate( (  (0.05 + ( _ColorMultipier - 0.0 ) * ( 0.25 - 0.05 ) / ( 1.0 - 0.0 ) ) * Lines132 * CrossMask538 ) ) * _EmissionColor );
			o.Emission = Emission56.rgb;
			float temp_output_3_0 = length( ( i.uv_texcoord - float2( 0.5,0.5 ) ) );
			float smoothstepResult4 = smoothstep( 0.5 , 0.45 , temp_output_3_0);
			float smoothstepResult44 = smoothstep( 0.39 , 0.45 , temp_output_3_0);
			float CircleMask17 = ( saturate( ( smoothstepResult4 * smoothstepResult44 ) ) * 2.0 );
			float Opacity55 = ( CircleMask17 * _Opacity );
			o.Alpha = Opacity55;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;171;-4912.96,-440.0243;Inherit;False;1381.031;496.1971;Comment;12;132;140;125;136;126;168;166;169;170;147;148;146;Lines;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;116;-3490.203,-688.4008;Inherit;False;983.0366;411.3143;Comment;7;114;113;112;107;109;108;300;Rotator;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;562;-5603.173,722.4761;Inherit;False;2095.877;380.4474;Comment;13;521;522;523;524;534;529;526;528;527;525;539;538;533;Cross Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;146;-4862.96,-382.5276;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;169;-4774.167,-135.6268;Inherit;False;Property;_LinesSpeed;LinesSpeed;5;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;112;-3472,-368;Inherit;False;Property;_RotationSpeed;RotationSpeed;4;0;Create;True;0;0;0;False;0;False;10;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;147;-4589.961,-381.5276;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;170;-4454.167,-146.6268;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.01;False;4;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;533;-5553.173,885.3088;Inherit;False;Constant;_Anchor;Anchor;7;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;113;-3456,-496;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;52;-3486.197,-225.4303;Inherit;False;1199.085;328.0595;Comment;9;4;44;42;43;49;17;1;2;3;Circle Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.LengthOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;148;-4580.461,-289.5277;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;166;-4439.167,-217.627;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;527;-5333.636,778.3763;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;300;-3056,-496;Inherit;False;RotationTimeSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-3410.085,-175.4303;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;136;-4196.459,-262.2891;Inherit;False;Constant;_LinesMultiplier;LinesMultiplier;8;0;Create;True;0;0;0;False;0;False;500;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;168;-4351.167,-381.6272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;534;-5082.032,920.6456;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;529;-5296.877,987.7634;Inherit;False;300;RotationTimeSpeed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;528;-5093.837,777.5762;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-3436.197,-33.89831;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;126;-4147.963,-379.037;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;50;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;526;-4899.032,772.4761;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-3286.197,-33.89831;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;125;-3926.952,-386.037;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;521;-4686.838,780.0118;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;524;-4663.838,903.0118;Inherit;False;Property;_CrossOffset;CrossOffset;6;0;Create;True;0;0;0;False;0;False;0;0.212;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-3110.726,-173.5383;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;140;-3934.932,-306.7075;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;522;-4337.838,781.0118;Inherit;False;BothSideMask;-1;;1;3e0ea21595357314d8c7fa660e07d472;0;2;7;FLOAT;0;False;8;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;523;-4344.838,874.0118;Inherit;False;BothSideMask;-1;;2;3e0ea21595357314d8c7fa660e07d472;0;2;7;FLOAT;0;False;8;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-3102.004,-53.85074;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.39;False;2;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;438;-4911.246,110.0255;Inherit;False;1377.314;538.3544;Comment;10;131;550;431;430;410;448;56;551;558;559;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2880.505,-163.5508;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;132;-3771.322,-390.0243;Inherit;True;Lines;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;525;-4094.413,806.3624;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-2884.703,-59.25073;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;410;-4868.486,170.4382;Inherit;False;Property;_ColorMultipier;ColorMultipier;2;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;131;-4806.881,318.883;Inherit;True;132;Lines;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;539;-3931.969,811.4695;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;561;-4903.717,-1148.114;Inherit;False;1028.434;593.8402;Comment;6;541;540;415;8;542;544;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;431;-4570.137,168.3656;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.05;False;4;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;550;-4570.408,412.9748;Inherit;True;538;CrossMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;551;-4413.21,323.5914;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;538;-3746.697,806.1791;Inherit;True;CrossMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-2669.105,-167.3025;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;59;-3477.281,195.5533;Inherit;False;939.8855;426.1968;Comment;4;445;446;55;18;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-2496.512,-170.9001;Inherit;True;CircleMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;430;-4286.071,170.1703;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;541;-4853.717,-669.4337;Inherit;False;Property;_CrossIntensity;CrossIntensity;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;542;-4837.483,-885.85;Inherit;True;538;CrossMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;446;-3444.352,354.8037;Inherit;False;Property;_Opacity;Opacity;3;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;448;-4132.85,169.3506;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;558;-4227.188,344.6284;Inherit;False;Property;_EmissionColor;EmissionColor;1;1;[HDR];Create;True;0;0;0;False;0;False;0,16,15.68666,1;0,16,15.68666,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;540;-4542.717,-767.4341;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-4824.383,-1076.424;Inherit;False;Property;_DefaultColor;DefaultColor;0;0;Create;True;0;0;0;False;0;False;0,0.9388051,1,1;0,0.9388051,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;18;-3432.594,268.9039;Inherit;False;17;CircleMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;559;-3925.188,168.6284;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;544;-4307.483,-1078.85;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;445;-3069.054,264.7115;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;55;-2768.936,259.6318;Inherit;True;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;56;-3755.601,168.5375;Inherit;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;415;-4114.683,-1098.114;Inherit;True;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;114;-2745.573,-630.4008;Inherit;True;Rotator;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-1406.235,-52.15887;Inherit;True;Property;_MainTex;_MainTex;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToTexture2D;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RotatorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;107;-2998.202,-626.0006;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;108;-3440.203,-627.0006;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;109;-3173.204,-629.0006;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;57;-1952,368;Inherit;True;56;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;53;-1952,560;Inherit;True;55;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;414;-1952,160;Inherit;True;415;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;-1458.43,280.4732;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;Mat_Super_Shield_Lit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;8;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;147;0;146;0
WireConnection;170;0;169;0
WireConnection;113;0;112;0
WireConnection;148;0;147;0
WireConnection;166;0;170;0
WireConnection;527;1;533;0
WireConnection;300;0;113;0
WireConnection;168;0;148;0
WireConnection;168;1;166;0
WireConnection;534;0;533;0
WireConnection;528;0;527;0
WireConnection;2;0;1;0
WireConnection;126;0;168;0
WireConnection;126;1;136;0
WireConnection;526;0;528;0
WireConnection;526;1;534;0
WireConnection;526;2;529;0
WireConnection;3;0;2;0
WireConnection;125;0;126;0
WireConnection;521;0;526;0
WireConnection;4;0;3;0
WireConnection;140;0;125;0
WireConnection;522;7;521;0
WireConnection;522;8;524;0
WireConnection;523;7;521;1
WireConnection;523;8;524;0
WireConnection;44;0;3;0
WireConnection;42;0;4;0
WireConnection;42;1;44;0
WireConnection;132;0;140;0
WireConnection;525;0;522;0
WireConnection;525;1;523;0
WireConnection;43;0;42;0
WireConnection;539;0;525;0
WireConnection;431;0;410;0
WireConnection;551;0;131;0
WireConnection;538;0;539;0
WireConnection;49;0;43;0
WireConnection;17;0;49;0
WireConnection;430;0;431;0
WireConnection;430;1;551;0
WireConnection;430;2;550;0
WireConnection;448;0;430;0
WireConnection;540;0;542;0
WireConnection;540;1;541;0
WireConnection;559;0;448;0
WireConnection;559;1;558;0
WireConnection;544;0;8;0
WireConnection;544;1;8;0
WireConnection;544;2;540;0
WireConnection;445;0;18;0
WireConnection;445;1;446;0
WireConnection;55;0;445;0
WireConnection;56;0;559;0
WireConnection;415;0;544;0
WireConnection;114;0;107;0
WireConnection;107;0;109;0
WireConnection;107;2;300;0
WireConnection;109;0;108;0
WireConnection;0;0;414;0
WireConnection;0;2;57;0
WireConnection;0;9;53;0
ASEEND*/
//CHKSM=15E67F8BF2A195664C6A3D7B2903D072C8992084