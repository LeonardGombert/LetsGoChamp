// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/WaterBG"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _NoiseTex("NoiseTex", 2D) = "white" {}
        _NoiseTilling("_NoiseTilling", Float) = 1
        _NoiseSpeed("_NoiseSpeed", Float) = 1

        _LightDir("_LightDir", Vector) = (1,1,1,1)
        _LightColorMain("_LightColorMain", Color) = (1,1,1,1)
        _LightColorAmbient("_LightColorAmbient", Color) = (1,1,1,1)
        _LightIntensity("_LightIntensity", Float) = 1
        _LightBands("LightBands", Float) = 1


        _WaveAmplitude("_WaveAmplitude", Float) = 1

        _WaterColorSurface("_WaterColorSurface", Color) = (1,1,1,1)
        _WaterColorDeep("_WaterColorDeep", Color) = (1,1,1,1)

        _WaterMax("_WaterMax", Float) = 1
        _WaterMin("_WaterMin", Float) = 1
        _WaterOffsetHeight("_WaterOffsetHeight", Float) = 1

        _FoamTest("FoamTest", Float) = 1
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" }
            //LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture;

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;

            float _NoiseSpeed;
            float _NoiseTilling;
            float2 NoiseUV;

            float _WaveAmplitude;
            half4 _WaterColorSurface;
            half4 _WaterColorDeep;
            half _WaterMax;
            half _WaterMin;
            half _WaterOffsetHeight;

            float4 _LightDir;
            half4 _LightColorMain;
            half4 _LightColorAmbient;
            half _LightIntensity;
            half _LightBands;

            float4 _WorldPos;

            half _FoamTest;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 vertCol : COLOR;
                half3 worldNormal : NORMAL;
                float4 screenPos : TEXCOORD3;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 vertCol : COLOR;
                float4 worldPos : TEXCOORD1;
                half3 worldNormal : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };



            v2f vert (appdata v)
            {
                v2f o;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
                o.vertCol = v.vertCol;
                o.worldNormal = UnityObjectToWorldNormal(v.worldNormal);

                NoiseUV = (o.worldPos.xz * (_NoiseTex_ST.xy * _NoiseTilling)) + float2(_Time.x * _NoiseSpeed, _Time.x * _NoiseSpeed);

                fixed4 noiseDisp = tex2Dlod(_NoiseTex, float4(NoiseUV, 0, 0)); //

                o.worldPos.y += (noiseDisp.r * _WaveAmplitude * o.vertCol) - (_WaveAmplitude * o.vertCol);
                
                _WorldPos = o.worldPos;

                o.vertex = mul(UNITY_MATRIX_VP, o.worldPos);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //Depth
                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
                half4 foamLine = 1 - saturate(_FoamTest * (depth - i.screenPos.w));


                // sample the texture

                float3 normal = normalize(i.worldNormal); 

                float NdotL = dot(float3(_LightDir.x, _LightDir.y, _LightDir.z), normal);

                NdotL = floor(NdotL * _LightBands) / (_LightBands - 0.5);

                fixed4 Lighting = NdotL;
                Lighting *= _LightIntensity * _LightColorMain;
                Lighting += _LightColorAmbient;

                // COLOR WAVE
                float waveHeight = i.worldPos.y - _WaterOffsetHeight;

                float _YDepth = clamp(((waveHeight - _WaterMin) / (_WaterMax - _WaterMin)), _WaterMin, _WaterMax);
                float4 _YColor = lerp(_WaterColorDeep, _WaterColorSurface, clamp(_YDepth, 0,1));


                fixed4 col = _YColor * Lighting;
                col += foamLine;
                return col;

            }
            ENDCG
        }
    }
}
