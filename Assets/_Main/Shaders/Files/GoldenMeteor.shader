// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GoldenMeteor"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MeteorTexture( "MeteorTexture", 2D ) = "white" {}
		_GoldTexture( "_GoldTexture", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.5
		#define ASE_VERSION 19905
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MeteorTexture;
		uniform float4 _MeteorTexture_ST;
		uniform sampler2D _GoldTexture;
		uniform float4 _GoldTexture_ST;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MeteorTexture = i.uv_texcoord * _MeteorTexture_ST.xy + _MeteorTexture_ST.zw;
			float4 break2_g70 = tex2D( _MeteorTexture, uv_MeteorTexture );
			float2 uv_GoldTexture = i.uv_texcoord * _GoldTexture_ST.xy + _GoldTexture_ST.zw;
			float4 Surface19 = saturate( ( ( ( break2_g70.x + break2_g70.y + break2_g70.z ) * break2_g70.w ) * tex2D( _GoldTexture, uv_GoldTexture ) ) );
			float2 uv_TexCoord2_g68 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 break2_g67 =  (float2( -1,-1 ) + ( uv_TexCoord2_g68 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) );
			float temp_output_5_0_g67 = ( ( break2_g67.x * break2_g67.x ) + ( break2_g67.y * break2_g67.y ) );
			float Depth14_g67 = saturate( ( sqrt( saturate( ( 1.0 - temp_output_5_0_g67 ) ) ) * 2.0 ) );
			float4 Emission29 = saturate( ( Surface19 * Depth14_g67 ) );
			o.Emission = Emission29.xyz;
			o.Alpha = 1;
			float2 uv_TexCoord2_g2 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float smoothstepResult3_g1 = smoothstep( 1.0 , 0.98 , length(  (float2( -1,-1 ) + ( uv_TexCoord2_g2 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) ) ));
			float Circular_Mask7_g1 = sqrt( ( 1.0 - pow( ( 1.0 - smoothstepResult3_g1 ) , 2.0 ) ) );
			float OpacityMask11 = Circular_Mask7_g1;
			clip( OpacityMask11 - _Cutoff );
		}

		ENDCG
	}
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;23;-2464,-400;Inherit;False;1524;586.7;Comment;6;14;18;19;17;39;40;Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;14;-2384,-256;Inherit;True;Property;_MeteorTexture;MeteorTexture;1;0;Create;True;0;0;0;False;0;False;-1;54c9e9baffb09864a83d438f96841dfd;54c9e9baffb09864a83d438f96841dfd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;39;-2080,-256;Inherit;False;2DTextureGrayscaleMask;-1;;70;a6ae6affcb0ad5a4fb811e84c869b980;0;1;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-2384,-48;Inherit;True;Property;_GoldTexture;_GoldTexture;2;0;Create;True;0;0;0;False;0;False;-1;None;f1fc18bb7335f2b4d9504c8ab0d6de26;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;18;-1744,-256;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-1472,-256;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;30;-1994.729,254;Inherit;False;748.7286;287.598;Comment;3;27;26;29;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;19;-1184,-256;Inherit;True;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2128,720;Inherit;False;596.0331;333.7123;Comment;2;11;25;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;27;-1952,304;Inherit;True;19;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;25;-2080,784;Inherit;False;UV_CircularMask;-1;;1;28af572f72fc00741ad2033d2531b438;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;26;-1728,304;Inherit;False;DepthSphere;-1;;3;3f6e013e4ff1a7d4ca2921bb52396135;0;1;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-1824,784;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;29;-1488,304;Inherit;True;Emission;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;13;-336,528;Inherit;True;11;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;31;-231.6594,81.71112;Inherit;False;29;Emission;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;0,0;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;GoldenMeteor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;39;1;14;0
WireConnection;18;0;39;0
WireConnection;18;1;17;0
WireConnection;40;0;18;0
WireConnection;19;0;40;0
WireConnection;26;2;27;0
WireConnection;11;0;25;0
WireConnection;29;0;26;0
WireConnection;0;2;31;0
WireConnection;0;10;13;0
ASEEND*/
//CHKSM=94ECDC2F3BAD0DFA4C5193BC4BF4172D26EB674E