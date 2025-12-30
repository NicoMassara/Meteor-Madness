// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GoldenEarth"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_HealthAmount( "HealthAmount", Range( 0, 1 ) ) = 0
		_EarthMap_tIL( "EarthMap_tIL", 2D ) = "white" {}
		_MainTex( "_MainTex", 2D ) = "white" {}
		_FlowMap( "FlowMap", 2D ) = "white" {}
		_Vector0( "Vector 0", Vector ) = ( 1, -1.31, 0, 0 )
		_Surface( "Surface", 2D ) = "white" {}
		_Magma( "Magma", 2D ) = "white" {}
		_Water( "Water", 2D ) = "white" {}
		_Lava( "Lava", 2D ) = "white" {}
		_FlowIntensity( "Flow Intensity", Range( 0, 1 ) ) = 0
		_WaterTiling1( "WaterTiling", Range( 0.1, 10 ) ) = 5.06
		_Opacity( "Opacity", Range( 0, 1 ) ) = 0
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

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Lava;
		uniform float2 _Vector0;
		uniform float _HealthAmount;
		uniform float _WaterTiling1;
		uniform sampler2D _FlowMap;
		uniform float _FlowIntensity;
		uniform sampler2D _Water;
		uniform float4 _Water_ST;
		uniform sampler2D _EarthMap_tIL;
		uniform sampler2D _Magma;
		uniform float4 _Magma_ST;
		uniform sampler2D _Surface;
		uniform float4 _Surface_ST;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = tex2D( _MainTex, uv_MainTex ).rgb;
			float mulTime28 = _Time.y * 0.1;
			float PannerTime30 = mulTime28;
			float HealthAmount13 = _HealthAmount;
			float2 PannerSpeed29 = ( _Vector0 *  (0.0 + ( HealthAmount13 - 0.0 ) * ( 1.0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float2 temp_cast_1 = (_WaterTiling1).xx;
			float2 uv_TexCoord56 = i.uv_texcoord * temp_cast_1;
			float2 temp_cast_3 = (_WaterTiling1).xx;
			float2 uv_TexCoord53 = i.uv_texcoord * temp_cast_3;
			float4 lerpResult59 = lerp( float4( uv_TexCoord56, 0.0 , 0.0 ) , tex2D( _FlowMap, uv_TexCoord53 ) , _FlowIntensity);
			float2 panner62 = ( PannerTime30 * ( PannerSpeed29 * _WaterTiling1 ) + lerpResult59.rg);
			float2 Panner64 = panner62;
			float2 uv_Water = i.uv_texcoord * _Water_ST.xy + _Water_ST.zw;
			float4 lerpResult69 = lerp( tex2D( _Lava, Panner64 ) , tex2D( _Water, uv_Water ) , HealthAmount13);
			float2 uv_TexCoord16 = i.uv_texcoord * float2( 0.5,1 );
			float2 panner18 = ( PannerTime30 * PannerSpeed29 + uv_TexCoord16);
			float4 tex2DNode19 = tex2D( _EarthMap_tIL, panner18 );
			float WaterMask22 = step( tex2DNode19.g , 0.35 );
			float4 WaterColor71 = ( lerpResult69 * WaterMask22 );
			float2 uv_Magma = i.uv_texcoord * _Magma_ST.xy + _Magma_ST.zw;
			float2 uv_Surface = i.uv_texcoord * _Surface_ST.xy + _Surface_ST.zw;
			float4 lerpResult48 = lerp( tex2D( _Magma, uv_Magma ) , tex2D( _Surface, uv_Surface ) , HealthAmount13);
			float LandMask23 = step( tex2DNode19.b , 0.35 );
			float4 SurfaceColor50 = ( lerpResult48 * LandMask23 );
			float4 Emission76 = saturate( ( WaterColor71 + SurfaceColor50 ) );
			o.Emission = Emission76.rgb;
			o.Alpha = _Opacity;
			float temp_output_4_0 = distance( ( i.uv_texcoord - float2( 0.5,0.5 ) ) , float2( 0,0 ) );
			float CircularMask9 = step( temp_output_4_0 , 0.5 );
			clip( CircularMask9 - _Cutoff );
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
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;14;-2384,-832;Inherit;False;587.001;163.95;Comment;2;12;13;Health Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;12;-2336,-784;Inherit;False;Property;_HealthAmount;HealthAmount;2;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-3200,160;Inherit;False;1164.251;503.9158;Comment;9;30;29;28;27;26;25;24;18;16;Panner;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;13;-2048,-768;Inherit;False;HealthAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;24;-3184,528;Inherit;False;13;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;25;-3120,352;Inherit;False;Property;_Vector0;Vector 0;6;0;Create;True;0;0;0;False;0;False;1,-1.31;0.35,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;26;-2848,480;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;52;-3810.158,1543.298;Inherit;False;1662.074;549.343;Comment;11;64;57;56;55;62;61;60;59;58;54;53;Liquid Panner;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;27;-2768,352;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;28;-2576,512;Inherit;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;29;-2640,384;Inherit;False;PannerSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;53;-3645.052,1798.689;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-3760.158,1606.335;Inherit;False;Property;_WaterTiling1;WaterTiling;12;0;Create;True;0;0;0;False;0;False;5.06;2.75;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-2960,208;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.5,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;30;-2352,496;Inherit;False;PannerTime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;55;-3396.656,1771.425;Inherit;True;Property;_FlowMap;FlowMap;5;0;Create;True;0;0;0;False;0;False;-1;None;a2567c79fcafff541bd097660979fea8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;56;-3360.723,1595.137;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;57;-3380.388,1977.483;Inherit;False;Property;_FlowIntensity;Flow Intensity;11;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;58;-3056,1936;Inherit;False;29;PannerSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-1984,80;Inherit;False;771.6578;591.5536;Comment;5;22;20;23;21;19;Surface / Water Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;18;-2544,208;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;59;-2970.422,1593.298;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;60;-2805.38,1832.257;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;61;-2768,1984;Inherit;False;30;PannerTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;19;-1920,192;Inherit;True;Property;_EarthMap_tIL;EarthMap_tIL;3;0;Create;True;0;0;0;False;0;False;-1;d40864d99dd8121429c2fc384f12c7af;d40864d99dd8121429c2fc384f12c7af;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;62;-2620.132,1696.167;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;32;-2176,736;Inherit;False;1155.376;648.6264;Comment;7;50;47;49;48;45;44;42;Colored Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;63;-2062.139,1525.509;Inherit;False;1054.61;577.4675;Comment;7;71;68;70;69;67;66;65;Colored Water;1,1,1,1;0;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;21;-1600,320;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;20;-1600,208;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;64;-2387.488,1689.849;Inherit;True;Panner;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2128,1024;Inherit;True;Property;_Surface;Surface;7;0;Create;True;0;0;0;False;0;False;-1;None;957642ce28cc87d4bb5a3128cf9aac35;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;23;-1440,384;Inherit;True;LandMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-2128,800;Inherit;True;Property;_Magma;Magma;8;0;Create;True;0;0;0;False;0;False;-1;eeb9d96c02a346c4884e4f7a9b316f92;eeb9d96c02a346c4884e4f7a9b316f92;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;45;-2000,1248;Inherit;False;13;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;22;-1440,144;Inherit;True;WaterMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;-1681.27,1594.371;Inherit;False;13;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;66;-1983.076,1594.509;Inherit;True;Property;_Lava;Lava;10;0;Create;True;0;0;0;False;0;False;-1;34c34d8c64649484e858b94f4766f062;34c34d8c64649484e858b94f4766f062;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;67;-1976.802,1795.821;Inherit;True;Property;_Water;Water;9;0;Create;True;0;0;0;False;0;False;-1;6fe629e0670cf8c4d980fd5b34631dcb;7db87761170876840840edce0bb6f3c8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;48;-1728,912;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;47;-1712,1152;Inherit;True;23;LandMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;69;-1643.032,1684.478;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-1680,1904;Inherit;True;22;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2349.52,-550.426;Inherit;False;1297.689;571.6741;Comment;10;11;10;9;8;7;6;5;4;3;2;Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-1488,912;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;70;-1462.722,1678.257;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-2300.952,-499.426;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;50;-1264,912;Inherit;True;SurfaceColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-1232,1680;Inherit;True;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-2052.951,-500.426;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;73;-1504,2496;Inherit;True;50;SurfaceColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;-1504,2272;Inherit;True;71;WaterColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DistanceOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-1803.948,-497.426;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;74;-1184,2384;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-1570.946,-499.426;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;75;-1056,2384;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;9;-1300.8,-487.957;Inherit;True;CircularMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;76;-896,2384;Inherit;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;5;-2329.224,-219.7521;Inherit;False;Property;_AtmosphereSize;AtmosphereSize;1;0;Create;True;0;0;0;False;0;False;2.11;3.73;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-2045.508,-254.796;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-1754.8,-255.9561;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;-1520.8,-260.9561;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-1294.861,-246.4091;Inherit;True;AtmoshphereMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;51;-480,64;Inherit;True;76;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;77;-538.8051,-188.9604;Inherit;True;Property;_MainTex;_MainTex;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;31;-464,480;Inherit;True;9;CircularMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;78;-452.2073,353.0815;Inherit;False;Property;_Opacity;Opacity;13;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;0,0;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Standard;GoldenEarth;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;True;1;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;12;0
WireConnection;26;0;24;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;29;0;27;0
WireConnection;53;0;54;0
WireConnection;30;0;28;0
WireConnection;55;1;53;0
WireConnection;56;0;54;0
WireConnection;18;0;16;0
WireConnection;18;2;29;0
WireConnection;18;1;30;0
WireConnection;59;0;56;0
WireConnection;59;1;55;0
WireConnection;59;2;57;0
WireConnection;60;0;58;0
WireConnection;60;1;54;0
WireConnection;19;1;18;0
WireConnection;62;0;59;0
WireConnection;62;2;60;0
WireConnection;62;1;61;0
WireConnection;21;0;19;3
WireConnection;20;0;19;2
WireConnection;64;0;62;0
WireConnection;23;0;21;0
WireConnection;22;0;20;0
WireConnection;66;1;64;0
WireConnection;48;0;44;0
WireConnection;48;1;42;0
WireConnection;48;2;45;0
WireConnection;69;0;66;0
WireConnection;69;1;67;0
WireConnection;69;2;65;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;70;0;69;0
WireConnection;70;1;68;0
WireConnection;50;0;49;0
WireConnection;71;0;70;0
WireConnection;3;0;2;0
WireConnection;4;0;3;0
WireConnection;74;0;72;0
WireConnection;74;1;73;0
WireConnection;6;0;4;0
WireConnection;75;0;74;0
WireConnection;9;0;6;0
WireConnection;76;0;75;0
WireConnection;7;0;4;0
WireConnection;7;1;5;0
WireConnection;8;0;7;0
WireConnection;10;0;9;0
WireConnection;10;1;8;0
WireConnection;11;0;10;0
WireConnection;0;0;77;0
WireConnection;0;2;51;0
WireConnection;0;9;78;0
WireConnection;0;10;31;0
ASEEND*/
//CHKSM=A40E31980261C7FA60243BC216C6834ED2A88A59