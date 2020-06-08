using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class _BackgroundMaterialPreset : MonoBehaviour
    {
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public MaterialPropertyBlock MatProp; // To change Mat Properties

        [Space]
        [Header("MATERIAL INFOS")]
        public Texture _MainTex;
        public Texture _RadialTexture;

        [Range(0,1)]public float _CutOff;

        [Range(-360, 360)] public float _Hue;
        [Range(0, 2)] public float _Contrast;
        [Range(0, 2)] public float _Saturation;
        [Range(-1, 1)] public float _Brightness;

        [Space]
        [Range(-360, 360)] public float _Hue2;
        [Range(0, 2)] public float _Contrast2;
        [Range(0, 2)] public float _Saturation2;
        [Range(-1, 1)] public float _Brightness2;

        // Start is called before the first frame update
        void Start()
        {
            MatProp = new MaterialPropertyBlock();
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();

            SetScriptablePreset();
        }

        // Update is called once per frame
        void Update()
        {
            SetMaterial();
        }

        public void SetMaterial()
        {
            meshRenderer.GetPropertyBlock(MatProp);

            MatProp.SetTexture("_MainTex", _MainTex);
            MatProp.SetTexture("_RadialTexture", _RadialTexture);

            MatProp.SetFloat("_CutOff", _CutOff);
            
            MatProp.SetFloat("_Hue", _Hue);
            MatProp.SetFloat("_Contrast", _Contrast);
            MatProp.SetFloat("_Saturation", _Saturation);
            MatProp.SetFloat("_Brightness", _Brightness);

            MatProp.SetFloat("_Hue2", _Hue2);
            MatProp.SetFloat("_Contrast2", _Contrast2);
            MatProp.SetFloat("_Saturation2", _Saturation2);
            MatProp.SetFloat("_Brightness2", _Brightness2);

            meshRenderer.SetPropertyBlock(MatProp);
        }

        public void SetScriptablePreset()
        {
            Debug.Log("_BGTex = " + _MaterialCentral.instance.actualPack._BGTex.name);
            _MainTex = _MaterialCentral.instance.actualPack._BGTex;
            _Hue = _MaterialCentral.instance.actualPack._HueBG;
            _Contrast = _MaterialCentral.instance.actualPack._ContrastBG;
            _Saturation = _MaterialCentral.instance.actualPack._SaturationBG;
            _Brightness = _MaterialCentral.instance.actualPack._BrightnessBG;

            SetMaterial();
        }
    }
}
