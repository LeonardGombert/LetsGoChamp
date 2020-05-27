// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "KILIAN/CUBE/CubeMasterShader"
{
    Properties
    {
            //MainTex
        [PerRendererData] _MainTex("Texture", 2D) = "white" {}
        [PerRendererData]_MainColor("_MainColor", Color) = (1,1,1,1)

            //_EdgeTex
        [PerRendererData]_EdgeTex("_EdgeTex", 2D) = "white" {}
        [PerRendererData]_EdgeColor("_EdgeColor", Color) = (1,1,1,1)
        [PerRendererData]_EdgeTexStrength("_EdgeTexStrength", Range(0,1)) = 0

            //_InsideTex
        [PerRendererData]_InsideTex("_InsideTex", 2D) = "white" {}
        [PerRendererData]_InsideColor("_InsideColor", Color) = (1,1,1,1)
        [PerRendererData]_InsideTexStrength("_InsideTexStrength", Range(0,1)) = 0

            //CENTER STARTS
        _GradientPoint("_GradientPoint", Vector) = (1,1,1,1)
        _GradientRadius("Radius" , Range(0,20)) = 0
        _GradientSoftness("Softness", Range(0,20)) = 0
        _GradientCenter("_GradientCenter", Color) = (1,1,1,1)
        _GradientExterior("_GradientExterior", Color) = (1,1,1,1)
        _GradientOffset("GradientOffset", Float) = 0

        _GradientMUL("GradientMUL", Float) = 0
        _GradientImpact("_GradientImpact", Range(0,2)) = 0

        //Test
        _GradientStrength("Graident Strength", Float) = 1
        _EmissiveStrengh("Emissive Strengh ", Float) = 1
        _ColorX("_ColorEdge", COLOR) = (1,1,1,1)
        _ColorY("_ColorEdge", COLOR) = (1,1,1,1)
        _ColorZ("_ColorEdge", COLOR) = (1,1,1,1)
        _ColorXMinus("_ColorEdge", COLOR) = (1,1,1,1)
        _ColorYMinus("_ColorEdge", COLOR) = (1,1,1,1)
        _ColorZMinus("_ColorEdge", COLOR) = (1,1,1,1)


        //CUSTOMIZATION
        [PerRendererData]_Hue("Hue", Range(-360, 360)) = 0.
        [PerRendererData]_Brightness("Brightness", Range(-1, 1)) = 0.
        [PerRendererData]_Contrast("Contrast", Range(0, 2)) = 1
        [PerRendererData]_Saturation("Saturation", Range(0, 2)) = 1

        //Pastille
            // Z : _OffsetPastilleX = -0.25
            //-Z : _OffsetPastilleX =  0.25
            // X : _OffsetPastilleX =  0
            //-X : _OffsetPastilleX =  0.5
            // Y : _OffsetPastilleY =  0.25
            //-Y : _OffsetPastilleY = -0.25
        [PerRendererData]_Emote("_Emote", 2D) = "white" {}
        [PerRendererData]_EmoteStrength("_EmoteStrength", Range(0,1)) = 0
        [PerRendererData]_OffsetPastilleX("_OffsetPastilleX", Range(-1,1)) = 1
        [PerRendererData]_OffsetPastilleY("_OffsetPastilleY", Range(-1,1)) = 1

       //DEBUG
        _DEBUG("_DEBUG", Float) = 0
        _DEBUG2("_DEBUG2", Float) = 0

        //SILOUETTE
        [PerRendererData]_Outline("_Outline", Range(0,0.1)) = 0
        [PerRendererData]_ColorOutline("_ColorOutline", Color) = (1,1,1,1)
        _ColorOutlineTest("_ColorOutlineTest", Color) = (1,1,1,1)
        _OutlineVertexMul("_OutlineVertexMul", Float) = 1
        _OutlineVertexOffset("_OutlineVertexOffset", Float) = 1
        _OutlineDotMul("_OutlineDotMul", Float) = 1

        //Custom Directionnal
        _LightDirection("_LightDirection", Vector) = (1,1,1,1)
        _LightIntensity("_LightIntensity", Float) = 1
        _LightColor("_LightColor", COLOR) = (1,1,1,1)
        _AmbientColor("_AmbientColor", COLOR) = (1,1,1,1)

    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100


           // UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

            Pass
            {
                CGPROGRAM
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag

                #define WHITE3 fixed3(1,1,1)
                #define UP float3(0,1,0)
                #define RIGHT float3(1,0,0)
                #define FORWARD float3(0,0,1)

                #include "UnityCG.cginc"


                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _MainColor;

                sampler2D _InsideTex;
                float4 _InsideTex_ST;
                fixed4 _InsideColor;
                half _InsideTexStrength;

                sampler2D _EdgeTex;
                float4 _EdgeTex_ST;
                fixed4 _EdgeColor;
                half _EdgeTexStrength;

                sampler2D _Emote;
                float4 _Emote_ST;
                half _EmoteStrength;

                float4 _GradientPoint;
                half _GradientRadius;
                half _GradientSoftness;

                fixed4 _GradientCenter;
                fixed4 _GradientExterior;
                half _GradientOffset;
                half _GradientMUL;
                half _GradientImpact;

                float _Hue;
                float _Brightness;
                float _Contrast;
                float _Saturation;

                half _DEBUG;
                half _DEBUG2;


                //TEST
                fixed4 _ColorX;
                fixed4 _ColorY;
                fixed4 _ColorZ;
                fixed4 _ColorXMinus;
                fixed4 _ColorYMinus;
                fixed4 _ColorZMinus;
                half _GradientStrength;
                half _EmissiveStrengh;
                float _OffsetPastilleX;
                float _OffsetPastilleY;

                //LIGHT DIRECTIONNAL
                float4 _LightDirection;
                float  _LightIntensity;
                fixed4 _LightColor;
                fixed4 _AmbientColor;

                struct appdata
                {
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float2 uv2 : TEXCOORD3;
                    float2 uv3 : TEXCOORD4;
                    half3 worldNormal : NORMAL;
                    float2 uv_Emote : TEXCOORD5;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 worldPos : TEXCOORD1;
                    half3 worldNormal : TEXCOORD2;
                    float2 uv2 : TEXCOORD3;
                    float2 uv3 : TEXCOORD4;
                    float2 uv_Emote : TEXCOORD5;
                };


                inline float3 applyHue(float3 aColor, float aHue)
                {
                    float angle = radians(aHue);
                    float3 k = float3(0.57735, 0.57735, 0.57735);
                    float cosAngle = cos(angle);
                    //Rodrigues' rotation formula
                    return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
                }


                inline float4 applyHSBEffect(float4 startColor)
                {

                    float4 outputColor = startColor;
                    outputColor.rgb = applyHue(outputColor.rgb, _Hue);
                    outputColor.rgb = (outputColor.rgb - 0.5f) * (_Contrast)+0.5f;
                    outputColor.rgb = outputColor.rgb + _Brightness;
                    float3 intensity = dot(outputColor.rgb, float3(0.299, 0.587, 0.114));
                    outputColor.rgb = lerp(intensity, outputColor.rgb, _Saturation);

                    return outputColor;
                }


                v2f vert(appdata v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    o.worldNormal = UnityObjectToWorldNormal(v.worldNormal);

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.uv2 = TRANSFORM_TEX(v.uv2, _InsideTex);
                    //o.uv3 = v.uv3 * _TexTwo_ST.xy + ((_TexTwo_ST.z + _OffsetPastilleX) + (_TexTwo_ST.w + _OffsetPastilleY));
                    o.uv3 = v.uv_Emote + float2(_OffsetPastilleX, _OffsetPastilleY);

                    half d = distance(_GradientPoint, o.worldPos);
                    o.uv2.x += o.worldPos.x * _DEBUG * o.worldPos.y * _DEBUG;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // DIRECTIONNAL LIGHT

                    float3 normal = normalize(i.worldNormal);
                    float NdotL = dot(float3(_LightDirection.x, _LightDirection.y, _LightDirection.z), normal);
                    float NdotLInverse = dot(_LightDirection.xyz, normal) * -1;


                    half d = distance(_GradientPoint, i.worldPos);
                    half sum = saturate((d - _GradientRadius) / -_GradientSoftness);

                    // sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                    fixed4 insideTex = tex2D(_InsideTex, i.uv2) * _InsideColor * _InsideTexStrength;
                    insideTex.rgb *= (insideTex.a);
                    fixed4 edgeTex = tex2D(_EdgeTex, i.uv) * _EdgeColor * _EdgeTexStrength;
                    edgeTex.rgb *= (edgeTex.a);
                    fixed4 emote = tex2D(_Emote, i.uv3) * _EmoteStrength;
                    emote.rgb *= (emote.a);
                    //fixed4 uvBASE = lerp(_GradientExterior, _GradientCenter, (i.uv.y * _GradientMUL + _GradientOffset));
                    //fixed4 worldBASE = lerp(_GradientExterior, _GradientCenter, (((i.worldPos.y + i.worldPos.z) * 0.5)* _GradientMUL + _GradientOffset));
                    fixed4 pointBASE = lerp(_GradientExterior, _GradientCenter, sum);

                    //Test

                    // lerp the 
                    half3 gradient = lerp(WHITE3, pointBASE, _GradientStrength);
                    // add ColorX if the normal is facing positive X-ish
                    half3 finalColor = _ColorX.rgb * max(0, dot(i.worldNormal, RIGHT)) * _ColorX.a;
                    finalColor += _ColorY.rgb * max(0, dot(i.worldNormal, UP)) * _ColorY.a;
                    finalColor += _ColorZ.rgb * max(0, dot(i.worldNormal, FORWARD)) * _ColorZ.a;
                    finalColor += _ColorXMinus.rgb * max(0, dot(i.worldNormal, RIGHT * -1)) * _ColorXMinus.a;
                    finalColor += _ColorYMinus.rgb * max(0, dot(i.worldNormal, UP * -1)) * _ColorYMinus.a;
                    finalColor += _ColorZMinus.rgb * max(0, dot(i.worldNormal, FORWARD * -1)) * _ColorZMinus.a;

                    // add the gradient color
                    finalColor += gradient;

                    // scale down to 0-1 values
                    finalColor = saturate(finalColor);

                    col = applyHSBEffect(col);

                    fixed4 result = ((col * (_AmbientColor + (NdotL* _LightIntensity * _LightColor)))) - edgeTex.a - insideTex.a - emote.a;

                    return result + (edgeTex * 2) + (insideTex * 2) + (emote * 2);
                }
                ENDCG
            }

            Pass{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100
            Cull Front
             Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 camPos : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                float4 camPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };
            sampler2D _MainTex;
            half _Outline;
            fixed4 _ColorOutline;
            fixed4 _ColorOutlineTest;
			half _OutlineVertexMul;
			half _OutlineVertexOffset;
			half _OutlineDotMul;

            v2f vert(appdata v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = mul((float3x3) UNITY_MATRIX_MV, v.normal);
                o.normal.x *= UNITY_MATRIX_P[0][0];
                o.normal.y *= UNITY_MATRIX_P[1][1];
                o.pos.xy += o.normal.xy * _Outline;
                o.camPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //fixed4 col = (/*tex2D(_MainTex, i.texcoord) + */dot(i.normal , i.camPos) * _OutlineDotMul + _OutlineVertexOffset)* _ColorOutlineTest; /////////TODO : WAIT
                fixed4 col = _ColorOutline;
                return col;
            }

            ENDCG
            }
        }
                
        FallBack "VertexLit"
}
