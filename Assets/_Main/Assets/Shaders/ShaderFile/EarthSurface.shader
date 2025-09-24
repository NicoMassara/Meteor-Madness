// Made with Amplify Shader Editor
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
			Tags { "LightMode"="ForwardBase" }
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
			uniform sampler2D _SurfaceDamage;
			uniform float4 _SurfaceDamage_ST;
			uniform sampler2D _GassTexture;
			uniform float _GrassTextureTiling;
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
				float4 Water172 = ( tex2D( _WaterDefault, Panner138 ) * color151 * WaterMask44 );
				float4 lerpResult59 = lerp( LavaWater150 , Water172 , HealthAmount61);
				float2 uv_SurfaceDamage = i.ase_texcoord1.xy * _SurfaceDamage_ST.xy + _SurfaceDamage_ST.zw;
				float SurfaceMask46 = temp_output_24_0;
				float4 LavaSurface168 = ( tex2D( _SurfaceDamage, uv_SurfaceDamage ) * SurfaceMask46 );
				float4 color182 = IsGammaSpace() ? float4(0.04298684,0.3962264,0.04298684,1) : float4(0.003334239,0.130239,0.003334239,1);
				float2 temp_cast_4 = (_GrassTextureTiling).xx;
				float2 texCoord180 = i.ase_texcoord1.xy * temp_cast_4 + float2( 0,0 );
				float simplePerlin2D158 = snoise( ( tex2D( _GassTexture, texCoord180 ) * SurfaceMask46 ).rg*11.8 );
				simplePerlin2D158 = simplePerlin2D158*0.5 + 0.5;
				float4 color109 = IsGammaSpace() ? float4(0,0.4745098,0,1) : float4(0,0.1912017,0,1);
				float4 Surface169 = ( ( color182 * step( simplePerlin2D158 , 0.46 ) ) + ( color109 * SurfaceMask46 ) );
				float4 lerpResult42 = lerp( LavaSurface168 , Surface169 , HealthAmount61);
				
				
				finalColor = saturate( ( lerpResult59 + lerpResult42 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
323;73;1523;935;2147.983;513.8279;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;52;-3074.903,-316.7787;Inherit;False;847.9467;613.9975;Masking;7;4;17;24;46;27;44;77;Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;65;-3063.4,-524.6848;Inherit;False;572.9548;166.825;Health Amount;2;43;61;Health Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;144;-3107.296,1002.962;Inherit;False;1628.177;780.281;;16;300;301;126;119;138;123;118;124;117;127;121;302;303;304;306;307;WaterPanner;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;4;-3025.94,-266.7788;Inherit;True;Property;_EarthMap;EarthMap;0;0;Create;True;0;0;0;False;0;False;-1;c036000bb622eae43b6b10cd89a3a25d;c036000bb622eae43b6b10cd89a3a25d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;121;-3057.296,1066;Inherit;False;Property;_WaterTiling;WaterTiling;9;0;Create;True;0;0;0;False;0;False;5.06;10;0.1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;184;-5228.924,-263.2907;Inherit;False;2059.387;928.6257;Comment;14;169;176;29;178;109;182;160;158;177;179;175;180;47;185;Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-3013.4,-473.8598;Inherit;False;Property;_HealthAmount;HealthAmount;6;0;Create;True;0;0;0;False;0;False;-0.26;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;17;-2730.877,-276.9059;Inherit;True;True;True;True;True;1;0;FLOAT;-0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-3001.191,178.0699;Inherit;False;Property;_StepValue;StepValue;7;0;Create;True;0;0;0;False;0;False;0.03752942;0.361;0.03;0.4;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;126;-2492.983,1528.913;Inherit;False;Constant;_PannerSpeed;Panner Speed;7;0;Create;True;0;0;0;False;0;False;0.05,0.075;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;127;-2942.189,1258.351;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-2714.445,-474.6848;Inherit;False;HealthAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;304;-2519.46,1664.353;Inherit;False;Property;_PannerSpeedMultiplier;PannerSpeedMultiplier;12;0;Create;True;0;0;0;False;0;False;2;10;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;24;-2985.837,-58.10147;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-5115.435,41.49957;Inherit;False;Property;_GrassTextureTiling;GrassTextureTiling;10;0;Create;True;0;0;0;False;0;False;-0.14;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-2433.576,-100.8491;Inherit;False;SurfaceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;302;-2366.46,1392.353;Inherit;False;Property;_MinToIncreasePannerSpeed;MinToIncreasePannerSpeed;11;0;Create;True;0;0;0;False;0;False;0.35;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;303;-2273.46,1589.353;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;301;-2309.666,1299.718;Inherit;False;61;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;307;-2005.744,1591.056;Inherit;False;Property;_TimeScale;TimeScale;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;118;-2693.793,1231.087;Inherit;True;Property;_FlowMap;FlowMap;3;0;Create;True;0;0;0;False;0;False;-1;None;a2567c79fcafff541bd097660979fea8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;180;-4858.924,10.48092;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;117;-2657.861,1054.802;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;124;-2677.526,1437.144;Inherit;False;Property;_FlowIntensity;Flow Intensity;8;0;Create;True;0;0;0;False;0;False;0;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;175;-4646.942,-26.11481;Inherit;True;Property;_GassTexture;GassTexture;5;0;Create;True;0;0;0;False;0;False;-1;None;fcb3e019a33c36d41a3ebbc5b14f61a0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;179;-4542.924,175.4809;Inherit;False;46;SurfaceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;306;-1834.744,1477.056;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;300;-2065.666,1315.718;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0.35;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;123;-2267.559,1052.963;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-4274.924,-23.5191;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;119;-1917.265,1155.831;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-1673.92,1184.513;Inherit;True;Panner;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;27;-2666.946,6.218773;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;149;-2130.426,2035.215;Inherit;False;1013.76;521.7198;Comment;6;148;146;145;147;150;186;LavaWater;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;165;-3087.457,366.7501;Inherit;False;1003.961;533.0252;Comment;6;172;31;151;143;45;137;Water;1,1,1,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;158;-3989.417,-24.05008;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;11.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-2427.957,-0.910593;Inherit;False;WaterMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;186;-2116.56,2120.358;Inherit;False;138;Panner;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-3037.457,437.5991;Inherit;False;138;Panner;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;182;-3740.514,-213.2907;Inherit;False;Constant;_Color0;Color 0;10;0;Create;True;0;0;0;False;0;False;0.04298684,0.3962264,0.04298684,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;47;-4534.632,573.9264;Inherit;False;46;SurfaceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;109;-4548.674,359.5589;Inherit;False;Constant;_Grass;Grass;5;0;Create;True;0;0;0;False;0;False;0,0.4745098,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;160;-3720.168,-21.78308;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.46;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;167;-3087.408,1955.094;Inherit;False;825.4619;365.9649;Comment;4;168;39;166;100;Lava Surface;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;151;-2791.891,612.6889;Inherit;False;Constant;_WaterColor;WaterColor;9;0;Create;True;0;0;0;False;0;False;0,0.4434402,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;145;-1937.737,2084.002;Inherit;True;Property;_WaterDamage;WaterDamage;2;0;Create;True;0;0;0;False;0;False;-1;7bea6e4487f50884f9d91abf7953b2a7;7bea6e4487f50884f9d91abf7953b2a7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-4271.764,364.7545;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;-2937.406,2191.365;Inherit;False;46;SurfaceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;146;-1861.126,2278.952;Inherit;False;Constant;_WaterDamageColor;WaterDamageColor;5;0;Create;True;0;0;0;False;0;False;1,0.4725828,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;100;-3037.408,2005.093;Inherit;True;Property;_SurfaceDamage;SurfaceDamage;1;0;Create;True;0;0;0;False;0;False;-1;None;f0ec32efe9d542045877f97847d48f9f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;148;-1632.563,2326.439;Inherit;False;44;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-3460.923,-21.5191;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;143;-2870.883,427.1549;Inherit;True;Property;_WaterDefault;WaterDefault;4;0;Create;True;0;0;0;False;0;False;-1;6fe629e0670cf8c4d980fd5b34631dcb;6fe629e0670cf8c4d980fd5b34631dcb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;45;-2753.831,782.1129;Inherit;False;44;WaterMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-2505.241,431.017;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;-3682.321,357.1323;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1548.416,2088.144;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2711.654,2010.549;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;169;-3466.158,356.4629;Inherit;True;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-2328.128,424.2031;Inherit;True;Water;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;174;-2015.682,-320.6091;Inherit;False;1331.466;614.4094;Comment;10;372;299;350;42;59;173;171;170;152;60;Out;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;150;-1342.315,2080.812;Inherit;True;LavaWater;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2490.975,2004.903;Inherit;True;LavaSurface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-1940.341,63.83933;Inherit;False;169;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-1965.682,174.694;Inherit;False;61;HealthAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;-1958.022,-19.96633;Inherit;False;168;LavaSurface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-1957.498,-153.7264;Inherit;False;172;Water;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-1948.215,-270.6091;Inherit;False;150;LavaWater;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;42;-1680.271,-1.947931;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;59;-1677.881,-173.5104;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;350;-1442.773,-83.79218;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;299;-1291.812,-83.89305;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;372;-1036.669,-88.84527;Float;False;True;-1;2;ASEMaterialInspector;100;1;EarthSurface;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;17;0;4;3
WireConnection;127;0;121;0
WireConnection;61;0;43;0
WireConnection;24;0;17;0
WireConnection;24;1;77;0
WireConnection;46;0;24;0
WireConnection;303;0;126;0
WireConnection;303;1;304;0
WireConnection;118;1;127;0
WireConnection;180;0;185;0
WireConnection;117;0;121;0
WireConnection;175;1;180;0
WireConnection;306;0;307;0
WireConnection;300;0;301;0
WireConnection;300;1;302;0
WireConnection;300;2;126;0
WireConnection;300;3;303;0
WireConnection;300;4;303;0
WireConnection;123;0;117;0
WireConnection;123;1;118;0
WireConnection;123;2;124;0
WireConnection;177;0;175;0
WireConnection;177;1;179;0
WireConnection;119;0;123;0
WireConnection;119;2;300;0
WireConnection;119;1;306;0
WireConnection;138;0;119;0
WireConnection;27;0;24;0
WireConnection;158;0;177;0
WireConnection;44;0;27;0
WireConnection;160;0;158;0
WireConnection;145;1;186;0
WireConnection;29;0;109;0
WireConnection;29;1;47;0
WireConnection;178;0;182;0
WireConnection;178;1;160;0
WireConnection;143;1;137;0
WireConnection;31;0;143;0
WireConnection;31;1;151;0
WireConnection;31;2;45;0
WireConnection;176;0;178;0
WireConnection;176;1;29;0
WireConnection;147;0;145;0
WireConnection;147;1;146;0
WireConnection;147;2;148;0
WireConnection;39;0;100;0
WireConnection;39;1;166;0
WireConnection;169;0;176;0
WireConnection;172;0;31;0
WireConnection;150;0;147;0
WireConnection;168;0;39;0
WireConnection;42;0;171;0
WireConnection;42;1;170;0
WireConnection;42;2;60;0
WireConnection;59;0;152;0
WireConnection;59;1;173;0
WireConnection;59;2;60;0
WireConnection;350;0;59;0
WireConnection;350;1;42;0
WireConnection;299;0;350;0
WireConnection;372;0;299;0
ASEEND*/
//CHKSM=EA1CED27F868E324F5E426CA72C0E0440395A2F5