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
        [Range(-360, 360)] public float _Hue;
        [Range(0, 2)] public float _Contrast;
        [Range(0, 2)] public float _Saturation;
        [Range(-1, 1)] public float _Brightness;

        // Start is called before the first frame update
        void Start()
        {
            MatProp = new MaterialPropertyBlock();
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();

            SetScriptablePreset();
            SetMaterial();
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

            MatProp.SetFloat("_Hue", _Hue);
            MatProp.SetFloat("_Contrast", _Contrast);
            MatProp.SetFloat("_Saturation", _Saturation);
            MatProp.SetFloat("_Brightness", _Brightness);

            meshRenderer.SetPropertyBlock(MatProp);
        }

        public void SetScriptablePreset()
        {
            _MainTex = _MaterialCentral.instance.actualPack._BGTex;
            _Hue = _MaterialCentral.instance.actualPack._HueBG;
            _Contrast = _MaterialCentral.instance.actualPack._ContrastBG;
            _Saturation = _MaterialCentral.instance.actualPack._SaturationBG;
            _Brightness = _MaterialCentral.instance.actualPack._BrightnessBG;
        }
    }
}
