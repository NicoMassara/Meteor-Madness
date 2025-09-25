// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EarthAtmosphere"
{
	Properties
	{
		_HealthAmount("HealthAmount", Range( 0 , 1)) = 0
		_Size("Size", Range( 0 , 2)) = 2
		[HDR]_DefaultAtmosphere("DefaultAtmosphere", Color) = (0,0.4753861,1,0)
		[HDR]_DamageAtmosphere("DamageAtmosphere", Color) = (1,0,0,1)
		_Intensity("Intensity", Range( 0.5 , 2)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 viewDir;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _Intensity;
		uniform float _Size;
		uniform float4 _DamageAtmosphere;
		uniform float4 _DefaultAtmosphere;
		uniform float _HealthAmount;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV5 = dot( ( ase_worldNormal / _Intensity ), i.viewDir );
			float fresnelNode5 = ( 0.0 + _Size * pow( 1.0 - fresnelNdotV5, 5.0 ) );
			float temp_output_9_0 = ( fresnelNode5 * _Size );
			float4 lerpResult10 = lerp( _DamageAtmosphere , _DefaultAtmosphere , _HealthAmount);
			float4 SelectedColor28 = lerpResult10;
			float4 Atmosthere13 = saturate( ( temp_output_9_0 * SelectedColor28 ) );
			o.Emission = Atmosthere13.rgb;
			o.Smoothness = 0.0;
			float Mask17 = temp_output_9_0;
			o.Alpha = Mask17;
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
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
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
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
-1080;-312;1080;1851;2605.225;197.3914;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;29;-2385.521,457.4287;Inherit;False;828.0447;469.5275;Comment;5;10;8;6;15;28;ColorSelect;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1;-2397.612,-56.0346;Inherit;False;1686.716;480.2203;Comment;12;27;17;13;12;11;9;5;3;2;4;32;33;Atmosthere;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;2;-2375.559,-1.683052;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;8;-2335.521,526.3848;Inherit;False;Property;_DefaultAtmosphere;DefaultAtmosphere;2;1;[HDR];Create;True;0;0;0;False;0;False;0,0.4753861,1,0;0,0.4753861,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;-2330.737,320.5914;Inherit;False;Property;_Intensity;Intensity;4;0;Create;True;0;0;0;False;0;False;0;2;0.5;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2052.554,744.2496;Inherit;False;Property;_HealthAmount;HealthAmount;0;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-2332.761,714.9575;Inherit;False;Property;_DamageAtmosphere;DamageAtmosphere;3;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0,1;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-2011.612,301.9656;Inherit;False;Property;_Size;Size;1;0;Create;True;0;0;0;False;0;False;2;1.15;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;-1960.25,511.3447;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;4;-2372.559,159.3165;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;33;-2171.737,3.59137;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-1781.476,507.4296;Inherit;False;SelectedColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;5;-2027.341,49.52071;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-1467.453,215.2124;Inherit;False;28;SelectedColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1693.648,46.10042;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1248.376,44.14949;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;12;-1111.797,42.41951;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-934.7961,37.41951;Inherit;True;Atmosthere;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1470.137,121.5401;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-297.6744,-22.15421;Inherit;False;13;Atmosthere;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-292.4138,330.3553;Inherit;False;17;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-308.8914,119.4695;Inherit;False;Constant;_Smoothnes;Smoothnes;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;EarthAtmosphere;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;6;0
WireConnection;10;1;8;0
WireConnection;10;2;15;0
WireConnection;33;0;2;0
WireConnection;33;1;32;0
WireConnection;28;0;10;0
WireConnection;5;0;33;0
WireConnection;5;4;4;0
WireConnection;5;2;3;0
WireConnection;9;0;5;0
WireConnection;9;1;3;0
WireConnection;11;0;9;0
WireConnection;11;1;27;0
WireConnection;12;0;11;0
WireConnection;13;0;12;0
WireConnection;17;0;9;0
WireConnection;0;2;14;0
WireConnection;0;4;35;0
WireConnection;0;9;18;0
ASEEND*/
//CHKSM=F5BCA0414F31B55F9CCBDEDFEC7B7980FA3EB839