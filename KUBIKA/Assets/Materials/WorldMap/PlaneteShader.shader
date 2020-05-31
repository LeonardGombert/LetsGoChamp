Shader "KILIAN/CUBE//PlaneteShader"
{
    Properties
    {

     _Cutoff("Alpha cutoff", Range(0,0.1)) = 0.5

        //MainTex
     _MainTex("Texture", 2D) = "white" {}
    _MainColor("_MainColor", Color) = (1,1,1,1)

     _MainTex2("Texture2", 2D) = "white" {}
    _MainColor2("_MainColor2", Color) = (1,1,1,1)

     _MainTex3("Texture3", 2D) = "white" {}
    _MainColor3("_MainColor3", Color) = (1,1,1,1)

     _MainTex4("Texture4", 2D) = "white" {}
    _MainColor4("_MainColor4", Color) = (1,1,1,1)

     _MainTex5("Texture5", 2D) = "white" {}
    _MainColor5("_MainColor5", Color) = (1,1,1,1)

     _MainTex6("Texture6", 2D) = "white" {}
    _MainColor6("_MainColor6", Color) = (1,1,1,1)


        //_EdgeTex
    _EdgeTex("_EdgeTex", 2D) = "white" {}
    _EdgeColor("_EdgeColor", Color) = (1,1,1,1)
    _EdgeTexStrength("_EdgeTexStrength", Range(0,1)) = 0




        //CUSTOMIZATION
        _Hue("Hue", Range(-360, 360)) = 0.
        _Brightness("Brightness", Range(-1, 1)) = 0.
        _Contrast("Contrast", Range(0, 2)) = 1
		_Saturation("Saturation", Range(0, 2)) = 1

		_Hue2("Hue2", Range(-360, 360)) = 0.
		_Brightness2("Brightness2", Range(-1, 1)) = 0.
		_Contrast2("Contrast2", Range(0, 2)) = 1
		_Saturation2("Saturation2", Range(0, 2)) = 1

		_Hue3("Hue3", Range(-360, 360)) = 0.
		_Brightness3("Brightness3", Range(-1, 1)) = 0.
		_Contrast3("Contrast3", Range(0, 2)) = 1
		_Saturation3("Saturation3", Range(0, 2)) = 1

		_Hue4("Hue4", Range(-360, 360)) = 0.
		_Brightness4("Brightness4", Range(-1, 1)) = 0.
		_Contrast4("Contrast4", Range(0, 2)) = 1
		_Saturation4("Saturation4", Range(0, 2)) = 1

		_Hue5("Hue5", Range(-360, 360)) = 0.
		_Brightness5("Brightness5", Range(-1, 1)) = 0.
		_Contrast5("Contrast5", Range(0, 2)) = 1
		_Saturation5("Saturation5", Range(0, 2)) = 1

		_Hue6("Hue6", Range(-360, 360)) = 0.
		_Brightness6("Brightness6", Range(-1, 1)) = 0.
		_Contrast6("Contrast6", Range(0, 2)) = 1
		_Saturation6("Saturation6", Range(0, 2)) = 1

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
            _Outline("_Outline", Range(0,0.1)) = 0
            _ColorOutline("_ColorOutline", Color) = (1,1,1,1)
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
            Cull Off

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


                 half _Cutoff;

                 sampler2D _MainTex;
                 float4 _MainTex_ST;
                 fixed4 _MainColor;

                 sampler2D _MainTex2;
                 float4 _MainTex2_ST;
                 fixed4 _MainColor2;

                 sampler2D _MainTex3;
                 float4 _MainTex3_ST;
                 fixed4 _MainColor3;

                 sampler2D _MainTex4;
                 float4 _MainTex4_ST;
                 fixed4 _MainColor4;

                 sampler2D _MainTex5;
                 float4 _MainTex5_ST;
                 fixed4 _MainColor5;

                 sampler2D _MainTex6;
                 float4 _MainTex6_ST;
                 fixed4 _MainColor6;

                 sampler2D _EdgeTex;
                 float4 _EdgeTex_ST;
                 fixed4 _EdgeColor;
                 half _EdgeTexStrength;

                 sampler2D _Emote;
                 float4 _Emote_ST;
                 half _EmoteStrength;


                 float _Hue;
                 float _Brightness;
                 float _Contrast;
                 float _Saturation;

                 float _Hue2;
                 float _Brightness2;
                 float _Contrast2;
                 float _Saturation2;

                 float _Hue3;
                 float _Brightness3;
                 float _Contrast3;
                 float _Saturation3;

                 float _Hue4;
                 float _Brightness4;
                 float _Contrast4;
                 float _Saturation4;

                 float _Hue5;
                 float _Brightness5;
                 float _Contrast5;
                 float _Saturation5;

                 float _Hue6;
                 float _Brightness6;
                 float _Contrast6;
                 float _Saturation6;

                 half _DEBUG;
                 half _DEBUG2;

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
                     float2 uv2 : TEXCOORD2;
                     float2 uv3 : TEXCOORD3;
                     float2 uv4 : TEXCOORD4;
                     float2 uv5 : TEXCOORD5;
                     float2 uv6 : TEXCOORD6;
                     half3 worldNormal : NORMAL;
                     float2 uv_Emote : TEXCOORD7;
                 };

                 struct v2f
                 {
                     float2 uv : TEXCOORD0;
                     float4 vertex : SV_POSITION;
                     float4 worldPos : TEXCOORD8;
                     half3 worldNormal : TEXCOORD9;
                     float2 uv2 : TEXCOORD2;
                     float2 uv3 : TEXCOORD3;
                     float2 uv4 : TEXCOORD4;
                     float2 uv5 : TEXCOORD5;
                     float2 uv6 : TEXCOORD6;
                     float2 uv_Emote : TEXCOORD57;
                 };


                 inline float3 applyHue(float3 aColor, float aHue)
                 {
                     float angle = radians(aHue);
                     float3 k = float3(0.57735, 0.57735, 0.57735);
                     float cosAngle = cos(angle);
                     //Rodrigues' rotation formula
                     return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
                 }


                 inline float4 applyHSBEffect(float4 startColor, float HUE, float CONTRASTE, float SATURATION, float BRIGHTNESS)
                 {

                     float4 outputColor = startColor;
                     outputColor.rgb = applyHue(outputColor.rgb, HUE);
                     outputColor.rgb = (outputColor.rgb - 0.5f) * (CONTRASTE)+0.5f;
                     outputColor.rgb = lerp(outputColor.rgb, WHITE3, BRIGHTNESS);
                     float3 intensity = dot(outputColor.rgb, float3(0.299, 0.587, 0.114));
                     outputColor.rgb = lerp(intensity, outputColor.rgb, SATURATION);

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
                     o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex2);
                     o.uv3 = TRANSFORM_TEX(v.uv2, _MainTex3);
                     o.uv4 = TRANSFORM_TEX(v.uv2, _MainTex4);
                     o.uv5 = TRANSFORM_TEX(v.uv2, _MainTex5);
                     o.uv6 = TRANSFORM_TEX(v.uv2, _MainTex6);

                     return o;
                 }

                 fixed4 frag(v2f i) : SV_Target
                 {
                     // DIRECTIONNAL LIGHT

                     float3 normal = normalize(i.worldNormal);
                     float NdotL = dot(float3(_LightDirection.x, _LightDirection.y, _LightDirection.z), normal);
                     float NdotLInverse = dot(_LightDirection.xyz, normal) * -1;

                     // sample the texture
                     fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                     fixed4 col2 = tex2D(_MainTex2, i.uv2) * _MainColor2;
                     fixed4 col3 = tex2D(_MainTex3, i.uv3) * _MainColor3;
                     fixed4 col4 = tex2D(_MainTex4, i.uv4) * _MainColor4;
                     fixed4 col5 = tex2D(_MainTex5, i.uv5) * _MainColor5;
                     fixed4 col6 = tex2D(_MainTex6, i.uv6) * _MainColor6;

                     clip(col.a - _Cutoff);
                     col.rgb *= col.a;

                     clip(col2.a - _Cutoff);
                     col2.rgb *= col2.a;

                     clip(col3.a - _Cutoff);
                     col3.rgb *= col3.a;

                     clip(col4.a - _Cutoff);
                     col4.rgb *= col4.a;

                     clip(col5.a - _Cutoff);
                     col5.rgb *= col5.a;

                     clip(col6.a - _Cutoff);
                     col6.rgb *= col6.a;


                     fixed4 edgeTex = tex2D(_EdgeTex, i.uv) * _EdgeColor * _EdgeTexStrength;
                     edgeTex.rgb *= (edgeTex.a);
                     fixed4 emote = tex2D(_Emote, i.uv3) * _EmoteStrength;
                     emote.rgb *= (emote.a);
                     //fixed4 uvBASE = lerp(_GradientExterior, _GradientCenter, (i.uv.y * _GradientMUL + _GradientOffset));
                     //fixed4 worldBASE = lerp(_GradientExterior, _GradientCenter, (((i.worldPos.y + i.worldPos.z) * 0.5)* _GradientMUL + _GradientOffset));


                     col = applyHSBEffect(col, _Hue, _Contrast, _Saturation, _Brightness);
                     col2 = applyHSBEffect(col2, _Hue2, _Contrast2, _Saturation2, _Brightness2);
                     col3 = applyHSBEffect(col3, _Hue3, _Contrast3, _Saturation3, _Brightness3);
                     col4 = applyHSBEffect(col4, _Hue4, _Contrast4, _Saturation4, _Brightness4);
                     col5 = applyHSBEffect(col5, _Hue5, _Contrast5, _Saturation5, _Brightness5);
                     col6 = applyHSBEffect(col6, _Hue6, _Contrast6, _Saturation6, _Brightness6);

                     col += col2 + col3 + col4 + col5 + col6;

                     col = saturate(col);

                     fixed4 result = ((col * (_AmbientColor + (NdotL * _LightIntensity * _LightColor)))) - edgeTex.a - emote.a;

                     return result + (edgeTex * 2) + (emote * 2);
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
