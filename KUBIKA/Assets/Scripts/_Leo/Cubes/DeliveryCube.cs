using System;
using UnityEngine;

namespace Kubika.Game
{
    public class DeliveryCube : _CubeScanner
    {
        bool touchingVictory;
        private bool locked;

        // Start is called before the first frame update
        public override void Start()
        {
            //call base.start AFTER assigning the cube's layers
            base.Start();

            _DataManager.instance.EndFalling.AddListener(CheckForVictory);
            _DataManager.instance.EndFalling.AddListener(ScannerSet);
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            //Debug.Log("Cheking in " + myIndex + _DirectionCustom.LocalScanner(facingDirection));
        }

        private void CheckForVictory()
        {
            //touchingVictory = ProximityChecker(_DirectionCustom.fixedUp, CubeTypes.BaseVictory, CubeLayers.None);
            touchingVictory = VictoryChecker(_DirectionCustom.LocalScanner(facingDirection));

            if (touchingVictory && locked == false)
            {
                locked = true;
                VictoryConditionManager.instance.IncrementVictory();
            }

            // flip the bools when the delivery cube loses track of the victory cube
            if (touchingVictory == false && locked == true)
            {
                locked = false;
                VictoryConditionManager.instance.DecrementVictory();
            }
        }
    }
}