// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EarthMeteorShader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_AtmosphereSize( "AtmosphereSize", Range( 1, 10 ) ) = 2.11
		_EarthMap_tIL( "EarthMap_tIL", 2D ) = "white" {}
		_Vector0( "Vector 0", Vector ) = ( 1, -1.31, 0, 0 )
		_FlowMap( "FlowMap", 2D ) = "white" {}
		_Surface( "Surface", 2D ) = "white" {}
		_AtmosphereColor( "AtmosphereColor", Color ) = ( 0, 0.4000001, 1, 1 )
		_SolidTop( "_SolidTop", Range( 0, 1 ) ) = 0.1373401
		_Water( "Water", 2D ) = "white" {}
		_TopFadeLenght( "_TopFadeLenght", Range( 0, 1 ) ) = 0.3475586
		_BottomFadeLenght( "_BottomFadeLenght", Range( 0, 1 ) ) = 0
		_SolidBottom( "_SolidBottom", Range( 0, 1 ) ) = 0.3649352
		_MainTex( "_MainTex", 2D ) = "white" {}
		_FlowIntensity( "Flow Intensity", Range( 0, 1 ) ) = 0
		_WaterTiling1( "WaterTiling", Range( 0.1, 10 ) ) = 5.06
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.5
		#define ASE_VERSION 19905
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Surface;
		uniform float2 _Vector0;
		uniform float _SolidTop;
		uniform float _TopFadeLenght;
		uniform float _SolidBottom;
		uniform float _BottomFadeLenght;
		uniform sampler2D _EarthMap_tIL;
		uniform sampler2D _Water;
		uniform float _WaterTiling1;
		uniform sampler2D _FlowMap;
		uniform float _FlowIntensity;
		uniform float4 _AtmosphereColor;
		uniform float _AtmosphereSize;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float mulTime26 = _Time.y * 0.1;
			float PannerTime28 = mulTime26;
			float2 PannerSpeed27 = ( _Vector0 *  (0.0 + ( 1.0 - 0.0 ) * ( 1.0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float2 temp_cast_0 = (0.25).xx;
			float2 uv_TexCoord32 = i.uv_texcoord * temp_cast_0;
			float2 panner37 = ( PannerTime28 * ( PannerSpeed27 * 0.25 ) + uv_TexCoord32);
			float temp_output_58_0 = ( 1.0 * 0.5 );
			float temp_output_2_0_g14 = ( 1.0 -  (0.0 + ( _SolidTop - 0.0 ) * ( temp_output_58_0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float smoothstepResult5_g14 = smoothstep( temp_output_2_0_g14 , ( temp_output_2_0_g14 +  (0.0 + ( _TopFadeLenght - 0.0 ) * ( 0.25 - 0.0 ) / ( 1.0 - 0.0 ) ) ) , i.uv_texcoord.y);
			float temp_output_2_0_g15 = ( 1.0 -  (0.0 + ( _SolidBottom - 0.0 ) * ( temp_output_58_0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			float smoothstepResult5_g15 = smoothstep( temp_output_2_0_g15 , ( temp_output_2_0_g15 +  (0.0 + ( _BottomFadeLenght - 0.0 ) * ( 0.25 - 0.0 ) / ( 1.0 - 0.0 ) ) ) , ( 1.0 - i.uv_texcoord.y ));
			float IceMask69 = ( saturate( smoothstepResult5_g14 ) + saturate( smoothstepResult5_g15 ) );
			float2 uv_TexCoord15 = i.uv_texcoord * float2( 0.5,1 );
			float2 panner16 = ( PannerTime28 * PannerSpeed27 + uv_TexCoord15);
			float4 tex2DNode17 = tex2D( _EarthMap_tIL, panner16 );
			float LandMask21 = step( tex2DNode17.b , 0.35 );
			float WaterMask20 = step( tex2DNode17.g , 0.35 );
			float Ice71 = ( IceMask69 * saturate( ( ( LandMask21 * 1.0 ) + ( WaterMask20 * 0.5 ) ) ) );
			float4 temp_cast_1 = (Ice71).xxxx;
			float4 lerpResult43 = lerp( tex2D( _Surface, panner37 ) , temp_cast_1 , Ice71);
			float4 SurfaceColor47 = ( lerpResult43 * LandMask21 );
			float2 temp_cast_2 = (_WaterTiling1).xx;
			float2 uv_TexCoord93 = i.uv_texcoord * temp_cast_2;
			float2 temp_cast_4 = (_WaterTiling1).xx;
			float2 uv_TexCoord90 = i.uv_texcoord * temp_cast_4;
			float4 lerpResult96 = lerp( float4( uv_TexCoord93, 0.0 , 0.0 ) , tex2D( _FlowMap, uv_TexCoord90 ) , _FlowIntensity);
			float2 panner99 = ( PannerTime28 * ( PannerSpeed27 * _WaterTiling1 ) + lerpResult96.rg);
			float2 Panner101 = panner99;
			float4 WaterColor108 = ( tex2D( _Water, Panner101 ) * WaterMask20 );
			float temp_output_4_0 = distance( ( i.uv_texcoord - float2( 0.5,0.5 ) ) , float2( 0,0 ) );
			float CircularMask9 = step( temp_output_4_0 , 0.5 );
			float AtmoshphereMask11 = ( CircularMask9 * ( pow( temp_output_4_0 , _AtmosphereSize ) * 5.0 ) );
			float4 Emission88 = ( tex2D( _MainTex, uv_MainTex ) * saturate( ( saturate( ( SurfaceColor47 + WaterColor108 ) ) + ( _AtmosphereColor * AtmoshphereMask11 ) ) ) );
			o.Emission = Emission88.rgb;
			o.Alpha = 1;
			clip( CircularMask9 - _Cutoff );
		}

		ENDCG
	}
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;13;-4048,-256;Inherit;False;1164.251;503.9158;Comment;8;28;27;26;25;24;23;16;15;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-3808,-208;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.5,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;14;-2848,-288;Inherit;False;933.5408;593.2576;Comment;5;21;20;19;18;17;Surface / Water Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-3392,-208;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-3920,1232;Inherit;False;1458.345;1149.663;Comment;21;71;70;69;68;67;66;65;64;63;62;61;60;59;58;57;56;55;54;52;51;50;Ice Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-2624,-176;Inherit;True;Property;_EarthMap_tIL;EarthMap_tIL;2;0;Create;True;0;0;0;False;0;False;-1;d40864d99dd8121429c2fc384f12c7af;d40864d99dd8121429c2fc384f12c7af;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;18;-2304,-160;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;19;-2304,-64;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;50;-3872,1280;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;20;-2144,-224;Inherit;True;WaterMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;21;-2160,16;Inherit;True;LandMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;51;-3632,1568;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;52;-3504,1952;Inherit;False;Constant;_SolidMax;_SolidMax;20;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;23;-3968,-48;Inherit;False;Property;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;1,-1.31;0.35,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-3520,1392;Inherit;False;Property;_SolidTop;_SolidTop;7;0;Create;True;0;0;0;False;0;False;0.1373401;0.9415613;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;55;-3520,1648;Inherit;False;Property;_SolidBottom;_SolidBottom;11;0;Create;True;0;0;0;False;0;False;0.3649352;0.4006471;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;56;-3424,1568;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;57;-3504,1744;Inherit;False;Property;_BottomFadeLenght;_BottomFadeLenght;10;0;Create;True;0;0;0;False;0;False;0;0.8013729;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;59;-3824,2160;Inherit;False;20;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;60;-3824,2064;Inherit;False;21;LandMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;61;-3536,1488;Inherit;False;Property;_TopFadeLenght;_TopFadeLenght;9;0;Create;True;0;0;0;False;0;False;0.3475586;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;58;-3232,1808;Inherit;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;24;-3696,64;Inherit;False;5;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;89;-6592,1792;Inherit;False;1662.074;549.343;Comment;11;101;99;98;97;96;95;94;93;92;91;90;Liquid Panner;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;25;-3616,-64;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;62;-3152,1328;Inherit;False;FadeMask;-1;;14;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;63;-3552,2064;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;64;-3552,2160;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;-3168,1616;Inherit;False;FadeMask;-1;;15;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;29;-3920,464;Inherit;False;1960.634;608.4853;Comment;12;47;46;44;43;40;39;37;33;32;30;48;36;Colored Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;26;-3424,96;Inherit;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;27;-3488,-32;Inherit;False;PannerSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;66;-3360,2096;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;67;-2880,1488;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;90;-6432,2048;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;91;-6544,1856;Inherit;False;Property;_WaterTiling1;WaterTiling;14;0;Create;True;0;0;0;False;0;False;5.06;2.742433;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2416,1280;Inherit;False;1297.689;571.6741;Comment;10;11;10;9;8;7;6;5;4;3;2;Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;28;-3184,80;Inherit;False;PannerTime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;30;-3872,752;Inherit;False;Constant;_SurfaceTiling;SurfaceTiling;14;0;Create;True;0;0;0;False;0;False;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;48;-3808,880;Inherit;False;27;PannerSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;69;-2976,1840;Inherit;True;IceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;-3216,2096;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;92;-6176,2032;Inherit;True;Property;_FlowMap;FlowMap;4;0;Create;True;0;0;0;False;0;False;-1;None;a2567c79fcafff541bd097660979fea8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;93;-6144,1856;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;94;-6160,2224;Inherit;False;Property;_FlowIntensity;Flow Intensity;13;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;95;-5808,2064;Inherit;False;27;PannerSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-2368,1328;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;32;-3888,592;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.25,0.25;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;33;-3696,720;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;70;-2960,2112;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;36;-3552,896;Inherit;False;28;PannerTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;96;-5744,1840;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;97;-5584,2080;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;98;-5584,2208;Inherit;False;28;PannerTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-2112,1328;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;37;-3488,592;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;-2736,2144;Inherit;True;Ice;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;99;-5392,1952;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-1856,1344;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;100;-4848,1776;Inherit;False;848.0939;582.665;Comment;4;108;107;105;104;Colored Water;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;5;-2384,1616;Inherit;False;Property;_AtmosphereSize;AtmosphereSize;1;0;Create;True;0;0;0;False;0;False;2.11;3.73;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-3264,576;Inherit;True;Property;_Surface;Surface;5;0;Create;True;0;0;0;False;0;False;-1;None;fcb3e019a33c36d41a3ebbc5b14f61a0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-3168,816;Inherit;True;71;Ice;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;101;-5168,1936;Inherit;True;Panner;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-1632,1328;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-2112,1584;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-2880,576;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-2816,832;Inherit;True;21;LandMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;104;-4816,1952;Inherit;True;Property;_Water;Water;8;0;Create;True;0;0;0;False;0;False;-1;6fe629e0670cf8c4d980fd5b34631dcb;6fe629e0670cf8c4d980fd5b34631dcb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;105;-4752,2208;Inherit;False;20;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;9;-1360,1344;Inherit;True;CircularMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-1808,1584;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;46;-2560,592;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;107;-4512,1952;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;109;-3574.268,2823.786;Inherit;False;1512.268;793.1641;Comment;12;74;75;76;77;81;83;85;88;72;82;79;80;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;-1584,1568;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;47;-2192,528;Inherit;True;SurfaceColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;108;-4256,1952;Inherit;True;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-1360,1584;Inherit;True;AtmoshphereMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;81;-3524.268,2967.226;Inherit;False;47;SurfaceColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;80;-3518.268,3107.226;Inherit;False;108;WaterColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;83;-3266.686,3024.891;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;-3488,3504;Inherit;False;11;AtmoshphereMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;79;-3520,3248;Inherit;False;Property;_AtmosphereColor;AtmosphereColor;6;0;Create;True;0;0;0;False;0;False;0,0.4000001,1,1;0,0.8062606,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;85;-3119.075,3025.013;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;82;-3168,3328;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;74;-2845.296,3192.283;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;75;-2661.296,3186.283;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;76;-2858.428,2873.786;Inherit;True;Property;_MainTex;_MainTex;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;77;-2497.428,2957.786;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;88;-2304,2960;Inherit;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;12;-326.6296,424.8711;Inherit;True;9;CircularMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;110;-352,64;Inherit;True;88;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;0,0;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Standard;EarthMeteorShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;False;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;15;0
WireConnection;16;2;27;0
WireConnection;16;1;28;0
WireConnection;17;1;16;0
WireConnection;18;0;17;2
WireConnection;19;0;17;3
WireConnection;20;0;18;0
WireConnection;21;0;19;0
WireConnection;51;0;50;2
WireConnection;56;0;51;0
WireConnection;58;1;52;0
WireConnection;25;0;23;0
WireConnection;25;1;24;0
WireConnection;62;8;50;2
WireConnection;62;9;54;0
WireConnection;62;10;61;0
WireConnection;62;12;58;0
WireConnection;63;0;60;0
WireConnection;64;0;59;0
WireConnection;65;8;56;0
WireConnection;65;9;55;0
WireConnection;65;10;57;0
WireConnection;65;12;58;0
WireConnection;27;0;25;0
WireConnection;66;0;63;0
WireConnection;66;1;64;0
WireConnection;67;0;62;0
WireConnection;67;1;65;0
WireConnection;90;0;91;0
WireConnection;28;0;26;0
WireConnection;69;0;67;0
WireConnection;68;0;66;0
WireConnection;92;1;90;0
WireConnection;93;0;91;0
WireConnection;32;0;30;0
WireConnection;33;0;48;0
WireConnection;33;1;30;0
WireConnection;70;0;69;0
WireConnection;70;1;68;0
WireConnection;96;0;93;0
WireConnection;96;1;92;0
WireConnection;96;2;94;0
WireConnection;97;0;95;0
WireConnection;97;1;91;0
WireConnection;3;0;2;0
WireConnection;37;0;32;0
WireConnection;37;2;33;0
WireConnection;37;1;36;0
WireConnection;71;0;70;0
WireConnection;99;0;96;0
WireConnection;99;2;97;0
WireConnection;99;1;98;0
WireConnection;4;0;3;0
WireConnection;39;1;37;0
WireConnection;101;0;99;0
WireConnection;6;0;4;0
WireConnection;7;0;4;0
WireConnection;7;1;5;0
WireConnection;43;0;39;0
WireConnection;43;1;40;0
WireConnection;43;2;40;0
WireConnection;104;1;101;0
WireConnection;9;0;6;0
WireConnection;8;0;7;0
WireConnection;46;0;43;0
WireConnection;46;1;44;0
WireConnection;107;0;104;0
WireConnection;107;1;105;0
WireConnection;10;0;9;0
WireConnection;10;1;8;0
WireConnection;47;0;46;0
WireConnection;108;0;107;0
WireConnection;11;0;10;0
WireConnection;83;0;81;0
WireConnection;83;1;80;0
WireConnection;85;0;83;0
WireConnection;82;0;79;0
WireConnection;82;1;72;0
WireConnection;74;0;85;0
WireConnection;74;1;82;0
WireConnection;75;0;74;0
WireConnection;77;0;76;0
WireConnection;77;1;75;0
WireConnection;88;0;77;0
WireConnection;0;2;110;0
WireConnection;0;10;12;0
ASEEND*/
//CHKSM=F7DA09B10818DFBD46ED4BB6176EE4B8829BDD5C