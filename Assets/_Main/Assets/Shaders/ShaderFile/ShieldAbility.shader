// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mat_Shield_AbilityEffect"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EffectFrequency("EffectFrequency", Range( 0 , 1)) = 0
		_AbilityColor("AbilityColor", Color) = (0.0009765625,0,0,0)
		_EffectSpeed("EffectSpeed", Range( 0 , 1)) = 6
		_EffectWidth("EffectWidth", Range( 0 , 1)) = 0.4235294
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_MainTex("_MainTex", 2D) = "white" {}
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
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _AbilityColor;
		uniform float _EffectFrequency;
		uniform float _EffectSpeed;
		uniform float _EffectWidth;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 AbilityColor125 = _AbilityColor;
			float mulTime60 = _Time.y * (0.0 + (_EffectSpeed - 0.0) * (30.0 - 0.0) / (1.0 - 0.0));
			float Effect36 = step( sin( ( ( i.uv_texcoord.x * (10.0 + (_EffectFrequency - 0.0) * (100.0 - 10.0) / (1.0 - 0.0)) ) + mulTime60 ) ) , (-0.75 + (_EffectWidth - 0.0) * (0.5 - -0.75) / (1.0 - 0.0)) );
			float4 Emission51 = saturate( ( ( AbilityColor125 + float4( 1,1,1,1 ) ) + ( AbilityColor125 * Effect36 ) ) );
			o.Emission = Emission51.rgb;
			o.Alpha = 1;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float grayscale230 = Luminance(tex2D( _MainTex, uv_MainTex ).rgb);
			float HideMask176 = ( i.uv_texcoord.x + (-1.0 + (_Opacity - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) );
			float OpacityMask153 = ( ( ( Effect36 * step( grayscale230 , 0.99 ) ) + (0) ) * HideMask176 );
			clip( OpacityMask153 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;174;-3125.997,1287.537;Inherit;False;1082.843;381.4408;Comment;5;181;176;172;158;167;Hide Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;134;-3155.266,-900.2668;Inherit;False;1178.062;733.8947;Comment;8;51;131;129;124;37;126;128;264;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;127;-411.686,-394.1908;Inherit;False;525.988;257.8;Comment;2;10;125;AbilityColor;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;65;-3165.218,-76.58676;Inherit;False;1441.727;646.1461;Comment;13;60;67;61;62;36;41;59;39;66;40;76;77;78;Effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;16;-3455.287,664.8702;Inherit;False;1950.017;549.2777;Comment;10;153;178;179;151;144;148;229;230;231;247;Opacity Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-109,-53;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Mat_Shield_AbilityEffect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-444.7552,-57.65074;Inherit;False;51;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;158;-2973.226,1337.537;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;151;-2222.572,764.239;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-1969.462,758.4595;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;-1783.29,754.2678;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;172;-2689.813,1468.14;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;181;-2500.361,1371.741;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-3043.997,1502.427;Inherit;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2617.607,25.26563;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-2400.138,22.68382;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-3111.682,336.6661;Inherit;False;Property;_EffectSpeed;EffectSpeed;3;0;Create;True;0;0;0;False;0;False;6;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;62;-2800.887,154.3375;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;10;False;4;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1956.621,15.35478;Inherit;True;Effect;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;76;-2114.992,22.09833;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-3115.218,160.2949;Inherit;False;Property;_EffectFrequency;EffectFrequency;1;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;60;-2559.397,288.3695;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-2357.992,379.0983;Inherit;False;Property;_EffectWidth;EffectWidth;4;0;Create;True;0;0;0;False;0;False;0.4235294;0.3516257;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;78;-2185.992,151.0983;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.75;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;41;-2269.608,26.26563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;67;-2776.94,349.6983;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;30;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;66;-3131.365,-2.878263;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-125.098,-336.2848;Inherit;False;AbilityColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;10;-361.6859,-344.1907;Inherit;False;Property;_AbilityColor;AbilityColor;2;0;Create;True;0;0;0;False;0;False;0.0009765625,0,0,0;1,0.9215686,0.01568628,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;230;-3311.643,969.8668;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;231;-3004.531,967.7786;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;229;-3402.215,734.4094;Inherit;True;Property;_MainTex;_MainTex;6;0;Create;True;0;0;0;False;0;False;-1;None;869c9fe9f51fc9c41b7f19176e72dfcc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;176;-2287.425,1352.772;Inherit;True;HideMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;247;-2551.307,980.7943;Inherit;True;-1;;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-2190.087,978.5726;Inherit;True;176;HideMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-433.8868,164.163;Inherit;False;153;OpacityMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;129;-2589.068,-659.066;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;131;-2446.166,-661.4095;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-2223.202,-678.8351;Inherit;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-2945.646,721.2389;Inherit;True;36;Effect;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-2424.467,732.7969;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;-3083.163,-493.6938;Inherit;False;125;AbilityColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-3061.761,-361.1143;Inherit;False;36;Effect;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-2829.084,-441.7044;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-3115.367,-831.3663;Inherit;False;125;AbilityColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;264;-2851.651,-808.1469;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,1;False;1;COLOR;0
WireConnection;0;2;52;0
WireConnection;0;10;155;0
WireConnection;151;0;148;0
WireConnection;151;1;247;0
WireConnection;178;0;151;0
WireConnection;178;1;179;0
WireConnection;153;0;178;0
WireConnection;172;0;167;0
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
WireConnection;125;0;10;0
WireConnection;230;0;229;0
WireConnection;231;0;230;0
WireConnection;176;0;181;0
WireConnection;129;0;264;0
WireConnection;129;1;124;0
WireConnection;131;0;129;0
WireConnection;51;0;131;0
WireConnection;148;0;144;0
WireConnection;148;1;231;0
WireConnection;124;0;126;0
WireConnection;124;1;37;0
WireConnection;264;0;128;0
ASEEND*/
//CHKSM=E68E3397BBA97360CE701CAE8EF51525DC1FD135