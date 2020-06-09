using Kubika.CustomLevelEditor;
using UnityEngine;

namespace Kubika.Game
{
    public class SwitchCube : _CubeMove
    {
        public bool isActive;

        // Start is called before the first frame update
        public override void Start()
        {
            //call base.start AFTER assigning the cube's layers
            base.Start();

            ChangeEmoteFace(_EmoteIdleOffTex);
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
                ChangeEmoteFace(_EmoteIdleOffTex);
                ChangeTex(_MaterialCentral.instance.actualPack._SwitchTexOff);
                myCubeLayer = CubeLayers.cubeFull;
            }

            else if (isActive)
            {
                _DataManager.instance.moveCube.Add(this);
                isStatic = false;
                ChangeEmoteFace(_EmoteIdleTex);
                ChangeTex(_MaterialCentral.instance.actualPack._SwitchTexOn);
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
    }
}