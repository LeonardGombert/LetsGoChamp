using Kubika.CustomLevelEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Kubika.Game
{
    public class SwitchCube : _CubeMove
    {
        public bool isActive;
        protected bool isSwitchVictory = false;

        // FEEDBACKS
        float actualnsideStrength;
        float currentOfValueChange;
        float timeOfValueChange = 0.5f;
        float currentValue;
        float maxValueStrnght = 1;

        // Start is called before the first frame update
        public override void Start()
        {
            //call base.start AFTER assigning the cube's layers
            base.Start();

            SetOutlineColor(false);
            ChangeEmoteFace(_EmoteIdleOffTex);
            isSelectable = false;
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        public void StatusUpdate(bool isVictory = false)
        {
            if(!isActive)
            {
                _DataManager.instance.moveCube.Remove(this);
                isStatic = true;
                if(isSwitchVictory == false)
                    ChangeTex(_MaterialCentral.instance.actualPack._SwitchTexOff);
                else
                    ChangeTex(_MaterialCentral.instance.actualPack._SwitchVTexOff);

                StartCoroutine(IncreaseInside(false));
                myCubeLayer = CubeLayers.cubeFull;
            }

            else if (isActive)
            {
                _DataManager.instance.moveCube.Add(this);
                isStatic = false;
                if (isSwitchVictory == false)
                    ChangeTex(_MaterialCentral.instance.actualPack._SwitchTexOn);
                else
                    ChangeTex(_MaterialCentral.instance.actualPack._SwitchVTexOn);

                StartCoroutine(IncreaseInside(true));
                myCubeLayer = CubeLayers.cubeMoveable;
            }

            SetRelevantNodeInfo();
        }

        void ChangeTex(Texture tex)
        {
            meshRenderer.GetPropertyBlock(MatProp);

            MatProp.SetTexture("_MainTex", tex);

            meshRenderer.SetPropertyBlock(MatProp);
        }


        IEnumerator IncreaseInside(bool isON)
        {
            meshRenderer.GetPropertyBlock(MatProp);
            currentOfValueChange = 0;

            if (isON)
            {
                actualnsideStrength = _InsideStrength;

                while (currentOfValueChange <= timeOfValueChange)
                {
                    currentOfValueChange += Time.deltaTime;

                    currentValue = Mathf.SmoothStep(actualnsideStrength, maxValueStrnght, currentOfValueChange / timeOfValueChange);

                    Debug.Log("CHANGING COLOR ON");
                    MatProp.SetFloat("_InsideTexStrength", currentValue);

                    meshRenderer.SetPropertyBlock(MatProp);
                    yield return currentValue;
                }
            }
            else
            {
                while (currentOfValueChange <= timeOfValueChange)
                {
                    currentOfValueChange += Time.deltaTime;

                    currentValue = Mathf.SmoothStep(maxValueStrnght, actualnsideStrength, currentOfValueChange / timeOfValueChange);

                    Debug.Log("CHANGING COLOR OFF");
                    MatProp.SetFloat("_InsideTexStrength", currentValue);

                    meshRenderer.SetPropertyBlock(MatProp);
                    yield return currentValue;
                }
            }

        }

        public void SetOutlineColor(bool isWhite)
        {
            meshRenderer.GetPropertyBlock(MatProp);

            if (isWhite == true)
                MatProp.SetColor("_ColorOutline", Color.white);
            else
                MatProp.SetColor("_ColorOutline", Color.black);

            meshRenderer.SetPropertyBlock(MatProp);
        }

    }
}