Shader "Custom/TriplanarMapping"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Texture Scale", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float _Scale;

        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 blending = abs(IN.worldNormal);
            blending = normalize(max(blending, 0.00001));
            float bSum = blending.x + blending.y + blending.z;
            blending /= bSum;

            float2 xProj = IN.worldPos.yz * _Scale;
            float2 yProj = IN.worldPos.xz * _Scale;
            float2 zProj = IN.worldPos.xy * _Scale;

            float4 xTex = tex2D(_MainTex, xProj);
            float4 yTex = tex2D(_MainTex, yProj);
            float4 zTex = tex2D(_MainTex, zProj);

            o.Albedo = xTex.rgb * blending.x + yTex.rgb * blending.y + zTex.rgb * blending.z;
            o.Alpha = 1.0;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
