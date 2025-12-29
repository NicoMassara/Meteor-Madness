// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BaseMeteor"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MeteorTexture( "MeteorTexture", 2D ) = "white" {}
		_InnerMask( "InnerMask", Range( 0, 1 ) ) = 0
		[HDR] _HoloColor( "HoloColor", Color ) = ( 1, 0, 0, 0 )
		_HoloIntensity( "HoloIntensity", Range( 0, 1 ) ) = 1
		_FlowMap( "FlowMap", 2D ) = "white" {}
		_FlowIntensity( "FlowIntensity", Range( 0, 1 ) ) = 1
		_FlowScale( "FlowScale", Range( 0, 1 ) ) = 1
		_FlowSpeed( "FlowSpeed", Range( 0.1, 1 ) ) = 0.1
		_FlowDirection( "FlowDirection", Vector ) = ( 0.5, 0.35, 0, 0 )
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19905
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MeteorTexture;
		uniform float4 _MeteorTexture_ST;
		uniform float4 _HoloColor;
		uniform float _HoloIntensity;
		uniform float _FlowSpeed;
		uniform float2 _FlowDirection;
		uniform sampler2D _FlowMap;
		uniform float _FlowIntensity;
		uniform float _FlowScale;
		uniform float _InnerMask;
		uniform float _Cutoff = 0.5;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MeteorTexture = i.uv_texcoord * _MeteorTexture_ST.xy + _MeteorTexture_ST.zw;
			float temp_output_3_0 = distance( ( i.uv_texcoord - float2( 0.5,0.5 ) ) , float2( 0,0 ) );
			float HoloMask23 = saturate( -(  (0.0 + ( ( 1.0 - 0.3012815 ) - 0.0 ) * ( 0.5 - 0.0 ) / ( 1.0 - 0.0 ) ) - temp_output_3_0 ) );
			float mulTime49 = _Time.y * _FlowSpeed;
			float4 lerpResult45 = lerp( float4( i.uv_texcoord, 0.0 , 0.0 ) , tex2D( _FlowMap, i.uv_texcoord ) , _FlowIntensity);
			float2 panner47 = ( mulTime49 * _FlowDirection + lerpResult45.rg);
			float simplePerlin2D50 = snoise( panner47* (0.0 + ( _FlowScale - 0.0 ) * ( 5.0 - 0.0 ) / ( 1.0 - 0.0 ) ) );
			simplePerlin2D50 = simplePerlin2D50*0.5 + 0.5;
			float Flow54 = simplePerlin2D50;
			float4 ColoredHolo30 = saturate( ( ( _HoloColor * HoloMask23 * _HoloIntensity ) * Flow54 ) );
			float4 Emission40 = saturate( ( tex2D( _MeteorTexture, uv_MeteorTexture ) + ColoredHolo30 ) );
			o.Emission = Emission40.rgb;
			o.Alpha = 1;
			float OpacityMask10 = ( step( temp_output_3_0 , 0.5 ) * ( 1.0 - step( temp_output_3_0 ,  (0.0 + ( _InnerMask - 0.0 ) * ( 0.45 - 0.0 ) / ( 1.0 - 0.0 ) ) ) ) );
			clip( OpacityMask10 - _Cutoff );
		}

		ENDCG
	}
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;24;-2247.371,736.5889;Inherit;False;1511.538;321.8868;Comment;7;23;37;21;17;22;16;15;Holo Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;18;-2248.04,1143.146;Inherit;False;1583.143;398.9619;Comment;10;8;9;5;4;10;7;6;3;2;1;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-2197.371,787.4442;Inherit;False;Constant;_HoloSize;HoloSize;1;0;Create;True;0;0;0;False;0;False;0.3012815;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2207.82,1205.67;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;53;-2248.782,112.9606;Inherit;False;1895.368;534.9857;Comment;12;54;46;51;52;47;48;50;49;45;44;43;60;Flow;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;22;-1907.632,793.5812;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-1943.818,1204.67;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-1728.531,792.9861;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;43;-2198.783,171.8949;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-1693.623,1205.157;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-1499.373,791.4442;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;44;-1910.327,269.4563;Inherit;True;Property;_FlowMap;FlowMap;6;0;Create;True;0;0;0;False;0;False;-1;a2567c79fcafff541bd097660979fea8;a2567c79fcafff541bd097660979fea8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;46;-1888.327,474.4558;Inherit;False;Property;_FlowIntensity;FlowIntensity;7;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;60;-1862.263,556.128;Inherit;False;Property;_FlowSpeed;FlowSpeed;9;0;Create;True;0;0;0;False;0;False;0.1;0.1;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;45;-1518.328,170.4558;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NegateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;21;-1322.774,786.0507;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;49;-1513.09,534.9279;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;51;-1254.739,414.961;Inherit;False;Property;_FlowScale;FlowScale;8;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;48;-1516.09,332.9286;Inherit;False;Property;_FlowDirection;FlowDirection;10;0;Create;True;0;0;0;False;0;False;0.5,0.35;0.5,0.35;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;47;-1239.328,169.4558;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;52;-878.739,402.961;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;37;-1168.333,786.6445;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;42;-2239.537,-459.821;Inherit;False;1242.994;494.18;Comment;8;30;35;27;56;55;29;26;28;Colored Holo;1,1,1,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;50;-865.739,162.9606;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;23;-998.0515,786.0247;Inherit;True;HoloMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;26;-2179.871,-208.5997;Inherit;False;23;HoloMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;29;-2183.537,-114.8015;Inherit;False;Property;_HoloIntensity;HoloIntensity;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-597.6937,162.4274;Inherit;True;Flow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;28;-2189.537,-407.8014;Inherit;False;Property;_HoloColor;HoloColor;4;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;55;-1833.219,-143.0054;Inherit;False;54;Flow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;27;-1822.971,-404.9668;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;56;-1567.354,-406.2222;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;35;-1398.454,-407.2926;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-2013.515,1348.469;Inherit;False;Property;_InnerMask;InnerMask;3;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;59;-2245.323,-962.7133;Inherit;False;1029.464;428.0911;Comment;5;32;34;38;39;40;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;30;-1213.939,-410.8214;Inherit;True;ColoredHolo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-1708.913,1349.088;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;32;-2175.844,-649.7822;Inherit;False;30;ColoredHolo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;5;-1488.499,1333.361;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;34;-2208,-912;Inherit;True;Property;_MeteorTexture;MeteorTexture;2;0;Create;True;0;0;0;False;0;False;-1;54c9e9baffb09864a83d438f96841dfd;54c9e9baffb09864a83d438f96841dfd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;38;-1795.258,-749.4762;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-1488.499,1208.361;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;9;-1358.499,1330.361;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-1642.258,-749.4762;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-1129.499,1211.361;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;-883.3433,1201.528;Inherit;False;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-1456,-768;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;193.3105,151.8209;Inherit;False;10;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;41;181.3348,-58.90967;Inherit;False;40;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;33;378.85,-342.4353;Inherit;True;Property;_MainTex;_MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;419.6417,-75.89265;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;BaseMeteor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;True;1;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;15;0
WireConnection;2;0;1;0
WireConnection;17;0;22;0
WireConnection;3;0;2;0
WireConnection;16;0;17;0
WireConnection;16;1;3;0
WireConnection;44;1;43;0
WireConnection;45;0;43;0
WireConnection;45;1;44;0
WireConnection;45;2;46;0
WireConnection;21;0;16;0
WireConnection;49;0;60;0
WireConnection;47;0;45;0
WireConnection;47;2;48;0
WireConnection;47;1;49;0
WireConnection;52;0;51;0
WireConnection;37;0;21;0
WireConnection;50;0;47;0
WireConnection;50;1;52;0
WireConnection;23;0;37;0
WireConnection;54;0;50;0
WireConnection;27;0;28;0
WireConnection;27;1;26;0
WireConnection;27;2;29;0
WireConnection;56;0;27;0
WireConnection;56;1;55;0
WireConnection;35;0;56;0
WireConnection;30;0;35;0
WireConnection;7;0;6;0
WireConnection;5;0;3;0
WireConnection;5;1;7;0
WireConnection;38;0;34;0
WireConnection;38;1;32;0
WireConnection;4;0;3;0
WireConnection;9;0;5;0
WireConnection;39;0;38;0
WireConnection;8;0;4;0
WireConnection;8;1;9;0
WireConnection;10;0;8;0
WireConnection;40;0;39;0
WireConnection;0;2;41;0
WireConnection;0;10;11;0
ASEEND*/
//CHKSM=1E2B0BD4ACD8BADF698A3C4B64F40B1AEF8CE6E1