Shader "Custom/FoliageMat"
{
        Properties
        {
            _MainTex("Texture", 2D) = "white" {}

            _ObjectStrentgh("_ObjectStrentgh" , Float) = 0
            _GrassDownLevel("_GrassDownLevel" , Float) = 0

            _Color("Grass Color" , Color) = (1,1,1,1)

            _Cutoff("Alpha cutoff", Range(0,1)) = 0.5

            _NoiseTex("_NoiseTex", 2D) = "white" {}
            _WindSpeed("_WindSpeed", Float) = 0.5
            _WindMouv("_WindMouv", Float) = 0.5
            _WindOffset("_WindOffset", Float) = 0.5
            _WindStrength("_WindStrength", Float) = 0.5

        }
            SubShader
            {
                Tags {  "RenderType" = "Opaque" }
                LOD 100
                Cull Off // turn off backface culling

                Pass
                {
                    CGPROGRAM
                    #pragma multi_compile_instancing
                    #pragma vertex vert
                    #pragma fragment frag
                    #include "UnityCG.cginc"


                    sampler2D _MainTex;
                    float4 _MainTex_ST;

                    half _WindSpeed;
                    half _WindMouv;
                    half _WindOffset;
                    half _WindStrength;

                    fixed4 _Color;

                    half _Cutoff;

                    float Sign(float number)
                    {
                        return number < 0 ? -1 : (number > 0 ? 1 : 0);
                    }

                    struct appdata
                    {
                        float4 vertex : POSITION;
                        float2 uv : TEXCOORD0;
                        float2 noise : TEXCOORD1;
                        float4 worldPos : TEXCOORD2;
                    };

                    struct v2f
                    {
                        float2 uv : TEXCOORD0;
                        float2 noise : TEXCOORD1;
                        float4 worldPos : TEXCOORD2;
                        float4 vertex : SV_POSITION;
                    };

                    v2f vert(appdata v)
                    {
                        v2f o;

                        //Link to the ball
                        o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                        //Offset && Tilling
                        o.vertex.x += step(0, o.uv.y - _WindOffset) * (sin(((_Time.x * _WindSpeed) + (o.worldPos.x * _WindMouv + o.worldPos.z * _WindMouv))) * _WindStrength);

                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        // sample the texture
                        fixed4 col = tex2D(_MainTex, i.uv);

                        clip(col.a - _Cutoff);
                        col.rgb *= col.a;
                        col *= _Color;

                        return col;
                    }
                    ENDCG
                }
            }
    }
