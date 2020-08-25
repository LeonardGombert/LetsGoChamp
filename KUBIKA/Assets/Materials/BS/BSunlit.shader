Shader "Custom/BSunlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("_Color", Color) = (1,1,1,1)

        _LightDir("_LightDir", Vector) = (1,1,1,1)
        _LightColorMain("_LightColorMain", Color) = (1,1,1,1)
        _LightColorAmbient("_LightColorAmbient", Color) = (1,1,1,1)
        _LightIntensity("_LightIntensity", Float) = 1
        _LightBands("LightBands", Float) = 1

        [PerRendererData] _VertexHeight("VertexHeight", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half3 worldNormal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 worldNormal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _VertexHeight;

            float4 _LightDir;
            half4 _LightColorMain;
            half4 _LightColorAmbient;
            half _LightIntensity;
            half _LightBands;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.worldNormal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float3 normal = normalize(i.worldNormal);

                float NdotL = dot(float3(_LightDir.x, _LightDir.y, _LightDir.z), normal);

                NdotL = floor(NdotL * _LightBands) / (_LightBands - 0.5);

                fixed4 Lighting = NdotL;
                Lighting *= _LightIntensity * _LightColorMain;
                Lighting += _LightColorAmbient;

                return _Color * Lighting;
            }
            ENDCG
        }
    }
}
