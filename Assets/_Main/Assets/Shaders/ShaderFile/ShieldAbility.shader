// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Shield_AbilityEffect"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_ShieldSilouette("ShieldSilouette", 2D) = "white" {}
		_ShieldSilouette1("ShieldSilouette", 2D) = "white" {}
		_EffectFrequency("EffectFrequency", Range( 0 , 1)) = 0
		_AbilityColor("AbilityColor", Color) = (0.0009765625,0,0,0)
		_EffectSpeed("EffectSpeed", Range( 0 , 1)) = 6
		_EffectWidth("EffectWidth", Range( 0 , 1)) = 0.4235294
		_Opacity("Opacity", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _AbilityColor;
		uniform sampler2D _ShieldSilouette;
		uniform float4 _ShieldSilouette_ST;
		uniform sampler2D _ShieldSilouette1;
		uniform float _EffectFrequency;
		uniform float _EffectSpeed;
		uniform float _EffectWidth;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 AbilityColor125 = _AbilityColor;
			float2 uv_ShieldSilouette = i.uv_texcoord * _ShieldSilouette_ST.xy + _ShieldSilouette_ST.zw;
			float grayscale5 = Luminance(tex2D( _ShieldSilouette, uv_ShieldSilouette ).rgb);
			float ShapeMask156 = step( grayscale5 , 0.99 );
			float2 temp_cast_1 = (-0.16).xx;
			float2 uv_TexCoord70 = i.uv_texcoord * ( 1.27 + float2( 0,0.02 ) ) + temp_cast_1;
			float grayscale71 = Luminance(tex2D( _ShieldSilouette1, uv_TexCoord70 ).rgb);
			float OutlineMask86 = ( ShapeMask156 * ( 1.0 - step( grayscale71 , 0.99 ) ) );
			float mulTime60 = _Time.y * (0.0 + (_EffectSpeed - 0.0) * (30.0 - 0.0) / (1.0 - 0.0));
			float Effect36 = step( sin( ( ( i.uv_texcoord.x * (10.0 + (_EffectFrequency - 0.0) * (100.0 - 10.0) / (1.0 - 0.0)) ) + mulTime60 ) ) , (-0.75 + (_EffectWidth - 0.0) * (0.5 - -0.75) / (1.0 - 0.0)) );
			float4 Emission51 = saturate( ( ( AbilityColor125 * OutlineMask86 ) + ( AbilityColor125 * ( ( 1.0 - OutlineMask86 ) * Effect36 ) ) ) );
			o.Albedo = Emission51.rgb;
			o.Alpha = 1;
			float HideMask176 = ( i.uv_texcoord.x + (-1.0 + (_Opacity - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) );
			float OpacityMask153 = ( ( ( Effect36 * ShapeMask156 ) + OutlineMask86 ) * HideMask176 );
			clip( OpacityMask153 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;174;-3209.989,1776.951;Inherit;False;1082.843;381.4408;Comment;5;181;176;172;158;167;Hide Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;134;-3155.266,-900.2668;Inherit;False;1447.162;750.7947;Comment;11;128;121;51;131;129;124;126;64;123;88;37;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;127;-411.686,-394.1908;Inherit;False;525.988;257.8;Comment;2;10;125;AbilityColor;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;119;-3151.269,1267.749;Inherit;False;2006.464;428.3583;Comment;12;73;71;70;72;82;83;86;117;115;116;118;157;OutlineMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;65;-3165.218,-76.58676;Inherit;False;1441.727;646.1461;Comment;13;60;67;61;62;36;41;59;39;66;40;76;77;78;Effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;16;-3148.287,664.8702;Inherit;False;1643.017;537.2777;Comment;11;153;178;179;151;150;156;144;13;5;3;148;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-1551.71,1389.603;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;-1384.203,1385.676;Inherit;True;OutlineMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-2880.869,1377.048;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-3101.269,1317.749;Inherit;False;Constant;_OutlineTiling;OutlineTiling;7;0;Create;True;0;0;0;False;0;False;1.27;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-3058.569,1580.948;Inherit;False;Constant;_OutlineOffset;OutlineOffset;7;0;Create;True;0;0;0;False;0;False;-0.16;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;118;-3078.869,1410.048;Inherit;False;Constant;_TilingOffset;TilingOffset;7;0;Create;True;0;0;0;False;0;False;0,0.02;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-109,-53;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Mat_Shield_AbilityEffect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-444.7552,-57.65074;Inherit;False;51;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;82;-1729.791,1474.875;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;72;-1882.976,1463.031;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;-1788.73,1351.036;Inherit;False;156;ShapeMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;70;-2703.203,1405.651;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;-2388.871,1412.315;Inherit;True;Property;_ShieldSilouette1;ShieldSilouette;2;0;Create;True;0;0;0;False;0;False;-1;None;869c9fe9f51fc9c41b7f19176e72dfcc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;71;-2086.397,1425.716;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-2426.423,732.7969;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-3108.287,722.9506;Inherit;True;Property;_ShieldSilouette;ShieldSilouette;1;0;Create;True;0;0;0;False;0;False;-1;None;869c9fe9f51fc9c41b7f19176e72dfcc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;5;-3114.614,947.8051;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;13;-2851.935,946.4686;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-914.8868,170.163;Inherit;False;153;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;158;-3057.218,1826.951;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;144;-2779.646,731.2389;Inherit;True;36;Effect;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-2698.824,945.0558;Inherit;True;ShapeMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-2460.671,939.5618;Inherit;True;86;OutlineMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;151;-2222.572,764.239;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-2203.087,946.5726;Inherit;True;176;HideMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-1969.462,758.4595;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;-1783.29,754.2678;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;172;-2773.805,1957.554;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;176;-2375.753,1838.934;Inherit;True;HideMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;181;-2584.353,1861.155;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-3127.989,1991.841;Inherit;False;Property;_Opacity;Opacity;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2617.607,25.26563;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-2400.138,22.68382;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-3111.682,336.6661;Inherit;False;Property;_EffectSpeed;EffectSpeed;5;0;Create;True;0;0;0;False;0;False;6;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;62;-2800.887,154.3375;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;10;False;4;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1956.621,15.35478;Inherit;True;Effect;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;76;-2114.992,22.09833;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-3115.218,160.2949;Inherit;False;Property;_EffectFrequency;EffectFrequency;3;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;60;-2559.397,288.3695;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-2357.992,379.0983;Inherit;False;Property;_EffectWidth;EffectWidth;6;0;Create;True;0;0;0;False;0;False;0.4235294;0.3516257;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;78;-2185.992,151.0983;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.75;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;41;-2269.608,26.26563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;67;-2776.94,349.6983;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;30;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;66;-3131.365,-2.878263;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;123;-2883.2,-512.7607;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2688.779,-463.3029;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;-2687.163,-562.6938;Inherit;False;125;AbilityColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-2428.084,-521.7044;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;129;-2248.068,-680.066;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;131;-2105.166,-682.4095;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-1918.202,-678.8351;Inherit;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-2692.844,-797.8807;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-3112.367,-804.3663;Inherit;False;125;AbilityColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-3092.761,-374.1143;Inherit;False;36;Effect;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-3101.781,-563.4193;Inherit;False;86;OutlineMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-125.098,-336.2848;Inherit;False;AbilityColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;10;-361.6859,-344.1907;Inherit;False;Property;_AbilityColor;AbilityColor;4;0;Create;True;0;0;0;False;0;False;0.0009765625,0,0,0;1,0.9215686,0.01568628,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;83;0;157;0
WireConnection;83;1;82;0
WireConnection;86;0;83;0
WireConnection;117;0;115;0
WireConnection;117;1;118;0
WireConnection;0;0;52;0
WireConnection;0;10;155;0
WireConnection;82;0;72;0
WireConnection;72;0;71;0
WireConnection;70;0;117;0
WireConnection;70;1;116;0
WireConnection;73;1;70;0
WireConnection;71;0;73;0
WireConnection;148;0;144;0
WireConnection;148;1;156;0
WireConnection;5;0;3;0
WireConnection;13;0;5;0
WireConnection;156;0;13;0
WireConnection;151;0;148;0
WireConnection;151;1;150;0
WireConnection;178;0;151;0
WireConnection;178;1;179;0
WireConnection;153;0;178;0
WireConnection;172;0;167;0
WireConnection;176;0;181;0
WireConnection;181;0;158;1
WireConnection;181;1;172;0
WireConnection;39;0;66;1
WireConnection;39;1;62;0
WireConnection;59;0;39;0
WireConnection;59;1;60;0
WireConnection;62;0;40;0
WireConnection;36;0;76;0
WireConnection;76;0;41;0
WireConnection;76;1;78;0
WireConnection;60;0;67;0
WireConnection;78;0;77;0
WireConnection;41;0;59;0
WireConnection;67;0;61;0
WireConnection;123;0;88;0
WireConnection;64;0;123;0
WireConnection;64;1;37;0
WireConnection;124;0;126;0
WireConnection;124;1;64;0
WireConnection;129;0;121;0
WireConnection;129;1;124;0
WireConnection;131;0;129;0
WireConnection;51;0;131;0
WireConnection;121;0;128;0
WireConnection;121;1;88;0
WireConnection;125;0;10;0
ASEEND*/
//CHKSM=EFFBBE93702EABD93514363684606FCD2FC1B62E