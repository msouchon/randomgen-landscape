// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 2016

//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/PhongShader"
{
	Properties
	{
		// Control Texture ("Splat Map")
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}

		// Terrain textures - each weighted according to the corresponding colour
		// channel in the control texture
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}

		// Used in fallback on old cards & also for distant base map
		[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color("Main Color", Color) = (1,1,1,1)

		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)

		// Using a Float to represent a Boolean
		_Blend("Blend Textures", Float) = 1
		_BlendAmount("Blend Amount", Float) = 0.1

		_Kd("Kd", Float) = 1
		_fAtt("fAtt", Float) = 1
		_Ks("Ks", Float) = 1
		_specN("specN", Float) = 5
		_Ka("Ka", Float) = 1

	}
	SubShader
	{ 
		Tags {
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _Control;
			float4 _Control_ST;
			float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
			uniform sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
			uniform fixed4 _Color;

			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;

			// Using a Float to represent a Boolean
			uniform float _Blend;
			uniform float _BlendAmount;
			uniform float _Kd;
			uniform float _fAtt;
			uniform float _Ka;
			uniform float _specN;
			uniform float _Ks;


			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
				float2 uv_Control : TEXCOORD0;
				float2 uv_Splat0 : TEXCOORD1;
				float2 uv_Splat1 : TEXCOORD2;
				float2 uv_Splat2 : TEXCOORD3;
				float2 uv_Splat3 : TEXCOORD4;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 worldVertex : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float2 uv_Control : TEXCOORD2;
				float2 uv_Splat0 : TEXCOORD3;
				float2 uv_Splat1 : TEXCOORD4;
				float2 uv_Splat2 : TEXCOORD5;
				float2 uv_Splat3 : TEXCOORD6;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				// Convert Vertex position and corresponding normal into world coords.
				// Note that we have to multiply the normal by the transposed inverse of the world 
				// transformation matrix (for cases where we have non-uniform scaling; we also don't
				// care about the "fourth" dimension, because translations don't affect the normal) 
				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

				// Transform vertex in world coordinates to camera coordinates, and pass colour
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				// Handle terrain textures
				o.uv_Control = TRANSFORM_TEX(v.uv_Control, _Control);
				o.uv_Splat0 = TRANSFORM_TEX(v.uv_Splat0, _Splat0);
				o.uv_Splat1 = TRANSFORM_TEX(v.uv_Splat1, _Splat1);
				o.uv_Splat2 = TRANSFORM_TEX(v.uv_Splat2, _Splat2);
				o.uv_Splat3 = TRANSFORM_TEX(v.uv_Splat3, _Splat3);

				// Pass out the world vertex position and world normal to be interpolated
				// in the fragment shader (and utilised)
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;

				return o;
			}

			// Function to make blending between textures sharper
			float4 blend(float4 texA, float aA, float4 texB, float aB)
			{
				float ma = max(texA.a + aA, texB.a + aB) - _BlendAmount;

				float b1 = max(texA.a + aA - ma, 0.0);
				float b2 = max(texB.a + aB - ma, 0.0);

				return (texA * b1 + texB * b2) / (b1 + b2);
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = _Ka;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = _fAtt;
				float Kd = _Kd;
				float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
				float LdotN = dot(L, interpNormal);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float Ks = _Ks;
				float specN = _specN; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				// Using classic reflection calculation:
				//float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				//float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);
				// Using Blinn-Phong approximation:
				//specN = 10; // We usually need a higher specular power when using Blinn-Phong
				float3 H = normalize(V + L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = v.color.a;

				// Calculate colours for terain textures
				fixed4 splat_control = tex2D(_Control, v.uv_Control);

				// Dont blend textures if not set
				if (_Blend == 0) {
					fixed3 col;
					col = splat_control.r * tex2D(_Splat0, v.uv_Splat0).rgb;
					col += splat_control.g * tex2D(_Splat1, v.uv_Splat1).rgb;
					col += splat_control.b * tex2D(_Splat2, v.uv_Splat2).rgb;
					col += splat_control.a * tex2D(_Splat3, v.uv_Splat3).rgb;
					returnColor.rgb = returnColor.rgb * col * _Color;

					return returnColor;
				}

				// If not, blend
				fixed4 col;
				col = blend(tex2D(_Splat0, v.uv_Splat0), splat_control.r, tex2D(_Splat1, v.uv_Splat1), splat_control.g);
				col = blend(col, splat_control.r + splat_control.g, tex2D(_Splat2, v.uv_Splat2), splat_control.b);
				col = blend(col, splat_control.r + splat_control.g + splat_control.b, tex2D(_Splat3, v.uv_Splat3), splat_control.a);

				returnColor.rgb = returnColor.rgb * col.rgb * _Color;

				return returnColor;
			}
			ENDCG
		}
	}
}
