Shader "Custom/SpecularTiling" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (_tex)", 2D) = "white" {}
		_Specular("Specular(_Sp)", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_EmissionPower("Emission Power", Float) = 1
		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_Emission("Emission(_M)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Specular;
		sampler2D _Normal;
		sampler2D _Emission;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Specular;
			float2 uv_Normal;
			float2 uv_Emission;
		};

		half _Glossiness;
		fixed4 _Color;
		float  _EmissionPower;
		fixed4 _EmissionColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo   = c.rgb;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			
			c = tex2D(_Specular, IN.uv_Specular) * _Color;
			o.Specular = c.rgb;

			c = tex2D(_Normal,   IN.uv_Normal);
			o.Normal   = UnpackNormal(c);

			c = tex2D(_Emission, IN.uv_Emission) * _EmissionColor * _EmissionPower;
			o.Emission = c.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
