// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BaseShield"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_ShieldTexture("ShieldTexture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Stencil
		{
			Ref 0
		}
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _ShieldTexture;
		uniform float4 _ShieldTexture_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ShieldTexture = i.uv_texcoord * _ShieldTexture_ST.xy + _ShieldTexture_ST.zw;
			float4 ShieldTexture13 = tex2D( _ShieldTexture, uv_ShieldTexture );
			o.Emission = ShieldTexture13.rgb;
			o.Alpha = 1;
			float4 break20 = ShieldTexture13;
			float4 appendResult24 = (float4(break20.r , break20.g , break20.b , break20.a));
			float grayscale8 = Luminance(appendResult24.xyz);
			float OpacityMask6 = step( grayscale8 , 0.99 );
			clip( OpacityMask6 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;16;-2227.384,592.9512;Inherit;False;1254.741;423.1599;Comment;6;8;20;14;9;6;24;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;15;-265.3047,-588.9109;Inherit;False;653.451;280;Comment;2;12;13;Shield Texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-1212.043,642.9512;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-555.2844,276.7659;Inherit;False;6;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;8;-1618.833,654.7006;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;20;-1998.039,665.4236;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;14;-2207.384,655.4656;Inherit;True;13;ShieldTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;12;-215.3051,-538.9109;Inherit;True;Property;_ShieldTexture;ShieldTexture;2;0;Create;True;0;0;0;False;0;False;-1;None;869c9fe9f51fc9c41b7f19176e72dfcc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;165.8459,-539.119;Inherit;False;ShieldTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;477.9761,-538.1936;Inherit;True;Property;_MainTex;_MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;BaseShield;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;True;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.StepOpNode;9;-1378.154,677.3641;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-1849.744,652.7711;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;-403.1727,5.190729;Inherit;False;13;ShieldTexture;1;0;OBJECT;;False;1;COLOR;0
WireConnection;6;0;9;0
WireConnection;8;0;24;0
WireConnection;20;0;14;0
WireConnection;13;0;12;0
WireConnection;0;2;17;0
WireConnection;0;10;11;0
WireConnection;9;0;8;0
WireConnection;24;0;20;0
WireConnection;24;1;20;1
WireConnection;24;2;20;2
WireConnection;24;3;20;3
ASEEND*/
//CHKSM=076BA5E70B68FE057823D11B7FBE67A51C6210FD