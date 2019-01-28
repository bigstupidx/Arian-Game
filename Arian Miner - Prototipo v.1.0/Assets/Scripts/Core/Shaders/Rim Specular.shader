Shader "Lovatto/Rim/Specular" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)	
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	    _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	     
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	}
	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong


		sampler2D _MainTex;
		fixed4 _Color;
		
		float4 _RimColor;
		float _RimPower;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;

			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow (rim, _RimPower);
		}
		ENDCG
	}

	FallBack "Specular"
}

