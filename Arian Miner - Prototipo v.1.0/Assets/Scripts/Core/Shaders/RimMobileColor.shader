Shader "Lovatto/Mobile/Rim Color" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
	    _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	}

		SubShader{
		Tags{ "RenderType" = "Opaque"  }
		LOD 100

		CGPROGRAM
        #pragma surface surf BlinnPhong


	struct Input {
		float3 _Color;
		float3 viewDir;
	};

	float4 _Color;
	float4 _RimColor;
	float _RimPower;

	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = _Color.rgb;
		o.Gloss = _Color.a;
		half rim = 1.0 - saturate(dot(Unity_SafeNormalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow(rim, _RimPower);
	}

	ENDCG
	}
		Fallback "diffuse"
}