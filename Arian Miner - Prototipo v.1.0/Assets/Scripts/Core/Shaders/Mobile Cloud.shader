Shader "Lovatto/Mobile/Cloud" {
	Properties{
		_Color("Main Color", Color) = (1,1,1)
		_MainTex("Diffuse Color", 2D) = "white" {}
	_RimTex("Edge Alpha", 2D) = "white" {}
	_RimPower("Alpha Amount", Range(0,10)) = 3.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
#pragma surface surf Lambert alpha
	struct Input {
		float2 uv_MainTex;
		float2 uv_RimTex;
		float3 viewDir;
	};
	float4 _Color;
	sampler2D _MainTex;
	sampler2D _RimTex;
	float _RimPower;
	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
		half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
		half rim2 = 1.0 - rim;
		o.Alpha = -tex2D(_RimTex, IN.uv_RimTex).rgb + (rim * _RimPower) - rim2;
	}
	ENDCG
	}
		Fallback "Diffuse"
}

