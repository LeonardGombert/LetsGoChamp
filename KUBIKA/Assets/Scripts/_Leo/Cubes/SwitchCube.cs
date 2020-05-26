using Kubika.CustomLevelEditor;

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
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        public void StatusUpdate()
        {
            if(!isActive)
            {
                isStatic = true;
                myCubeLayer = CubeLayers.cubeFull;
                myCubeType = CubeTypes.SwitchCube;

                SetRelevantNodeInfo();
            }

            else if (isActive)
            {
                isStatic = false;
                myCubeLayer = CubeLayers.cubeMoveable;
                myCubeType = CubeTypes.SwitchCube;

                SetRelevantNodeInfo();
            }
        }
    }
}