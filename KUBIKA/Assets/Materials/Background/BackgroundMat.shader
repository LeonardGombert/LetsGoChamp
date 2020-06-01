Shader "Unlit/BackgroundMat"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
        [PerRendererData]_RadialTexture("_RadialTexture", 2D) = "white" {}
        [PerRendererData]_CutOff("_CutOff", Range(0,1)) = 0.5

	    //CUSTOMIZATION
        [PerRendererData]_Hue("Hue", Range(-360, 360)) = 0.
        [PerRendererData]_Brightness("Brightness", Range(-1, 1)) = 0.
        [PerRendererData]_Contrast("Contrast", Range(0, 2)) = 1
        [PerRendererData]_Saturation("Saturation", Range(0, 2)) = 1

        //2nd IMAGE
        [PerRendererData]_Hue2("Hue2", Range(-360, 360)) = 0.
        [PerRendererData]_Brightness2("Brightness2", Range(-1, 1)) = 0.
        [PerRendererData]_Contrast2("Contrast2", Range(0, 2)) = 1
        [PerRendererData]_Saturation2("Saturation2", Range(0, 2)) = 1
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

                float _Hue;
                float _Brightness;
                float _Contrast;
                float _Saturation;

                float _Hue2;
                float _Brightness2;
                float _Contrast2;
                float _Saturation2;

                half _CutOff;

                sampler2D _MainTex;
                float4 _MainTex_ST;

                sampler2D _RadialTexture;
                float4 _RadialTexture_ST;

                inline float3 applyHue(float3 aColor, float aHue)
                {
                    float angle = radians(aHue);
                    float3 k = float3(0.57735, 0.57735, 0.57735);
                    float cosAngle = cos(angle);
                    //Rodrigues' rotation formula
                    return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
                }


                inline float4 applyHSBEffect(float4 startColor, float HUE, float CONTRAST, float SATURATION, float BRIGTHNESS)
                {

                    float4 outputColor = startColor;
                    outputColor.rgb = applyHue(outputColor.rgb, HUE);
                    outputColor.rgb = (outputColor.rgb - 0.5f) * (CONTRAST)+0.5f;
                    outputColor.rgb = outputColor.rgb + BRIGTHNESS;
                    float3 intensity = dot(outputColor.rgb, float3(0.299, 0.587, 0.114));
                    outputColor.rgb = lerp(intensity, outputColor.rgb, SATURATION);

                    return outputColor;
                }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _RadialTexture);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_MainTex, i.uv);

                fixed4 transition = tex2D(_RadialTexture, i.uv2);

                col = applyHSBEffect(col, _Hue, _Contrast, _Saturation, _Brightness);
                col2 = applyHSBEffect(col, _Hue2, _Contrast2, _Saturation2, _Brightness2);

                if (transition.b < _CutOff)
                {
                    return col;
                }
                else
                {
                    return col2;
                }
            }
            ENDCG
        }
    }
}
