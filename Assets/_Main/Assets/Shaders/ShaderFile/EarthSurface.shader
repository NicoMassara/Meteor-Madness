// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EarthSurface"
{
	Properties
	{
		_EarthMap("EarthMap", 2D) = "white" {}
		_SurfaceDamage("SurfaceDamage", 2D) = "white" {}
		_WaterDamage("WaterDamage", 2D) = "white" {}
		_FlowMap("FlowMap", 2D) = "white" {}
		_WaterDefault("WaterDefault", 2D) = "white" {}
		_GassTexture("GassTexture", 2D) = "white" {}
		_HealthAmount("HealthAmount", Range( 0 , 1)) = -0.26
		_StepValue("StepValue", Range( 0.03 , 0.4)) = 0.03752942
		_FlowIntensity("Flow Intensity", Range( 0 , 1)) = 0
		_WaterTiling("WaterTiling", Range( 0.1 , 10)) = 5.06
		_GrassTextureTiling("GrassTextureTiling", Float) = -0.14
		_MinToIncreasePannerSpeed("MinToIncreasePannerSpeed", Range( 0 , 1)) = 0.35
		_PannerSpeedMultiplier("PannerSpeedMultiplier", Range( 0 , 15)) = 2
		_TimeScale("TimeScale", Float) = 1
		_SurfaceIceIntensity("Surface Ice Intensity", Range( 0 , 1)) = 0.9
		_WaterIceIntensity("Water Ice Intensity", Range( 0 , 1)) = 0.6
		_SolidTop("_SolidTop", Range( 0 , 1)) = 0.1373401
		_TopFadeLenght("_TopFadeLenght", Range( 0 , 1)) = 0.3475586
		_BottomFadeLenght("_BottomFadeLenght", Range( 0 , 1)) = 0
		_SolidBottom("_SolidBottom", Range( 0 , 1)) = 0.3649352
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _WaterDamage;
			uniform float _TimeScale;
			uniform float _HealthAmount;
			uniform float _MinToIncreasePannerSpeed;
			uniform float _PannerSpeedMultiplier;
			uniform float _WaterTiling;
			uniform sampler2D _FlowMap;
			uniform float _FlowIntensity;
			uniform sampler2D _EarthMap;
			uniform float4 _EarthMap_ST;
			uniform float _StepValue;
			uniform sampler2D _WaterDefault;
			uniform float _SolidTop;
			uniform float _TopFadeLenght;
			uniform float _SolidBottom;
			uniform float _BottomFadeLenght;
			uniform float _WaterIceIntensity;
			uniform sampler2D _SurfaceDamage;
			uniform float4 _SurfaceDamage_ST;
			uniform sampler2D _GassTexture;
			uniform float _GrassTextureTiling;
			uniform float _SurfaceIceIntensity;
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
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float mulTime306 = _Time.y * _TimeScale;
				float HealthAmount61 = _HealthAmount;
				float2 _PannerSpeed = float2(0.05,0.075);
				float2 temp_output_303_0 = ( _PannerSpeed * _PannerSpeedMultiplier );
				float2 ifLocalVar300 = 0;
				if( HealthAmount61 <= _MinToIncreasePannerSpeed )
				ifLocalVar300 = temp_output_303_0;
				else
				ifLocalVar300 = _PannerSpeed;
				float2 temp_cast_0 = (_WaterTiling).xx;
				float2 texCoord117 = i.ase_texcoord1.xy * temp_cast_0 + float2( 0,0 );
				float2 temp_cast_2 = (_WaterTiling).xx;
				float2 texCoord127 = i.ase_texcoord1.xy * temp_cast_2 + float2( 0,0 );
				float4 lerpResult123 = lerp( float4( texCoord117, 0.0 , 0.0 ) , tex2D( _FlowMap, texCoord127 ) , _FlowIntensity);
				float2 panner119 = ( mulTime306 * ifLocalVar300 + lerpResult123.rg);
				float2 Panner138 = panner119;
				float4 color146 = IsGammaSpace() ? float4(1,0.4725828,0,1) : float4(1,0.189536,0,1);
				float2 uv_EarthMap = i.ase_texcoord1.xy * _EarthMap_ST.xy + _EarthMap_ST.zw;
				float temp_output_24_0 = step( tex2D( _EarthMap, uv_EarthMap ).b , _StepValue );
				float WaterMask44 = ( 1.0 - temp_output_24_0 );
				float4 LavaWater150 = ( tex2D( _WaterDamage, Panner138 ) * color146 * WaterMask44 );
				float4 color151 = IsGammaSpace() ? float4(0,0.4434402,1,1) : float4(0,0.1653733,1,1);
				float temp_output_2_0_g14 = ( 1.0 - (0.0 + (_SolidTop - 0.0) * (0.5 - 0.0) / (1.0 - 0.0)) );
				float2 texCoord775 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float smoothstepResult5_g14 = smoothstep( temp_output_2_0_g14 , ( temp_output_2_0_g14 + (0.0 + (_TopFadeLenght - 0.0) * (0.25 - 0.0) / (1.0 - 0.0)) ) , texCoord775.y);
				float temp_output_2_0_g13 = ( 1.0 - (0.0 + (_SolidBottom - 0.0) * (0.5 - 0.0) / (1.0 - 0.0)) );
				float smoothstepResult5_g13 = smoothstep( temp_output_2_0_g13 , ( temp_output_2_0_g13 + (0.0 + (_BottomFadeLenght - 0.0) * (0.25 - 0.0) / (1.0 - 0.0)) ) , ( 1.0 - texCoord775.y ));
				float IceMask706 = ( saturate( smoothstepResult5_g14 ) + saturate( smoothstepResult5_g13 ) );
				float4 Water172 = saturate( ( ( tex2D( _WaterDefault, Panner138 ) * color151 * WaterMask44 ) + saturate( ( WaterMask44 * IceMask706 * _WaterIceIntensity ) ) ) );
				float4 lerpResult59 = lerp( LavaWater150 , Water172 , HealthAmount61);
				float2 uv_SurfaceDamage = i.ase_texcoord1.xy * _SurfaceDamage_ST.xy + _SurfaceDamage_ST.zw;
				float SurfaceMask46 = temp_output_24_0;
				float4 LavaSurface168 = ( tex2D( _SurfaceDamage, uv_SurfaceDamage ) * SurfaceMask46 );
				float2 temp_cast_4 = (_GrassTextureTiling).xx;
				float2 texCoord180 = i.ase_texcoord1.xy * temp_cast_4 + float2( 0,0 );
				float simplePerlin2D158 = snoise( ( tex2D( _GassTexture, texCoord180 ) * SurfaceMask46 ).rg*11.8 );
				simplePerlin2D158 = simplePerlin2D158*0.5 + 0.5;
				float4 color182 = IsGammaSpace() ? float4(0.04298684,0.3962264,0.04298684,1) : float4(0.003334239,0.130239,0.003334239,1);
				float4 color109 = IsGammaSpace() ? float4(0,0.4745098,0,1) : float4(0,0.1912017,0,1);
				float4 Surface169 = saturate( ( ( step( simplePerlin2D158 , 0.49 ) * color182 ) + ( color109 * SurfaceMask46 ) + saturate( ( SurfaceMask46 * IceMask706 * _SurfaceIceIntensity ) ) ) );
				float4 lerpResult42 = lerp( LavaSurface168 , Surface169 , HealthAmount61);
				float4 FinalSurface402 = saturate( ( lerpResult59 + lerpResult42 ) );
				
				
				finalColor = saturate( FinalSurface402 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;898;-4852.508,-365.4926;Inherit;False;1757.623;792.3076;Comment;22;180;175;185;177;158;160;598;182;179;615;611;616;178;719;169;109;705;572;600;599;29;176;Grass;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;892;-4873.436,494.9089;Inherit;False;1658.505;683.0768;Comment;9;706;867;897;862;864;873;831;824;775;Ice Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;607;-4845.723,-1189.729;Inherit;False;1420.33;772.4872;Comment;12;143;151;45;137;454;604;606;31;573;559;570;172;Water;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;452;-2057.724,-1268.097;Inherit;False;1063.058;548.2256;Comment;3;1052;372;592;Out;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;52;-3021.392,-1297.786;Inherit;False;847.9467;613.9975;Masking;7;4;17;24;46;27;44;77;Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;65;-3009.889,-1505.692;Inherit;False;572.9548;166.825;Health Amount;2;43;61;Health Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;144;-3038.349,234.19;Inherit;False;1628.177;780.281;;16;300;301;126;119;138;123;118;124;117;127;121;302;303;304;306;307;WaterPanner;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;149;-2119.363,1139.103;Inherit;False;1013.76;521.7198;Comment;6;148;146;145;147;150;186;LavaWater;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;167;-3026.179,1124.583;Inherit;False;825.4619;365.9649;Comment;4;168;39;166;100;Lava Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;174;-3000.263,-604.8162;Inherit;False;1331.466;614.4094;Comment;10;350;42;59;173;171;170;152;60;402;593;Final Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;4;-2972.429,-1247.786;Inherit;True;Property;_EarthMap;EarthMap;0;0;Create;True;0;0;0;False;0;False;-1;c036000bb622eae43b6b10cd89a3a25d;c036000bb622eae43b6b10cd89a3a25d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-2959.889,-1454.867;Inherit;False;Property;_HealthAmount;HealthAmount;6;0;Create;True;0;0;0;False;0;False;-0.26;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;17;-2677.366,-1257.913;Inherit;True;True;True;True;True;1;0;FLOAT;-0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-2947.68,-802.9385;Inherit;False;Property;_StepValue;StepValue;7;0;Create;True;0;0;0;False;0;False;0.03752942;0.361;0.03;0.4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-2660.934,-1455.692;Inherit;False;HealthAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;24;-2932.326,-1039.109;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;27;-2613.435,-974.7893;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-2374.446,-981.9187;Inherit;False;WaterMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-2380.065,-1081.857;Inherit;False;SurfaceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;143;-4629.148,-1139.729;Inherit;True;Property;_WaterDefault;WaterDefault;4;0;Create;True;0;0;0;False;0;False;-1;6fe629e0670cf8c4d980fd5b34631dcb;6fe629e0670cf8c4d980fd5b34631dcb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-4263.506,-1135.867;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-3649.391,-1136.681;Inherit;True;Water;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-4798.723,-1114.284;Inherit;False;138;Panner;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;151;-4552.67,-952.9355;Inherit;False;Constant;_WaterColor;WaterColor;9;0;Create;True;0;0;0;False;0;False;0,0.4434402,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;454;-4513,-676.1886;Inherit;False;706;IceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-4513.388,-766.8246;Inherit;False;44;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;559;-4259.115,-756.7301;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;570;-4118.614,-757.0922;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;573;-4612.367,-591.6381;Inherit;False;Property;_WaterIceIntensity;Water Ice Intensity;15;0;Create;True;0;0;0;False;0;False;0.6;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;604;-4001.024,-1134.096;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;606;-3841.196,-1135.353;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;775;-4823.436,544.9089;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;824;-4485.623,666.122;Inherit;False;Property;_SolidTop;_SolidTop;16;0;Create;True;0;0;0;False;0;False;0.1373401;0.892;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;831;-4489.553,740.7753;Inherit;False;Property;_TopFadeLenght;_TopFadeLenght;17;0;Create;True;0;0;0;False;0;False;0.3475586;0.788;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;873;-4482.861,918.3699;Inherit;False;Property;_SolidBottom;_SolidBottom;19;0;Create;True;0;0;0;False;0;False;0.3649352;0.683;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;862;-4376.447,833.0414;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;897;-4596.144,830.662;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;180;-4808.235,-269.9959;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;175;-4596.253,-306.5913;Inherit;True;Property;_GassTexture;GassTexture;5;0;Create;True;0;0;0;False;0;False;-1;None;fcb3e019a33c36d41a3ebbc5b14f61a0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;185;-4800.43,-153.0367;Inherit;False;Property;_GrassTextureTiling;GrassTextureTiling;10;0;Create;True;0;0;0;False;0;False;-0.14;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-4238.83,-303.9957;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-4494.235,-78.99617;Inherit;False;46;SurfaceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;611;-3782.65,102.0112;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;705;-4623.44,218.9541;Inherit;False;706;IceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;572;-4727.841,310.815;Inherit;False;Property;_SurfaceIceIntensity;Surface Ice Intensity;14;0;Create;True;0;0;0;False;0;False;0.9;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;600;-4274.069,247.9499;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;599;-4063.82,258.2349;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-4233.825,33.84249;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;-3739.359,136.2313;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;109;-4825.578,38.41739;Inherit;False;Constant;_Grass;Grass;5;0;Create;True;0;0;0;False;0;False;0,0.4745098,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;150;-1331.253,1184.7;Inherit;True;LavaWater;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;59;-2662.462,-457.7181;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;350;-2427.354,-367.9999;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;593;-2251.977,-369.379;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;42;-2664.853,-286.1557;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1537.354,1192.032;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;126;-2424.036,760.1433;Inherit;False;Constant;_PannerSpeed;Panner Speed;7;0;Create;True;0;0;0;False;0;False;0.05,0.075;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;304;-2450.513,895.5839;Inherit;False;Property;_PannerSpeedMultiplier;PannerSpeedMultiplier;12;0;Create;True;0;0;0;False;0;False;2;10;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;303;-2204.513,820.5836;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;301;-2240.719,530.9476;Inherit;False;61;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;302;-2297.513,623.583;Inherit;False;Property;_MinToIncreasePannerSpeed;MinToIncreasePannerSpeed;11;0;Create;True;0;0;0;False;0;False;0.35;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;307;-1936.799,822.2865;Inherit;False;Property;_TimeScale;TimeScale;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;118;-2624.846,462.3159;Inherit;True;Property;_FlowMap;FlowMap;3;0;Create;True;0;0;0;False;0;False;-1;None;a2567c79fcafff541bd097660979fea8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;117;-2588.914,286.0303;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;124;-2608.579,668.3744;Inherit;False;Property;_FlowIntensity;Flow Intensity;8;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;306;-1765.799,708.2863;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;300;-1996.721,546.9478;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0.35;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;123;-2198.612,284.1912;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;119;-1848.32,387.0597;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;127;-2873.242,489.5801;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;121;-2988.349,297.2284;Inherit;False;Property;_WaterTiling;WaterTiling;9;0;Create;True;0;0;0;False;0;False;5.06;10;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;145;-1926.675,1187.89;Inherit;True;Property;_WaterDamage;WaterDamage;2;0;Create;True;0;0;0;False;0;False;-1;7bea6e4487f50884f9d91abf7953b2a7;7bea6e4487f50884f9d91abf7953b2a7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;146;-1850.064,1382.84;Inherit;False;Constant;_WaterDamageColor;WaterDamageColor;5;0;Create;True;0;0;0;False;0;False;1,0.4725828,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;100;-2976.179,1174.582;Inherit;True;Property;_SurfaceDamage;SurfaceDamage;1;0;Create;True;0;0;0;False;0;False;-1;None;f0ec32efe9d542045877f97847d48f9f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2650.425,1180.038;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2429.746,1174.392;Inherit;True;LavaSurface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;-2876.177,1360.854;Inherit;False;46;SurfaceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;-1621.501,1430.327;Inherit;False;44;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;186;-2105.497,1224.246;Inherit;False;138;Panner;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-1603.975,380.7416;Inherit;True;Panner;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;615;-3176.884,-99.98548;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;182;-3648.264,-89.26481;Inherit;False;Constant;_SurfaceColor;Surface Color;10;0;Create;True;0;0;0;False;0;False;0.04298684,0.3962264,0.04298684,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;616;-3414.8,80.2635;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-3402.671,-315.4926;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;719;-3592.989,155.9762;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;864;-4467.707,1005.513;Inherit;False;Property;_BottomFadeLenght;_BottomFadeLenght;18;0;Create;True;0;0;0;False;0;False;0;0.938;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;598;-3930.958,-86.11716;Inherit;False;Constant;_SurfaceStep;Surface Step;18;0;Create;True;0;0;0;False;0;False;0.49;0;0.1;0.49;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;160;-3657.92,-311.7566;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.46;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;158;-3972.178,-314.0236;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;11.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;169;-3384.968,154.1499;Inherit;True;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;402;-1878.25,-375.886;Inherit;False;FinalSurface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-2953.797,-556.8165;Inherit;False;150;LavaWater;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-2951.08,-437.9341;Inherit;False;172;Water;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;-2950.604,-315.1741;Inherit;False;168;LavaSurface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-2952.264,-110.514;Inherit;False;61;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-2950.923,-218.3686;Inherit;False;169;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;592;-1604.554,-1154.447;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;372;-1377.882,-1157.785;Float;False;True;-1;2;ASEMaterialInspector;100;5;EarthSurface;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.GetLocalVarNode;1052;-2020.3,-1162.49;Inherit;False;402;FinalSurface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1055;-4121.389,880.2159;Inherit;False;FadeMask;-1;;13;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1056;-4116.043,590.5623;Inherit;False;FadeMask;-1;;14;82343dc598e55714e83c410ab3b8d20a;0;4;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;12;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;867;-3687.504,830.9921;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;706;-3487.331,799.4509;Inherit;True;IceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;17;0;4;3
WireConnection;61;0;43;0
WireConnection;24;0;17;0
WireConnection;24;1;77;0
WireConnection;27;0;24;0
WireConnection;44;0;27;0
WireConnection;46;0;24;0
WireConnection;143;1;137;0
WireConnection;31;0;143;0
WireConnection;31;1;151;0
WireConnection;31;2;45;0
WireConnection;172;0;606;0
WireConnection;559;0;45;0
WireConnection;559;1;454;0
WireConnection;559;2;573;0
WireConnection;570;0;559;0
WireConnection;604;0;31;0
WireConnection;604;1;570;0
WireConnection;606;0;604;0
WireConnection;862;0;897;0
WireConnection;897;0;775;2
WireConnection;180;0;185;0
WireConnection;175;1;180;0
WireConnection;177;0;175;0
WireConnection;177;1;179;0
WireConnection;611;0;616;0
WireConnection;600;0;179;0
WireConnection;600;1;705;0
WireConnection;600;2;572;0
WireConnection;599;0;600;0
WireConnection;29;0;109;0
WireConnection;29;1;179;0
WireConnection;176;0;611;0
WireConnection;176;1;29;0
WireConnection;176;2;599;0
WireConnection;150;0;147;0
WireConnection;59;0;152;0
WireConnection;59;1;173;0
WireConnection;59;2;60;0
WireConnection;350;0;59;0
WireConnection;350;1;42;0
WireConnection;593;0;350;0
WireConnection;42;0;171;0
WireConnection;42;1;170;0
WireConnection;42;2;60;0
WireConnection;147;0;145;0
WireConnection;147;1;146;0
WireConnection;147;2;148;0
WireConnection;303;0;126;0
WireConnection;303;1;304;0
WireConnection;118;1;127;0
WireConnection;117;0;121;0
WireConnection;306;0;307;0
WireConnection;300;0;301;0
WireConnection;300;1;302;0
WireConnection;300;2;126;0
WireConnection;300;3;303;0
WireConnection;300;4;303;0
WireConnection;123;0;117;0
WireConnection;123;1;118;0
WireConnection;123;2;124;0
WireConnection;119;0;123;0
WireConnection;119;2;300;0
WireConnection;119;1;306;0
WireConnection;127;0;121;0
WireConnection;145;1;186;0
WireConnection;39;0;100;0
WireConnection;39;1;166;0
WireConnection;168;0;39;0
WireConnection;138;0;119;0
WireConnection;615;0;178;0
WireConnection;616;0;615;0
WireConnection;178;0;160;0
WireConnection;178;1;182;0
WireConnection;719;0;176;0
WireConnection;160;0;158;0
WireConnection;160;1;598;0
WireConnection;158;0;177;0
WireConnection;169;0;719;0
WireConnection;402;0;593;0
WireConnection;592;0;1052;0
WireConnection;372;0;592;0
WireConnection;1055;8;862;0
WireConnection;1055;9;873;0
WireConnection;1055;10;864;0
WireConnection;1056;8;775;2
WireConnection;1056;9;824;0
WireConnection;1056;10;831;0
WireConnection;867;0;1056;0
WireConnection;867;1;1055;0
WireConnection;706;0;867;0
ASEEND*/
//CHKSM=67BB4B9B66D332436B1FDD5CA452D2FEA734FBE3