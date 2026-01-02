// Made with Amplify Shader Editor v1.9.9.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GoldenMeteor"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_InnerMask( "InnerMask", Range( 0, 1 ) ) = 0
		_MeteorTexture( "MeteorTexture", 2D ) = "white" {}
		_TextureSample0( "Texture Sample 0", 2D ) = "white" {}
		_Float0( "Float 0", Float ) = 0
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MeteorTexture;
		uniform float4 _MeteorTexture_ST;
		uniform float _Float0;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _InnerMask;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MeteorTexture = i.uv_texcoord * _MeteorTexture_ST.xy + _MeteorTexture_ST.zw;
			float4 tex2DNode14 = tex2D( _MeteorTexture, uv_MeteorTexture );
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 Emission19 = ( ( ( tex2DNode14.r + tex2DNode14.g + tex2DNode14.b ) * _Float0 ) * tex2D( _TextureSample0, uv_TextureSample0 ) );
			o.Emission = Emission19.rgb;
			o.Alpha = 1;
			float temp_output_4_0 = distance( ( i.uv_texcoord - float2( 0.5,0.5 ) ) , float2( 0,0 ) );
			float OpacityMask11 = ( step( temp_output_4_0 , 0.5 ) * ( 1.0 - step( temp_output_4_0 ,  (0.0 + ( _InnerMask - 0.0 ) * ( 0.45 - 0.0 ) / ( 1.0 - 0.0 ) ) ) ) );
			clip( OpacityMask11 - _Cutoff );
		}

		ENDCG
	}
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19905
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2128,720;Inherit;False;1583.143;398.9619;Comment;10;11;10;9;8;7;6;5;4;3;2;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;23;-2418,-146;Inherit;False;1524;586.7;Comment;7;14;15;22;21;18;19;17;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-2096,768;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;5;-1904,912;Inherit;False;Property;_InnerMask;InnerMask;1;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-1824,768;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-1600,912;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;14;-2368,-96;Inherit;True;Property;_MeteorTexture;MeteorTexture;2;0;Create;True;0;0;0;False;0;False;-1;54c9e9baffb09864a83d438f96841dfd;54c9e9baffb09864a83d438f96841dfd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.DistanceOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-1584,768;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-1376,896;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-1984,-48;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;22;-1952,208;Inherit;False;Property;_Float0;Float 0;4;0;Create;True;0;0;0;False;0;False;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-1376,784;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;9;-1248,896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;21;-1616,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-1728,208;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;None;f1fc18bb7335f2b4d9504c8ab0d6de26;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;-1008,784;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;18;-1408,0;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;11;-768,768;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;19;-1136,0;Inherit;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;13;-336,528;Inherit;True;11;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;20;-352,48;Inherit;True;19;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;0,0;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Standard;GoldenMeteor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;False;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;2;0
WireConnection;6;0;5;0
WireConnection;4;0;3;0
WireConnection;7;0;4;0
WireConnection;7;1;6;0
WireConnection;15;0;14;1
WireConnection;15;1;14;2
WireConnection;15;2;14;3
WireConnection;8;0;4;0
WireConnection;9;0;7;0
WireConnection;21;0;15;0
WireConnection;21;1;22;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;18;0;21;0
WireConnection;18;1;17;0
WireConnection;11;0;10;0
WireConnection;19;0;18;0
WireConnection;0;2;20;0
WireConnection;0;10;13;0
ASEEND*/
//CHKSM=14B0D7F1517F4F39F220CA17023CA3A36FFDE3C8