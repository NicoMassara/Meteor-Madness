// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DeepSpace"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_StarsSize("StarsSize", Float) = 0
		_StarsAmount("StarsAmount", Range( 0 , 1)) = 0
		_NebulaColor("NebulaColor", Color) = (0.4500256,0,1,0)
		_MaskSize("MaskSize", Range( 0 , 1)) = 0.6235294
		_MaskOutterIntensity("MaskOutterIntensity", Range( 0 , 10)) = 3.32
		_MaskInnerIntensity("MaskInnerIntensity", Range( 1 , 10)) = 0
		_NebulaNoiseIntensity("NebulaNoiseIntensity", Range( 0 , 1)) = 3.3
		_NebulaIntensity("NebulaIntensity", Range( 0 , 1)) = 0.5
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
			#define ASE_NEEDS_FRAG_POSITION


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
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _StarsSize;
			uniform float _StarsAmount;
			uniform float _NebulaNoiseIntensity;
			uniform float _MaskSize;
			uniform float _MaskOutterIntensity;
			uniform float _MaskInnerIntensity;
			uniform float4 _NebulaColor;
			uniform float _NebulaIntensity;
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
				o.ase_texcoord2 = v.vertex;
				
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
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float simplePerlin2D346 = snoise( i.ase_texcoord2.xyz.xy*( _StarsSize / 2.0 ) );
				simplePerlin2D346 = simplePerlin2D346*0.5 + 0.5;
				float temp_output_339_0 = (-15.0 + (_StarsAmount - 0.0) * (-8.0 - -15.0) / (1.0 - 0.0));
				float simplePerlin2D337 = snoise( i.ase_texcoord2.xyz.xy*_StarsSize );
				simplePerlin2D337 = simplePerlin2D337*0.5 + 0.5;
				float Stars453 = max( (temp_output_339_0 + (simplePerlin2D346 - 0.0) * (1.0 - temp_output_339_0) / (1.0 - 0.0)) , (temp_output_339_0 + (simplePerlin2D337 - 0.0) * (1.0 - temp_output_339_0) / (1.0 - 0.0)) );
				float4 temp_cast_2 = (Stars453).xxxx;
				float simplePerlin2D301 = snoise( i.ase_texcoord2.xyz.xy*25.0 );
				simplePerlin2D301 = simplePerlin2D301*0.5 + 0.5;
				float Nebula312 = (0.0 + (( -simplePerlin2D301 * _NebulaNoiseIntensity ) - 0.1) * (1.0 - 0.0) / (1.0 - 0.1));
				float temp_output_236_0 = (0.0 + (_MaskSize - 0.0) * (0.25 - 0.0) / (1.0 - 0.0));
				float NebulaMask223 = (0.0 + (( ( -( i.ase_texcoord2.xyz.y - temp_output_236_0 ) * ( i.ase_texcoord2.xyz.y + temp_output_236_0 ) * _MaskOutterIntensity ) * _MaskInnerIntensity ) - 0.0) * (1.0 - 0.0) / (1.0 - 0.0));
				float4 MaskedNebula238 = ( saturate( ( ( Nebula312 + NebulaMask223 ) * _NebulaColor ) ) * _NebulaIntensity );
				
				
				finalColor = ( tex2D( _MainTex, uv_MainTex ) * max( temp_cast_2 , MaskedNebula238 ) );
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
Node;AmplifyShaderEditor.CommentaryNode;454;-2584.909,-759.9319;Inherit;False;914.0962;505.4064;Comment;6;318;264;192;1;240;326;Out;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;352;-4199.339,-793.3651;Inherit;False;1194.373;766.2482;Comment;11;353;350;351;338;339;346;337;328;334;335;453;Stars;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;314;-4191.825,1150.146;Inherit;False;1570.792;402.5822;Comment;0;Nebula;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;270;-4201.635,599.0867;Inherit;False;1313.903;517.8859;Comment;0;MaskedNebula;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;269;-4207.777,38.9539;Inherit;False;1874.353;552.2298;;0;Nebula Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;225;-3547.775,227.9053;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;227;-3554.775,93.90542;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;232;-3350.775,95.90541;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-3207.775,202.9053;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;-3011.315,200.3153;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;236;-3828.048,264.1502;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;219;-3891.786,88.95389;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;311;-3594.929,1208.138;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;301;-3891.329,1202.782;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;251;-3478.07,696.9528;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;-3647.633,698.1618;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;238;-3172.071,693.9528;Inherit;True;MaskedNebula;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;309;-3318.222,700.2855;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-3543.222,851.2855;Inherit;False;Property;_NebulaIntensity;NebulaIntensity;8;0;Create;True;0;0;0;False;0;False;0.5;0.399;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;303;-3870.816,693.394;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;313;-4175.65,686.9092;Inherit;True;312;Nebula;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;-4166.457,900.2029;Inherit;True;223;NebulaMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;-3621.048,396.1503;Inherit;False;Property;_MaskOutterIntensity;MaskOutterIntensity;5;0;Create;True;0;0;0;False;0;False;3.32;7.5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;226;-4141.777,254.9053;Inherit;False;Property;_MaskSize;MaskSize;4;0;Create;True;0;0;0;False;0;False;0.6235294;0.546;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;293;-3207.315,369.3155;Inherit;False;Property;_MaskInnerIntensity;MaskInnerIntensity;6;0;Create;True;0;0;0;False;0;False;0;8.83;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;212;-3837.841,874.3215;Inherit;False;Property;_NebulaColor;NebulaColor;3;0;Create;True;0;0;0;False;0;False;0.4500256,0,1,0;0.4221258,0,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;305;-3380.101,1209.911;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;308;-3486.101,1342.911;Inherit;False;Property;_NebulaNoiseIntensity;NebulaNoiseIntensity;7;0;Create;True;0;0;0;False;0;False;3.3;0.341;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;312;-2845.372,1193.775;Inherit;True;Nebula;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;323;-3135.136,1206.592;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;223;-2546.272,193.237;Inherit;True;NebulaMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;327;-2801.223,196.6002;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;302;-4162.074,1210.654;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;335;-4110.757,-302.7006;Inherit;False;Property;_StarsSize;StarsSize;1;0;Create;True;0;0;0;False;0;False;0;200;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;334;-4112.757,-191.7006;Inherit;False;Property;_StarsAmount;StarsAmount;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;337;-3834.758,-497.7006;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;346;-3831.34,-743.3651;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;339;-3723.791,-213.9169;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-15;False;4;FLOAT;-8;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;338;-3463.16,-491.7006;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;351;-3162.085,-560.4132;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;350;-3464.085,-697.4132;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;353;-4098.085,-704.4132;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;328;-4138.818,-491.933;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;453;-3392.766,-235.4042;Inherit;False;Stars;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;318;-2069.47,-594.9627;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;264;-2265.49,-449.1932;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;-2520.599,-478.4512;Inherit;False;453;Stars;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2534.909,-709.9319;Inherit;True;Property;_MainTex;_MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;240;-2516.379,-369.6855;Inherit;False;238;MaskedNebula;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;326;-1897.173,-592.7801;Float;False;True;-1;2;ASEMaterialInspector;100;5;DeepSpace;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;225;0;219;2
WireConnection;225;1;236;0
WireConnection;227;0;219;2
WireConnection;227;1;236;0
WireConnection;232;0;227;0
WireConnection;230;0;232;0
WireConnection;230;1;225;0
WireConnection;230;2;235;0
WireConnection;291;0;230;0
WireConnection;291;1;293;0
WireConnection;236;0;226;0
WireConnection;311;0;301;0
WireConnection;301;0;302;0
WireConnection;251;0;297;0
WireConnection;297;0;303;0
WireConnection;297;1;212;0
WireConnection;238;0;309;0
WireConnection;309;0;251;0
WireConnection;309;1;310;0
WireConnection;303;0;313;0
WireConnection;303;1;224;0
WireConnection;305;0;311;0
WireConnection;305;1;308;0
WireConnection;312;0;323;0
WireConnection;323;0;305;0
WireConnection;223;0;327;0
WireConnection;327;0;291;0
WireConnection;337;0;328;0
WireConnection;337;1;335;0
WireConnection;346;0;328;0
WireConnection;346;1;353;0
WireConnection;339;0;334;0
WireConnection;338;0;337;0
WireConnection;338;3;339;0
WireConnection;351;0;350;0
WireConnection;351;1;338;0
WireConnection;350;0;346;0
WireConnection;350;3;339;0
WireConnection;353;0;335;0
WireConnection;453;0;351;0
WireConnection;318;0;1;0
WireConnection;318;1;264;0
WireConnection;264;0;192;0
WireConnection;264;1;240;0
WireConnection;326;0;318;0
ASEEND*/
//CHKSM=6ADC018B561919C20A9CAD2B2A4FEA57F1FC91AE