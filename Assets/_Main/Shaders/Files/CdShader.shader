// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CdShader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Texture( "_Texture", 2D ) = "white" {}
		_InnerMaskRadius( "InnerMaskRadius", Range( 0, 1 ) ) = 0.7
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

		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float _InnerMaskRadius;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float2 uv_TexCoord2_g68 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 break2_g67 =  (float2( -1,-1 ) + ( uv_TexCoord2_g68 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) );
			float temp_output_5_0_g67 = ( ( break2_g67.x * break2_g67.x ) + ( break2_g67.y * break2_g67.y ) );
			float Depth14_g67 = saturate( ( sqrt( saturate( ( 1.0 - temp_output_5_0_g67 ) ) ) * 2.0 ) );
			o.Emission = saturate( ( tex2D( _Texture, uv_Texture ) * Depth14_g67 ) ).xyz;
			o.Alpha = 1;
			float2 uv_TexCoord2_g12 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float temp_output_9_0_g11 = length(  (float2( -1,-1 ) + ( uv_TexCoord2_g12 - float2( 0,0 ) ) * ( float2( 1,1 ) - float2( -1,-1 ) ) / ( float2( 1,1 ) - float2( 0,0 ) ) ) );
			float smoothstepResult7_g11 = smoothstep( 1.0 , 0.98 , temp_output_9_0_g11);
			float smoothstepResult16_g11 = smoothstep(  (-1.0 + ( _InnerMaskRadius - 0.0 ) * ( 0.95 - -1.0 ) / ( 1.0 - 0.0 ) ) , 0.98 , temp_output_9_0_g11);
			float OpacityMask33 = saturate( ( sqrt( ( 1.0 - pow( ( 1.0 - smoothstepResult7_g11 ) , 2.0 ) ) ) * smoothstepResult16_g11 ) );
			clip( OpacityMask33 - _Cutoff );
		}

		ENDCG
	}
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;34;-1200,304;Inherit;False;548;303.95;Comment;3;33;36;40;Opacity MaskR;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;36;-1168,464;Inherit;False;Property;_InnerMaskRadius;InnerMaskRadius;2;0;Create;True;0;0;0;False;0;False;0.7;0.1583681;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-1152,368;Inherit;False;UV_DonutMask;-1;;11;4ee4458d51442914e84cb4f094dbef67;0;1;19;FLOAT;1;False;1;FLOAT;10
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;33;-896,352;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-1040,48;Inherit;True;Property;_Texture;_Texture;1;0;Create;True;0;0;0;False;0;False;-1;None;b63cb86ade31367439ed377d92f214c3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;35;-345.4111,411.1408;Inherit;True;33;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;41;-464,48;Inherit;False;DepthSphere;-1;;13;3f6e013e4ff1a7d4ca2921bb52396135;0;1;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;0,0;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;CdShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;19;36;0
WireConnection;33;0;40;10
WireConnection;41;2;8;0
WireConnection;0;2;41;0
WireConnection;0;10;35;0
ASEEND*/
//CHKSM=D92C9B074DDA6B5EE27A0A11C57A90FD1FB949E5