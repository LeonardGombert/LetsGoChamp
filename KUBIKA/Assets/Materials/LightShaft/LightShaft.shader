// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/LightShaft"
{
    Properties
    {
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Color("_Color", Color) = (1,1,1,1)
        _VertexMul("_VertexMul", Float) = 1
        _VertexOffset("_VertexOffset", Float) = 1
        _DotMul("_DotMul", Float) = 1
        _DotPow("_DotPow", Float) = 1
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100

            //ZWrite Off
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha

            Pass {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag

                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord : TEXCOORD0;
                        float4 camPos : TEXCOORD1;
                        float3 normal : NORMAL;
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        half2 texcoord : TEXCOORD0;
                        float4 camPos : TEXCOORD1;
                        float3 normal : TEXCOORD2;
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;

                    fixed4 _Color;
                    half _VertexMul;
                    half _VertexOffset;
                    half _DotMul;
                    half _DotPow;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                        o.camPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                        o.normal = v.normal;
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        fixed4 col = (tex2D(_MainTex, i.texcoord) + (dot(i.normal * -1, i.camPos)) * _DotMul) * _Color * ((1 /i.texcoord.y) * _VertexMul + _VertexOffset);
                        return col;
                    }
                ENDCG
            }
    }
}
