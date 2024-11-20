// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rain/RainScroller"
{
	Properties
	{
		_DropletMask("Droplet Mask", 2D) = "white" {}
		_Distortion("Distortion", Float) = 0.01
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Tint("Tint", Color) = (0.8809185,0.9188843,0.9245283,1)
		_Droplets_Strength("Droplets_Strength", Range( 0 , 1)) = 1
		_RivuletMask("Rivulet Mask", 2D) = "white" {}
		_GlobalRotation("Global Rotation", Range( -180 , 180)) = 0
		_RivuletRotation("Rivulet Rotation", Range( -180 , 180)) = 0
		_RivuletSpeed("Rivulet Speed", Range( 0 , 2)) = 0.2
		_RivuletsStrength("Rivulets Strength", Range( 0 , 3)) = 1
		_DropletsGravity("Droplets Gravity", Range( 0 , 1)) = 0
		_DropletsStrikeSpeed("Droplets Strike Speed", Range( 0 , 2)) = 0.3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "_GrabScreen0" }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
			float3 worldNormal;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabScreen0 )
		uniform sampler2D _DropletMask;
		uniform float _DropletsGravity;
		uniform float2 _Tiling;
		uniform float _GlobalRotation;
		uniform float _DropletsStrikeSpeed;
		uniform float _Droplets_Strength;
		uniform sampler2D _RivuletMask;
		uniform float _RivuletRotation;
		uniform float _RivuletSpeed;
		uniform float _RivuletsStrength;
		uniform float _Distortion;
		uniform float4 _Tint;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 color217 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			o.Albedo = color217.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult145 = (float2(0.0 , _DropletsGravity));
			float2 uv_TexCoord3 = i.uv_texcoord * _Tiling;
			float2 panner143 = ( 1.0 * _Time.y * appendResult145 + uv_TexCoord3);
			float cos76 = cos( radians( _GlobalRotation ) );
			float sin76 = sin( radians( _GlobalRotation ) );
			float2 rotator76 = mul( panner143 - float2( 0,0 ) , float2x2( cos76 , -sin76 , sin76 , cos76 )) + float2( 0,0 );
			float4 tex2DNode2 = tex2D( _DropletMask, rotator76 );
			float4 temp_cast_1 = (1.0).xxxx;
			float4 break171 = ( ( tex2DNode2 * 2.0 ) - temp_cast_1 );
			float4 appendResult172 = (float4(break171.r , break171.g , 0.0 , 0.0));
			float mulTime4 = _Time.y * _DropletsStrikeSpeed;
			float4 break201 = ( appendResult172 * saturate( ceil( (0.0 + (( tex2DNode2.a - frac( ( (-1.0 + (tex2DNode2.b - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + mulTime4 ) ) ) - ( 1.0 - _Droplets_Strength )) * (1.0 - 0.0) / (1.0 - ( 1.0 - _Droplets_Strength ))) ) ) );
			float4 appendResult200 = (float4(break201.x , break201.y , break201.z , break201.w));
			float cos85 = cos( radians( _RivuletRotation ) );
			float sin85 = sin( radians( _RivuletRotation ) );
			float2 rotator85 = mul( rotator76 - float2( 0,0 ) , float2x2( cos85 , -sin85 , sin85 , cos85 )) + float2( 0,0 );
			float4 break90 = tex2D( _RivuletMask, rotator85 );
			float2 appendResult91 = (float2(break90.b , break90.a));
			float2 temp_output_126_0 = (float2( -0.1,0 ) + (appendResult91 - float2( 0,0 )) * (float2( 0.1,3 ) - float2( -0.1,0 )) / (float2( 1,1 ) - float2( 0,0 )));
			float mulTime92 = _Time.y * 0.23;
			float temp_output_1_0_g15 = mulTime92;
			float rest1_ratio102 = (0.0 + (( ( temp_output_1_0_g15 - floor( ( temp_output_1_0_g15 + 0.5 ) ) ) * 2 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0));
			float mulTime132 = _Time.y * _RivuletSpeed;
			float2 appendResult133 = (float2(0.0 , mulTime132));
			float temp_output_1_0_g14 = (mulTime92*1.0 + 0.5);
			float rest2_ratio106 = (0.0 + (( ( temp_output_1_0_g14 - floor( ( temp_output_1_0_g14 + 0.5 ) ) ) * 2 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0));
			float temp_output_1_0_g16 = mulTime92;
			float bias114 = pow( (0.0 + (( ( ( abs( ( ( temp_output_1_0_g16 - floor( ( temp_output_1_0_g16 + 0.5 ) ) ) * 2 ) ) * 2 ) - 1.0 ) + 0.0 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) , 2.0 );
			float4 lerpResult130 = lerp( tex2D( _RivuletMask, ( ( temp_output_126_0 * rest1_ratio102 ) + rotator85 + appendResult133 ) ) , tex2D( _RivuletMask, ( ( temp_output_126_0 * rest2_ratio106 ) + rotator85 + appendResult133 ) ) , bias114);
			float4 temp_cast_2 = (1.0).xxxx;
			Gradient gradient213 = NewGradient( 0, 2, 2, float4( 1, 1, 1, 0.8500038 ), float4( 0, 0, 0, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult191 = dot( ase_worldNormal , float3(0,1,0) );
			float4 temp_output_41_0 = ( ase_grabScreenPosNorm + ( ( appendResult200 + ( ( ( ( lerpResult130 * 2.0 ) - temp_cast_2 ) * float4(1,1,0,0) ) * _RivuletsStrength * SampleGradient( gradient213, abs( dotResult191 ) ).r ) ) * _Distortion ) );
			float4 screenColor45 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabScreen0,temp_output_41_0.xy/temp_output_41_0.w);
			o.Emission = ( screenColor45 * _Tint ).rgb;
			o.Metallic = 0.0;
			o.Smoothness = 0.72;
			o.Alpha = 1.0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
788.6667;268.6667;1154;872;-2875.96;913.939;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;185;-3346.149,-655.5524;Inherit;False;3552.048;1229.899;Raindrops;31;12;144;3;145;77;143;78;76;1;2;184;4;183;180;159;161;61;62;163;181;160;34;162;25;171;29;172;38;64;65;66;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-3141.011,78.6628;Inherit;False;Property;_DropletsGravity;Droplets Gravity;10;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;12;-3296.149,-103.2215;Inherit;False;Property;_Tiling;Tiling;2;0;Create;True;0;0;0;False;0;False;1,1;8,6;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;145;-2763.708,87.00745;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-3083.69,-112.7801;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;77;-3003.609,277.8702;Inherit;False;Property;_GlobalRotation;Global Rotation;6;0;Create;True;0;0;0;False;0;False;0;0;-180;180;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;193;-2614.845,934.7326;Inherit;False;4544.834;1726.802;Rivulets;27;83;84;95;97;125;107;135;129;85;126;130;117;133;109;132;108;79;91;90;68;67;187;186;208;210;209;207;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-2572.595,1642.966;Inherit;False;Property;_RivuletRotation;Rivulet Rotation;7;0;Create;True;0;0;0;False;0;False;0;0;-180;180;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;138;-2951.183,3176.139;Inherit;False;1841.358;943.51;Timing;15;92;124;103;119;101;123;104;102;106;120;113;134;114;121;194;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;143;-2663.708,-142.9926;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;78;-2681.211,284.4903;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;84;-2220.5,1637.544;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;76;-2429.178,38.14053;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;92;-2901.183,3489.695;Inherit;False;1;0;FLOAT;0.23;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;124;-2552.864,3500.68;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;85;-2007.972,1596.534;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;67;-2248.496,2432.594;Inherit;True;Property;_RivuletMask;Rivulet Mask;5;0;Create;True;0;0;0;False;0;False;bd357bf0331325748a7c1bd168220b67;bd357bf0331325748a7c1bd168220b67;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;1;-2383.969,-389.6209;Inherit;True;Property;_DropletMask;Droplet Mask;0;0;Create;True;0;0;0;False;0;False;76ae1285472e6ce48a2f01ef7905b8fd;76ae1285472e6ce48a2f01ef7905b8fd;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;103;-2249.6,3513.924;Inherit;False;Sawtooth Wave;-1;;14;289adb816c3ac6d489f255fc3caf5016;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;119;-2247.544,3237.823;Inherit;False;Sawtooth Wave;-1;;15;289adb816c3ac6d489f255fc3caf5016;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;68;-1723.261,1875.321;Inherit;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;101;-1789.975,3230.481;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;104;-1779.678,3501.962;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;-2594.186,459.3465;Inherit;False;Property;_DropletsStrikeSpeed;Droplets Strike Speed;11;0;Create;True;0;0;0;False;0;False;0.3;0.08362373;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;90;-1350.854,1888.57;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;2;-2045.154,-384.3428;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;123;-2421.437,3824.195;Inherit;False;Triangle Wave;-1;;16;51ec3c8d117f3ec4fa3742c3e00d535b;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;121;-2432.272,3970.731;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;91;-1212.312,1925.437;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;120;-2103.986,3917.204;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;4;-2104.096,363.4884;Inherit;False;1;0;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;183;-2038.025,115.021;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-1370.612,3226.139;Inherit;False;rest1_ratio;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;106;-1358.49,3501.159;Inherit;False;rest2_ratio;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-1219.027,1629.254;Inherit;False;Property;_RivuletSpeed;Rivulet Speed;8;0;Create;True;0;0;0;False;0;False;0.2;0.0075;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-992.4468,2281.274;Inherit;False;106;rest2_ratio;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;132;-901.3378,1638.146;Inherit;False;1;0;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;113;-1942.115,3961.407;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;126;-1008.694,1909.833;Inherit;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-0.1,0;False;4;FLOAT2;0.1,3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;-1003.078,1270.61;Inherit;False;102;rest1_ratio;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-1762.644,210.974;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;134;-1690.98,3883.677;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-748.6919,1218.143;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1083.095,328.0911;Inherit;False;Property;_Droplets_Strength;Droplets_Strength;4;0;Create;True;0;0;0;False;0;False;1;0.9928027;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;159;-1576.137,213.4214;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-1312.197,-497.6755;Inherit;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;133;-677.0065,1613.037;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-708.3076,2240.243;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-1115.197,-456.6756;Inherit;False;Constant;_Float3;Float 3;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;129;-196.7576,1830.622;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;181;-1297.123,-39.87589;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-205.9469,1366.775;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;-1144.197,-357.6756;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;-1385.063,3917.905;Inherit;False;bias;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;62;-748.6647,336.5907;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;135;134.679,1818.097;Inherit;True;Property;_TextureSample3;Texture Sample 3;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;117;492.3798,1941.183;Inherit;False;114;bias;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;34;-589.9092,-45.61497;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;162;-914.1967,-460.6756;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;95;140.5874,1338.537;Inherit;True;Property;_TextureSample2;Texture Sample 2;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;130;786.3969,1550.415;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;171;-518.4973,-605.5524;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CeilOpNode;25;-369.292,-46.10262;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;208;1130.182,2052.926;Inherit;False;Constant;_Float4;Float 4;12;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;147;837.163,263.8531;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;192;852.822,421.6476;Inherit;False;Constant;_Vector0;Vector 0;12;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;191;1174.254,293.9951;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;1395.6,1851.636;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;29;-208.7866,-49.67434;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;172;-318.6481,-599.0532;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;210;1374.224,2104.585;Inherit;False;Constant;_Float5;Float 5;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;209;1602.234,2001.267;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientNode;213;1796.962,-39.1756;Inherit;False;0;2;2;1,1,1,0.8500038;0,0,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-35.79423,-151.36;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;212;2050.649,1854.384;Inherit;False;Constant;_Vector1;Vector 1;12;0;Create;True;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;149;1389.847,290.8159;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;201;771.5914,-343.3264;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GradientSampleNode;214;1946.962,77.8244;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;141;1362.87,658.413;Inherit;False;Property;_RivuletsStrength;Rivulets Strength;9;0;Create;True;0;0;0;False;0;False;1;0.6;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;211;2076.894,1598.495;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;200;950.4155,-343.3264;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;2194.844,285.4623;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;203;2725.23,-208.7087;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;47;2888.927,309.0444;Inherit;False;Property;_Distortion;Distortion;1;0;Create;True;0;0;0;False;0;False;0.01;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;3216.976,-353.8261;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GrabScreenPosition;43;2990.031,-788.8883;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;41;3459.537,-461.8412;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;45;3694.517,-470.4211;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;0;False;0;False;Object;-1;True;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;59;3736.883,-1009.448;Inherit;False;Property;_Tint;Tint;3;0;Create;True;0;0;0;False;0;False;0.8809185,0.9188843,0.9245283,1;0.8624066,0.8931519,0.8962264,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StickyNoteNode;186;-1664.469,1053.786;Inherit;False;629.2295;305.6625;New Note;;1,1,1,1;Here we're sampling the same texture's R and G channels twice. R and G are the rivulet's normals for distortion. B and A are the flow map distortion channels. We scale up the distortion vectors over time, then look up the R and G textures after adding the distortion channels to the UV coordinates.$$Scaling up the distortion creates the "flow" effect, but this quickly gets ugly and causes artifacts, so you have to "reset" the UVs periodically using the waveforms at the bottom of this graph. To prevent the reset from being visible, we do the distortion twice with two different timings at opposite phases to each other, and blend between them. When one variant is being reset, the other is fully visible, so the transition is seamless.;0;0
Node;AmplifyShaderEditor.RangedFloatNode;215;4335.439,-277.3534;Inherit;False;Constant;_Float6;Float 6;12;0;Create;True;0;0;0;False;0;False;0.72;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StickyNoteNode;66;-619.4516,-265.0604;Inherit;False;775.35;406.9901;Erosion;;1,1,1,1;The animated alpha channel is run through Ceil, which effectively turns any non-black pixels white. Saturate ensures that the range is clamped from 0-1. This is multiplied against the R and G channels (the normal map, effectively) to create the final distortion mask.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;65;-2120.295,-98.30007;Inherit;False;1031.175;653.3401;Animate erosion mask;;1,1,1,1;Add the random-by-droplet B channel to the current time and get the fractional component.$This is subtracted from the alpha channel to create the erosion effect.$The Droplet Strength parameter remaps this range so that droplets are eroded further,$so only the larger droplets will be seen (and will be smaller).;0;0
Node;AmplifyShaderEditor.RangedFloatNode;55;4532.072,-288.3486;Inherit;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;4158.791,-577.4361;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StickyNoteNode;194;-2928.844,3782.269;Inherit;False;363.991;290.009;New Note;;1,1,1,1;These waveforms are for the flow map part of the shader (for the rivulets). The first two sawtooth waves are halfway out of phase with each other. The triangle wave is used to interpolate between the two flowmap variants so that you never see either variation of the flowmap reset itself.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;190;1173.421,-54.83735;Inherit;False;543.3674;236.4097;New Note;;1,1,1,1;This little check here just ensures that we don't draw rivulets on vertical-facing surfaces. If the absolute value of the dot product of the surface normal and world up is close to 1, then the surface must be nearly vertical, and we don't draw rivulets.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;187;127.3808,984.7326;Inherit;False;386.5408;323.7932;New Note;;1,1,1,1;Now we blend between the two sampled variants of our distortion texture based on a ping-ponging waveform (triangle wave). This wave is timed so that when one texture is being reprojected, the other texture is 100% visible, so that you never see a "bump" in the final result.$$This blending is necessary because each flow variant will distort itself to death if left for too long, so it needs to be reset periodically. This is exactly the same concept as Houdini's "dual rest fields" for texturing volumes.;0;0
Node;AmplifyShaderEditor.SamplerNode;218;3576.935,-725.6463;Inherit;True;Property;_GrabNoBlurTexture;_GrabNoBlurTexture;12;0;Fetch;True;0;0;0;False;0;False;218;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;217;4448.643,-734.5272;Inherit;False;Constant;_Color0;Color 0;12;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;56;4363.756,-447.5587;Inherit;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StickyNoteNode;64;-1326.288,-556.8902;Inherit;False;640.8052;361.5099;normalize to -1, 1;;1,1,1,1;LDR textures can't have negative values, so refit the range to -1, 1. ;0;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4785.206,-617.2;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Rain/RainScroller;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;145;1;144;0
WireConnection;3;0;12;0
WireConnection;143;0;3;0
WireConnection;143;2;145;0
WireConnection;78;0;77;0
WireConnection;84;0;83;0
WireConnection;76;0;143;0
WireConnection;76;2;78;0
WireConnection;124;0;92;0
WireConnection;85;0;76;0
WireConnection;85;2;84;0
WireConnection;103;1;124;0
WireConnection;119;1;92;0
WireConnection;68;0;67;0
WireConnection;68;1;85;0
WireConnection;68;7;67;1
WireConnection;101;0;119;0
WireConnection;104;0;103;0
WireConnection;90;0;68;0
WireConnection;2;0;1;0
WireConnection;2;1;76;0
WireConnection;2;7;1;1
WireConnection;123;1;92;0
WireConnection;91;0;90;2
WireConnection;91;1;90;3
WireConnection;120;0;123;0
WireConnection;120;1;121;0
WireConnection;4;0;184;0
WireConnection;183;0;2;3
WireConnection;102;0;101;0
WireConnection;106;0;104;0
WireConnection;132;0;79;0
WireConnection;113;0;120;0
WireConnection;126;0;91;0
WireConnection;180;0;183;0
WireConnection;180;1;4;0
WireConnection;134;0;113;0
WireConnection;125;0;126;0
WireConnection;125;1;107;0
WireConnection;159;0;180;0
WireConnection;133;1;132;0
WireConnection;109;0;126;0
WireConnection;109;1;108;0
WireConnection;129;0;109;0
WireConnection;129;1;85;0
WireConnection;129;2;133;0
WireConnection;181;0;2;4
WireConnection;181;1;159;0
WireConnection;97;0;125;0
WireConnection;97;1;85;0
WireConnection;97;2;133;0
WireConnection;160;0;2;0
WireConnection;160;1;161;0
WireConnection;114;0;134;0
WireConnection;62;0;61;0
WireConnection;135;0;67;0
WireConnection;135;1;129;0
WireConnection;135;7;67;1
WireConnection;34;0;181;0
WireConnection;34;1;62;0
WireConnection;162;0;160;0
WireConnection;162;1;163;0
WireConnection;95;0;67;0
WireConnection;95;1;97;0
WireConnection;95;7;67;1
WireConnection;130;0;95;0
WireConnection;130;1;135;0
WireConnection;130;2;117;0
WireConnection;171;0;162;0
WireConnection;25;0;34;0
WireConnection;191;0;147;0
WireConnection;191;1;192;0
WireConnection;207;0;130;0
WireConnection;207;1;208;0
WireConnection;29;0;25;0
WireConnection;172;0;171;0
WireConnection;172;1;171;1
WireConnection;209;0;207;0
WireConnection;209;1;210;0
WireConnection;38;0;172;0
WireConnection;38;1;29;0
WireConnection;149;0;191;0
WireConnection;201;0;38;0
WireConnection;214;0;213;0
WireConnection;214;1;149;0
WireConnection;211;0;209;0
WireConnection;211;1;212;0
WireConnection;200;0;201;0
WireConnection;200;1;201;1
WireConnection;200;2;201;2
WireConnection;200;3;201;3
WireConnection;142;0;211;0
WireConnection;142;1;141;0
WireConnection;142;2;214;1
WireConnection;203;0;200;0
WireConnection;203;1;142;0
WireConnection;46;0;203;0
WireConnection;46;1;47;0
WireConnection;41;0;43;0
WireConnection;41;1;46;0
WireConnection;45;0;41;0
WireConnection;60;0;45;0
WireConnection;60;1;59;0
WireConnection;218;1;41;0
WireConnection;0;0;217;0
WireConnection;0;2;60;0
WireConnection;0;3;56;0
WireConnection;0;4;215;0
WireConnection;0;9;55;0
ASEEND*/
//CHKSM=CF4689CA22A08DA3459A936D908AD9CCA2FB7AEF