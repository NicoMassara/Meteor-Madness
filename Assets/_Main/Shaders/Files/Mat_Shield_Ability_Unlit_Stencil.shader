// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Shield_Ability_Unlit_Stencil"
{
	Properties
	{
		_EffectFrequency( "EffectFrequency", Range( 0, 1 ) ) = 0
		_AbilityColor( "AbilityColor", Color ) = ( 0.0009765625, 0, 0, 0 )
		_EffectSpeed( "EffectSpeed", Range( 0, 1 ) ) = 6
		_EffectWidth( "EffectWidth", Range( 0, 1 ) ) = 0.4235294
		_Opacity( "Opacity", Range( 0, 1 ) ) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Stencil
		{
			Ref 1
			Comp Equal
			Pass Keep
		}
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

		uniform float4 _AbilityColor;
		uniform float _EffectFrequency;
		uniform float _EffectSpeed;
		uniform float _EffectWidth;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 AbilityColor10 = _AbilityColor;
			float mulTime47 = _Time.y *  (0.0 + ( _EffectSpeed - 0.0 ) * ( 30.0 - 0.0 ) / ( 1.0 - 0.0 ) );
			float Effect44 = step( sin( ( ( i.uv_texcoord.x *  (10.0 + ( _EffectFrequency - 0.0 ) * ( 100.0 - 10.0 ) / ( 1.0 - 0.0 ) ) ) + mulTime47 ) ) ,  (-0.75 + ( _EffectWidth - 0.0 ) * ( 0.5 - -0.75 ) / ( 1.0 - 0.0 ) ) );
			float4 Emission59 = saturate( ( AbilityColor10 + ( AbilityColor10 * ( ( 1.0 - 0.0 ) * Effect44 ) ) ) );
			o.Emission = Emission59.rgb;
			float HideMask37 = saturate( ( i.uv_texcoord.x +  (-1.0 + ( _Opacity - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) );
			float OpacityMask64 = ( HideMask37 * Effect44 );
			o.Alpha = OpacityMask64;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

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
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-2966.491,14.69378;Inherit;False;1441.727;646.1461;Comment;13;52;51;50;49;48;47;46;45;44;43;42;41;40;Effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2912.955,427.9466;Inherit;False;Property;_EffectSpeed;EffectSpeed;2;0;Create;True;0;0;0;False;0;False;6;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;46;-2916.491,251.5756;Inherit;False;Property;_EffectFrequency;EffectFrequency;0;0;Create;True;0;0;0;False;0;False;0;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-2602.16,245.6182;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;10;False;4;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;51;-2578.213,440.9788;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;30;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;52;-2932.638,88.40231;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-2418.88,116.5462;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;47;-2360.67,379.65;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;41;-2201.411,113.9644;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;48;-2159.265,470.3788;Inherit;False;Property;_EffectWidth;EffectWidth;3;0;Create;True;0;0;0;False;0;False;0.4235294;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-2963.478,752.0388;Inherit;False;1082.843;381.4408;Comment;6;39;38;37;36;28;72;Hide Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-1987.264,242.379;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.75;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;50;-2070.881,117.5462;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;45;-1916.264,113.3789;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-2931.478,973.9276;Inherit;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-212.9584,-302.9102;Inherit;False;525.988;257.8;Comment;2;11;10;AbilityColor;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-2956.539,-808.9859;Inherit;False;1447.162;750.7947;Comment;9;62;61;59;58;57;56;55;54;73;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-1764.893,89.63538;Inherit;True;Effect;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;28;-2904.707,816.0389;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;36;-2611.107,931.6415;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-162.9583,-252.9102;Inherit;False;Property;_AbilityColor;AbilityColor;1;0;Create;True;0;0;0;False;0;False;0.0009765625,0,0,0;1,0.9215686,0.01568628,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;38;-2460.564,821.7197;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;73;-2744.532,-445.3108;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;62;-2912,-304;Inherit;True;44;Effect;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;73.62952,-245.0042;Inherit;False;AbilityColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-2490.052,-372.0225;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;-2304.052,827.7043;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;55;-2496,-560;Inherit;True;10;AbilityColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-2959.342,1187.387;Inherit;False;839.1813;486.5867;Comment;4;69;68;64;70;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;56;-2229.357,-430.4239;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;37;-2109.241,817.0218;Inherit;True;HideMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;61;-2496,-752;Inherit;True;10;AbilityColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;57;-2049.341,-588.7851;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-2908.321,1237.387;Inherit;True;37;HideMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;70;-2909.342,1443.974;Inherit;True;44;Effect;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;58;-1906.438,-591.1286;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;69;-2548.321,1354.387;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;59;-1719.474,-587.5543;Inherit;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;64;-2359.561,1343.496;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-246.0276,33.62983;Inherit;False;59;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;-300.9866,347.402;Inherit;False;64;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;0,0;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;Mat_Shield_Ability_Unlit_Stencil;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;True;1;False;;255;False;;255;False;;5;False;;1;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;43;0;46;0
WireConnection;51;0;42;0
WireConnection;40;0;52;1
WireConnection;40;1;43;0
WireConnection;47;0;51;0
WireConnection;41;0;40;0
WireConnection;41;1;47;0
WireConnection;49;0;48;0
WireConnection;50;0;41;0
WireConnection;45;0;50;0
WireConnection;45;1;49;0
WireConnection;44;0;45;0
WireConnection;36;0;39;0
WireConnection;38;0;28;1
WireConnection;38;1;36;0
WireConnection;10;0;11;0
WireConnection;54;0;73;0
WireConnection;54;1;62;0
WireConnection;72;0;38;0
WireConnection;56;0;55;0
WireConnection;56;1;54;0
WireConnection;37;0;72;0
WireConnection;57;0;61;0
WireConnection;57;1;56;0
WireConnection;58;0;57;0
WireConnection;69;0;68;0
WireConnection;69;1;70;0
WireConnection;59;0;58;0
WireConnection;64;0;69;0
WireConnection;0;2;8;0
WireConnection;0;9;65;0
ASEEND*/
//CHKSM=27A30ADF83C600EF23967BB7509C4EC5B111B2BB