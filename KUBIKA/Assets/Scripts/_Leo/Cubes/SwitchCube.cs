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

        public virtual void StatusUpdate(bool isVictory = false)
        {
            if(!isActive)
            {
                _DataManager.instance.moveCube.Remove(this);
                isStatic = true;
                myCubeLayer = CubeLayers.cubeFull;
            }

            else if (isActive)
            {
                _DataManager.instance.moveCube.Add(this);
                isStatic = false;
                myCubeLayer = CubeLayers.cubeMoveable;
            }

            SetRelevantNodeInfo();
        }
    }
}